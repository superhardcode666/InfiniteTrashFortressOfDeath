using System;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Game;
using UnityEngine;
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

namespace Hardcode.ITFOD.Player
{
    public class PlayerController : MonoBehaviour
    {
        public void OnCreated(Tilemap floorMap, Tilemap wallsMap, Vector3 position)
        {
            floor = floorMap;
            walls = wallsMap;
            transform.position = position;

            #region Singleton

            if (instance != null)
            {
                Debug.Log("Dude, why are there multiple Player Instances?");
                return;
            }

            instance = this;

            #endregion

            playerMovePoint.transform.parent = null;
        }

        public void ResetPosition(Vector3 position)
        {
            Debug.Log($"PlayerPosition reset to {position}");

            playerMovePoint.position = position;
            transform.position = position;
        }

        public void OnUpdate()
        {
            transform.position =
                Vector3.MoveTowards(transform.position, playerMovePoint.position, playerSpeed * Time.deltaTime);
        }

        public void ProcessMovement(Vector2 direction)
        {
            if (CanMove(direction))
            {
                playerMovePoint.position += (Vector3) direction;
                playAudio?.Invoke(playerStep);
                increaseStepCounter?.Invoke();
            }

            if (direction.x == -1)
                spriteRenderer.flipX = true;
            else if (direction.x == 1) spriteRenderer.flipX = false;

            var cellPosition = floor.LocalToCell(playerMovePoint.localPosition);
            var origin = floor.GetCellCenterLocal(cellPosition);


            interactor.CheckForFocus(origin, direction);
        }

        private bool CanMove(Vector2 direction)
        {
            var gridPosition = floor.WorldToCell(playerMovePoint.position + (Vector3) direction);

            if (!floor.HasTile(gridPosition) || walls.HasTile(gridPosition) ||
                roomManager.spawnPositions.Contains((Vector2Int) gridPosition)) return false;

            return true;
        }

        #region Field Declarations

        private GameManager gameManager;

        [SerializeField] private Transform playerMovePoint;
        [SerializeField] private GameObject playerSprite;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float playerSpeed = 1f;

        public static event Action<EventReference> playAudio;
        [SerializeField] private EventReference playerStep;

        private Tilemap floor;
        private Tilemap walls;

        [SerializeField] private Interactor interactor;
        [SerializeField] private RoomManager roomManager;

        private static PlayerController instance;

        public static event Action increaseStepCounter;

        #endregion
    }
}