using System.Collections.Generic;
using Hardcode.ITFOD.Game;
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
    public static class PCGAlgorithms
    {
        public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
        {
            var path = new HashSet<Vector2Int>();

            path.Add(startPosition);
            var previousPosition = startPosition;

            for (var i = 0; i < walkLength; i++)
            {
                var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
                path.Add(newPosition);
                previousPosition = newPosition;
            }

            return path;
        }

        public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
        {
            //Select a Direction and walk dude!
            var corridor = new List<Vector2Int>();
            var direction = Direction2D.GetRandomCardinalDirection();
            var currentPosition = startPosition;

            //Add startposition
            corridor.Add(currentPosition);

            for (var i = 0; i < corridorLength; i++)
            {
                currentPosition += direction;
                corridor.Add(currentPosition);
            }

            return corridor;
        }
        
        public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
        {
            var roomsQueue = new Queue<BoundsInt>();
            var roomsList = new List<BoundsInt>();

            roomsQueue.Enqueue(spaceToSplit);

            while (roomsQueue.Count > 0)
            {
                var roomBoundsInt = roomsQueue.Dequeue();
                if (roomBoundsInt.size.y >= minHeight && roomBoundsInt.size.x >= minWidth)
                {
                    if (Random.value < 0.5f)
                    {
                        if (roomBoundsInt.size.y >= minHeight * 2)
                            SplitHorizontally(minHeight, roomsQueue, roomBoundsInt);
                        else if (roomBoundsInt.size.x >= minWidth * 2)
                            SplitVertically(minWidth, roomsQueue, roomBoundsInt);
                        else if (roomBoundsInt.size.x >= minWidth && roomBoundsInt.size.y >= minHeight)
                        {
                            roomsList.Add(roomBoundsInt);
                        }
                    }
                    else
                    {
                        if (roomBoundsInt.size.x >= minWidth * 2)
                            SplitVertically(minWidth, roomsQueue, roomBoundsInt);
                        else if (roomBoundsInt.size.y >= minHeight * 2)
                            SplitHorizontally(minHeight, roomsQueue, roomBoundsInt);
                        else if (roomBoundsInt.size.x >= minWidth && roomBoundsInt.size.y >= minHeight) roomsList.Add(roomBoundsInt);
                    }
                }
            }

            return roomsList;
        }

        private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
        {
            var xSplit = Random.Range(1, room.size.x);
            var room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
            var room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
                new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }

        private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
        {
            var ySplit = Random.Range(1, room.size.y);
            var room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
            var room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }

        #region Field Declarations

        #endregion
    }
}