using System;
using System.Collections.Generic;
using System.Linq;
using Hardcode.ITFOD.Interactions;
using Hardcode.ITFOD.Items;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

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
    public class RoomManager : MonoBehaviour
    {
        private void OnDestroy()
        {
            GameEvents.onClearSpawnPosition -= RemoveSpawnPosition;
            GameEvents.onLootDropped -= SpawnLootChest;

            Debug.Log("<color=green>RoomManager:</color> Unsubbed from all Events, shutting down.");
        }

        #region static Actions

        public static event Action<IInteractable> setLootFocus;

        #endregion

        public void OnCreated()
        {
            GameEvents.onClearSpawnPosition += RemoveSpawnPosition;
            GameEvents.onLootDropped += SpawnLootChest;

            Debug.Log("<color=green>RoomManager:</color> Set up Dependencies, booting up.");
        }

        public void InitRooms(List<BoundsInt> roomBounds, Tilemap floorMap)
        {
            ResetSpawnPositions();
            rooms = roomBounds;
            floor = floorMap;
        }
        
        public Vector3Int GetPlayerStartPosition()
        {
            // SetUpAllRooms();
            startRoom = DetermineStartRoom();
            bossRoom = DetermineBossRoom();

            SpawnExit();

            return floor.WorldToCell(startRoom.center);
        }

        [SerializeField] private List<Room> allRoomsInCurrentMap;
        
        public void SetUpAllRooms(List<BoundsInt> roomBounds, Tilemap floorMap)
        {
            ResetSpawnPositions();
            
            rooms = roomBounds;
            floor = floorMap;
            
            CreateRooms(roomBounds);
        }
        private void CreateRooms(List<BoundsInt> roomBounds)
        {
            allRoomsInCurrentMap.Clear();
            
            foreach (BoundsInt boundsInt in roomBounds)
            {
                var room = new Room();
                room.bounds = boundsInt;
                room.center = floor.WorldToCell(room.bounds.center);
                allRoomsInCurrentMap.Add(room);
            }
        }

        private List<BoundsInt> ExcludeStartRoom()
        {
            var spawnableRooms = rooms;
            spawnableRooms.Remove(startRoom);

            return spawnableRooms;
        }

        public void ResetSpawnPositions()
        {
            // Reset All SpawnPositions if any
            spawnPositions = new HashSet<Vector2Int>();
        }

        public void SpawnObjects()
        {
            var spawnRooms = ExcludeStartRoom();
            var floorPositions = new HashSet<Vector2Int>();

            foreach (var room in spawnRooms) floorPositions.UnionWith(GetHashSetFromBoundsInt(room));

            var spawnAmount = floorPositions.Count / spawnRate;

            while (spawnPositions.Count < spawnAmount)
            {
                var index = Random.Range(0, floorPositions.Count);
                spawnPositions.Add(floorPositions.ElementAt(index));
            }

            foreach (var position in spawnPositions)
            {
                var spawn = spawnTable.GetSpawn();
                SpawnSingleObject(spawn, position);
            }

            Debug.Log("<color=green>RoomManager:</color> Populating Dungeon complete.");
        }

        private void RemoveSpawnPosition(Vector3 position)
        {
            var spawnPosition = (Vector2Int) floor.WorldToCell(position);

            if (spawnPositions.Contains(spawnPosition))
            {
                spawnPositions.Remove(spawnPosition);
                Debug.Log($"<color=green>RoomManager:</color> Spawnposition {spawnPosition} cleared!");
            }
            else
            {
                Debug.Log(
                    $"<color=green>RoomManager:</color> Spawnposition World:{spawnPosition} / Map{position} not found!");
            }
        }

        private void SpawnLootChest(Vector3 position, Item chestContents)
        {
            var spawnPosition = (Vector2Int) floor.WorldToCell(position);
            var newLootChest = SpawnSingleObject(lootChestPrefab, spawnPosition);

            newLootChest.GetComponent<Chest>().SetChestContents(chestContents);

            // Set LootChest as Player Focus
            var interactable = newLootChest.GetComponent<IInteractable>();
            if (interactable != null) setLootFocus?.Invoke(interactable);

            Debug.Log($"<color=green>RoomManager:</color> Spawned Loot Chest at {spawnPosition}");
        }

        private void SpawnExit()
        {
            var exitPosition = (Vector2Int) GetRandomPositionInBoundsInt(bossRoom);
            SpawnSingleObject(exitPrefab, exitPosition);

            Debug.Log($"<color=green>RoomManager:</color> Spawned Exit at {exitPosition}");
        }

        public GameObject SpawnSingleObject(GameObject spawn, Vector2Int position)
        {
            var spawnPosition = floor.GetCellCenterWorld((Vector3Int) position);
            spawnPositions.Add(position);

            var newSpawn = Instantiate(spawn, spawnPosition, Quaternion.identity, activeEntities);

            return newSpawn;
        }

        private Vector3Int GetRandomPositionInBoundsInt(BoundsInt room)
        {
            return new Vector3Int
            (
                Random.Range(room.min.x + offset, room.max.x - offset),
                Random.Range(room.min.y + offset, room.max.y - offset),
                0
            );
        }

        private HashSet<Vector2Int> GetHashSetFromBoundsInt(BoundsInt boundsInt)
        {
            var uniqueSpawnPositions = new HashSet<Vector2Int>();

            for (var col = offset; col < boundsInt.size.x - offset; col++)
            for (var row = offset; row < boundsInt.size.y - offset; row++)
            {
                var position = (Vector2Int) boundsInt.min + new Vector2Int(col, row);
                uniqueSpawnPositions.Add(position);
            }

            return uniqueSpawnPositions;
        }

        private BoundsInt DetermineStartRoom()
        {
            if (rooms == null)
            {
                Debug.Log("Room List empty!");
                return new BoundsInt(Vector3Int.zero, Vector3Int.zero);
            }

            return rooms[Random.Range(0, rooms.Count)];
        }

        private BoundsInt DetermineBossRoom()
        {
            float currentDistance = 0;
            var farthestRoomCenter = new BoundsInt();

            foreach (var room in rooms)
            {
                var distance = Vector2.Distance(startRoom.center, room.center);
                if (distance > currentDistance)
                {
                    currentDistance = distance;
                    farthestRoomCenter = room;
                }
            }

            return farthestRoomCenter;
        }

        #region Field Declarations

        [Range(1, 100)] [SerializeField] private int spawnRate = 10;

        private readonly int offset = 2;
        private List<BoundsInt> rooms;

        [SerializeField] private SpawnTable spawnTable;
        [SerializeField] private Transform activeEntities;

        [SerializeField] private GameObject doorPrefab;
        [SerializeField] private GameObject lootChestPrefab;
        [SerializeField] private GameObject exitPrefab;

        [SerializeField] private BoundsInt startRoom;
        public BoundsInt StartRoom { get; set; }

        private Tilemap floor;

        public HashSet<Vector2Int> spawnPositions { get; set; }

        [SerializeField] private BoundsInt bossRoom;
        private List<Room> currentSetOfRooms;


        #endregion
    }
}