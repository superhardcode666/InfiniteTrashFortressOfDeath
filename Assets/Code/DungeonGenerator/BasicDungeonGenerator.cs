using System.Collections.Generic;
using UnityEngine;

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
namespace Hardcode.ITFOD.Procedural
{
    public class BasicDungeonGenerator : RandomWalk
    {
        public List<BoundsInt> GenerateRoomBounds()
        {
            RunProceduralGeneration();
            return roomsList;
        }

        protected override void RunProceduralGeneration()
        {
            tilemapGenerator.Clear();
            CreateRooms();
        }

        private void CreateRooms()
        {
            roomsList = PCGAlgorithms.BinarySpacePartitioning(
                new BoundsInt((Vector3Int) startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth,
                minRoomHeight);

            var floor = new HashSet<Vector2Int>();

            if (generateCaveRooms)
                floor = CreateRoomsRandomly(roomsList);
            else
                floor = CreateSimpleRooms(roomsList);


            var roomCenters = new List<Vector2Int>();
            foreach (var room in roomsList) roomCenters.Add((Vector2Int) Vector3Int.RoundToInt(room.center));

            var corridors = ConnectRooms(roomCenters);
            floor.UnionWith(corridors);

            // Main Dungeon
            tilemapGenerator.DrawFloorTiles(floor);
            WallGenerator.CreateWalls(floor, tilemapGenerator);

            // Mini Map Display
            tilemapGenerator.DrawMiniMapFloorTiles(floor);
            tilemapGenerator.DrawMiniMapWallTiles(WallGenerator.GetWallPositions());

            floor.UnionWith(WallGenerator.GetWallPositions());
            fogMap = floor;

            tilemapGenerator.DrawFog(fogMap);
        }

        private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
        {
            var floor = new HashSet<Vector2Int>();
            for (var i = 0; i < roomsList.Count; i++)
            {
                var roomBounds = roomsList[i];
                var roomCenter =
                    new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
                var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
                foreach (var position in roomFloor)
                    if (position.x >= roomBounds.xMin + offset && position.x <= roomBounds.xMax - offset &&
                        position.y >= roomBounds.yMin - offset && position.y <= roomBounds.yMax - offset)
                        floor.Add(position);
            }

            return floor;
        }

        private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
        {
            var corridors = new HashSet<Vector2Int>();
            var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
            roomCenters.Remove(currentRoomCenter);

            while (roomCenters.Count > 0)
            {
                var closest = FindClosestPointTo(currentRoomCenter, roomCenters);
                roomCenters.Remove(closest);
                var newCorridor = CreateCorridor(currentRoomCenter, closest);
                currentRoomCenter = closest;
                corridors.UnionWith(newCorridor);
            }

            return corridors;
        }

        private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
        {
            var corridor = new HashSet<Vector2Int>();
            var doorsList = new List<Vector2Int>();

            var position = currentRoomCenter;
            corridor.Add(position);

            while (position.y != destination.y)
            {
                if (destination.y > position.y)
                    position += Vector2Int.up;
                else if (destination.y < position.y) position += Vector2Int.down;
                corridor.Add(position);
            }

            while (position.x != destination.x)
            {
                if (destination.x > position.x)
                    position += Vector2Int.right;
                else if (destination.x < position.x) position += Vector2Int.left;
                corridor.Add(position);
            }

            return corridor;
        }

        private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
        {
            var closest = Vector2Int.zero;
            var distance = float.MaxValue;
            foreach (var position in roomCenters)
            {
                var currentDistance = Vector2.Distance(position, currentRoomCenter);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closest = position;
                }
            }

            return closest;
        }

        private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
        {
            var floor = new HashSet<Vector2Int>();

            foreach (var room in roomsList)
                for (var col = offset; col < room.size.x - offset; col++)
                for (var row = offset; row < room.size.y - offset; row++)
                {
                    var position = (Vector2Int) room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }

            return floor;
        }

        #region Field Declarations

        [Header("Dungeon Dimensions")] [SerializeField] [Range(20, 100)]
        private int dungeonWidth;

        [SerializeField] [Range(20, 100)] private int dungeonHeight;

        [Header("Room Dimensions")] [SerializeField] [Range(4, 100)]
        private int minRoomWidth;

        [SerializeField] [Range(4, 100)] private int minRoomHeight;

        [Header("Room Offset")] [SerializeField] [Range(2, 10)]
        private int offset;

        private List<BoundsInt> roomsList;
        public List<BoundsInt> RoomsList { get; set; }

        private HashSet<Vector2Int> doorPositions;
        private HashSet<Vector2Int> fogMap;

        private readonly bool generateCaveRooms = false;

        #endregion
    }
}