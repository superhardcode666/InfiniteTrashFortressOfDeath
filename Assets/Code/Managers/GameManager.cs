using System;
using System.Collections.Generic;
using Cinemachine;
using FMODUnity;
using Hardcode.ITFOD.Camera;
using Hardcode.ITFOD.Combat;
using Hardcode.ITFOD.Items;
using Hardcode.ITFOD.Player;
using Hardcode.ITFOD.Procedural;
using Hardcode.ITFOD.SAVEDATA;
using Hardcode.ITFOD.StatSystem;
using Hardcode.ITFOD.UI;
using Hardcode.ITFOD.Audio;
using ScottSteffes.AnimatedText;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/*      __  _____    ____  ____ 
       / / / /   |  / __ \/ __ \
      / /_/ / /| | / /_/ / / / /
     / __  / ___ |/ _, _/ /_/ / 
    /_/_/_/_/__|_/_/_|_/_____/_ 
      / ____/ __ \/ __ \/ ____/ 
     / /   / / / / / / / __/    
    / /___/ /_/ / /_/ / /___    
    \____/\____/_____/_____/ 
        アンフェタミンを燃料
*/
namespace Hardcode.ITFOD.Game
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            playerMovementControls = new PlayerMovement();
        }

        private void Start()
        {
            CombatManager.changeGameState += UpdateGameState;
            DialogManager.changeGameState += UpdateGameState;
            UIManager.inventoryToggle += UpdateGameState;

            Exit.triggerMapChange += MapChange;

            playerMovementControls.Enable();
            uiManager.EnableUIControls();

            Debug.Log("<color=cyan>GameManager:</color> Dependencies hooked up successfully.");
            Debug.Log("<color=cyan>GameManager:</color> Initializing Game Systems...");

            InitGame();

            GameEvents.switchMusic?.Invoke(MusicModes.Dungeon);
        }

        private void Update()
        {
            #region Camera Intro Stuff

            cameraFade.OnUpdate();

            if (Vector3.Distance(virtualCamera.transform.position, playerStartPos) >= 0.15f)
                cameraController.MoveToTarget(virtualCameraTarget);

            virtualCamera.Follow = playerTransform;

            #endregion

            if (state == GameState.Combat) combatManager.OnUpdate();
            if (state == GameState.Explore)
            {
                playerController.OnUpdate();
                fogOfWar.OnUpdate();
            }
        }

        private void OnDestroy()
        {
            CombatManager.changeGameState -= UpdateGameState;
            DialogManager.changeGameState -= UpdateGameState;
            UIManager.inventoryToggle -= UpdateGameState;

            Exit.triggerMapChange -= MapChange;

            playerMovementControls.Disable();
            uiManager.DisableUIControls();

            Debug.Log("<color=green>GameManager:</color> Unsubbed from all Events, shutting down.");
        }

        public void UpdateGameState(GameState newState)
        {
            state = newState;

            switch (state)
            {
                case GameState.Explore:
                    cameraController.ZoomOut();
                    startTrackingThisRun?.Invoke();
                    playerMovementControls.Enable();
                    playerInteractor.ToggleInteractor(true);
                    break;
                case GameState.Combat:
                    pauseTrackingThisRun?.Invoke();
                    playerMovementControls.Disable();
                    playerInteractor.ToggleInteractor(false);
                    break;
                case GameState.Cutscene:
                    cameraController.ZoomIn();
                    pauseTrackingThisRun?.Invoke();
                    playerMovementControls.Disable();
                    playerInteractor.ToggleInteractor(false);
                    break;
                case GameState.GameOver:
                    endTrackingThisRun?.Invoke();
                    saveCurrentRunData?.Invoke();
                    Invoke("GameOver", 3f);
                    playerMovementControls.Disable();
                    playerInteractor.ToggleInteractor(false);
                    break;
                case GameState.Menu:
                    playerMovementControls.Disable();
                    playerInteractor.ToggleInteractor(false);
                    break;
                case GameState.Loading:
                    playerMovementControls.Disable();
                    playerInteractor.ToggleInteractor(false);
                    break;
                default:
                    Debug.Log($"State: {newState} not implemented!");
                    break;
            }

            gameStateChanged?.Invoke(newState);
        }

        private void InitGame()
        {
            dungeonFloor = 1;

            roomManager.OnCreated();

            Debug.Log("First Call of GenerateMap");
            GenerateMap();

            #region (Data Management (Saving / Loading) and Run Tracking)

            if (!runTracker.TrackerInitialized)
            {
                // Run Tracking & Data Management
                // Hooks up Event Dependencies, this should only be called on a fresh Run 
                runTracker.OnCreated();
                // Looks for PlayerData from a previous run and delete it,
                // then creates a fresh PlayerData Object and Timer
                runTracker.InitTracker();
            }

            #endregion

            #region Fresh Run Initializations

            playerController.OnCreated(floor, walls, playerStartPos);
            virtualCameraTarget = new Vector3(playerStartPos.x, playerStartPos.y, -10);
            playerMovementControls.Main.Movement.performed +=
                ctx => playerController.ProcessMovement(ctx.ReadValue<Vector2>());

            cameraController.OnCreated();
            cameraFade.OnCreated();

            dialogManager.OnCreated();
            combatManager.OnCreated();

            uiManager.InitScreenTransition();

            inventoryManager.OnCreated();
            equipmentManager.OnCreated();
            uiManager.InitInventoryUi();
            uiManager.InitEquipmentUi();

            playerStats.OnCreated();
            playerStats.InitStats();

            uiManager.InitCombatUi();
            uiManager.OnCreated();

            uiManager.InitStatsUi();
            playerStats.UpdateStatUI();

            #endregion

            // Start Run Tracker & Run Timer
            startTrackingThisRun?.Invoke();

            UpdateGameState(GameState.Explore);
        }

        private void GenerateMap()
        {
            #region (Dungeon Map, Spawning, FOW, PlayerPos, Camera Stuff)

            // Fetch all Room Floor Boundsints as List
            // Generate Room Objects accordingly assign bounds 
            // Set Room Types:
            // 1st Room is Startroom, player will be spawned here
            // last Room is Bossroom, boss is spawned here
            // all rooms in between will be randomly assigned as either hostile, shop, treasure or trap
            
            var roomBounds = dungeonGenerator.GenerateRoomBounds();
            
            // Spawn Stuff, Clear old spawnPositions
            roomManager.InitRooms(roomBounds, floor);
            roomManager.SpawnObjects();

            // Generate a new Start Position for the Player and put them there
            playerStartPos = floor.GetCellCenterWorld(roomManager.GetPlayerStartPosition());

            fogOfWar.OnCreated();

            //Invoke onMapGenerated Event here if necessary for other Systems
            //for example SetPlayerPosition(position)

            #endregion
        }

        private void MapChange()
        {
            pauseTrackingThisRun?.Invoke();

            dungeonFloor += 1;

            ClearActiveEntities();

            roomManager.ResetSpawnPositions();
            GenerateMap();
            SetPlayerPosition();

            getMapFloorName?.Invoke(dungeonFloor);

            startTrackingThisRun?.Invoke();
        }

        private void SetPlayerPosition()
        {
            // Gets called on every new Floor after the Map is done generating
            playerController.ResetPosition(playerStartPos);
        }

        private void ClearActiveEntities()
        {
            if (activeEntities.childCount > 0)
            {
                Debug.Log($"<color=cyan>GameManager:</color> Found {activeEntities.childCount} active Entities.");

                foreach (Transform child in activeEntities) Destroy(child.gameObject);

                Debug.Log("<color=cyan>GameManager:</color> All Entities purged.");
            }

            Debug.Log("<color=cyan>GameManager:</color> Found no active Entities.");
        }

        private void GameOver()
        {
            SceneManager.LoadScene(2);
        }

        #region Field Declarations

        public static event Action<GameState> gameStateChanged;

        // Player Input
        private PlayerMovement playerMovementControls;
        [SerializeField] private Interactor playerInteractor;

        [Header("Run Tracker")] [SerializeField]
        private RunTracker runTracker;

        // Actions related to controlling the RunTracker
        public static event Action startTrackingThisRun;
        public static event Action pauseTrackingThisRun;
        public static event Action endTrackingThisRun;

        public static event Action saveCurrentRunData;

        [Header("Dungeon Generation")] [SerializeField]
        private BasicDungeonGenerator dungeonGenerator;

        [SerializeField] private int dungeonFloor;
        public static event Action<int> getMapFloorName;

        [SerializeField] private RoomManager roomManager;
        [SerializeField] private FogOfWar fogOfWar;

        [Header("Player")] [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private Transform playerTransform;

        private Vector3 playerStartPos;
        private Vector3 virtualCameraTarget;

        [Header("Inventory")] [SerializeField] private InventoryManager inventoryManager;
        [Header("Equipment")] [SerializeField] private EquipmentManager equipmentManager;
        [Header("Game UI")] [SerializeField] private UIManager uiManager;
        [Header("Combat")] [SerializeField] private CombatManager combatManager;
        [Header("Dialog")] [SerializeField] private DialogManager dialogManager;

        [Header("Camera Stuff")] [SerializeField]
        private CinemachineVirtualCamera virtualCamera;

        [SerializeField] private CameraController cameraController;
        [SerializeField] private CameraFade cameraFade;

        private bool cameraFollowSet;

        [SerializeField] private Tilemap floor;
        [SerializeField] private Tilemap walls;

        [Header("Active Entities")] public Transform activeEntities;

        public GameState state;
        
        public static event Action<EventReference> playMusic;

        #endregion
    }
}

public enum GameState
{
    Explore,
    Combat,
    Cutscene,
    GameOver,
    Menu,
    Loading
}