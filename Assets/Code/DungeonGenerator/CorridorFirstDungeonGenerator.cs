using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CorridorFirstDungeonGenerator : RandomWalk
    {
        protected override void RunProceduralGeneration()
        {
            CorridorFirstGeneration();
        }

        private void CorridorFirstGeneration()
        {
            var floorPositions = new HashSet<Vector2Int>();
            var potentialRoomPositions = new HashSet<Vector2Int>();

            CreateCorridors(floorPositions, potentialRoomPositions);

            var roomPositions = CreateRooms(potentialRoomPositions);
            var deadEnds = FindAllDeadEnds(floorPositions);

            if (!mayContainDeadEnds)
                //Always create Room at Deadend even if we exceed roompercentage
                CreateRoomsAtDeadEnd(deadEnds, roomPositions);

            floorPositions.UnionWith(roomPositions);

            tilemapGenerator.DrawFloorTiles(floorPositions);
            WallGenerator.CreateWalls(floorPositions, tilemapGenerator);
        }

        private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
        {
            foreach (var position in deadEnds)
                if (!roomFloors.Contains(position))
                {
                    var room = RunRandomWalk(randomWalkParameters, position);
                    roomFloors.UnionWith(room);
                }
        }

        private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
        {
            var deadEnds = new List<Vector2Int>();
            foreach (var position in floorPositions)
            {
                var neighborsCount = 0;
                // if we have a neighbor in only one direction, we know we have a deadend
                foreach (var direction in Direction2D.cardinalDirectionsList)
                    if (floorPositions.Contains(position + direction))
                        neighborsCount++;

                if (neighborsCount == 1) deadEnds.Add(position);
            }

            return deadEnds;
        }

        private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
        {
            //as usual hashset to avoid duplicates
            var roomPositions = new HashSet<Vector2Int>();

            //calculate number of rooms to create via roomPercentage
            var roomsToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercentage);

            //randomly sort room positions by guid and take as many entries as roompercentage dictates
            var roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsToCreateCount).ToList();

            foreach (var roomPosition in roomsToCreate)
            {
                var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
                roomPositions.UnionWith(roomFloor);
            }

            return roomPositions;
        }

        private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
        {
            var currentPosition = startPosition;
            potentialRoomPositions.Add(currentPosition);

            for (var i = 0; i < corridorCount; i++)
            {
                var corridor = PCGAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
                // Set current post to last position visited to ensure corridors are connected
                currentPosition = corridor[corridor.Count - 1];
                potentialRoomPositions.Add(currentPosition);
                floorPositions.UnionWith(corridor);
            }
        }

        #region Field Declarations

        [SerializeField] private int corridorLength = 14;
        [SerializeField] private int corridorCount = 5;

        [Range(0.1f, 1f)] [SerializeField] private float roomPercentage = 0.8f;
        [SerializeField] private bool mayContainDeadEnds;

        #endregion
    }
}