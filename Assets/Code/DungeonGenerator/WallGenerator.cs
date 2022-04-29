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
    public static class WallGenerator
    {
        private static HashSet<Vector2Int> wallPositions = new();

        public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapGenerator tilemapGenerator)
        {
            // Walls in Cardinal directions
            var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
            var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionsList);
            CreateBasicWalls(tilemapGenerator, basicWallPositions, floorPositions);
            CreateCornerWalls(tilemapGenerator, cornerWallPositions, floorPositions);

            basicWallPositions.UnionWith(cornerWallPositions);
            wallPositions = basicWallPositions;
        }

        public static HashSet<Vector2Int> GetWallPositions()
        {
            return wallPositions;
        }

        private static void CreateCornerWalls(TilemapGenerator tilemapGenerator,
            HashSet<Vector2Int> cornerWallPositions,
            HashSet<Vector2Int> floorPositions)
        {
            foreach (var position in cornerWallPositions)
            {
                var neighboursBinaryType = "";
                foreach (var direction in Direction2D.eightDirectionsList)
                {
                    var neighbourPosition = position + direction;
                    if (floorPositions.Contains(neighbourPosition))
                        neighboursBinaryType += "1";
                    else
                        neighboursBinaryType += "0";
                }

                tilemapGenerator.DrawSingleCornerWall(position, neighboursBinaryType);

                //Draw MiniMap Wall
            }
        }

        public static void CreateBasicWalls(TilemapGenerator tilemapGenerator, HashSet<Vector2Int> basicWallPositions,
            HashSet<Vector2Int> floorPositions)
        {
            foreach (var position in basicWallPositions)
            {
                var neighboursBinaryType = "";
                foreach (var direction in Direction2D.cardinalDirectionsList)
                {
                    var neighbourPosition = position + direction;
                    if (floorPositions.Contains(neighbourPosition))
                        neighboursBinaryType += "1";
                    else
                        neighboursBinaryType += "0";
                }

                tilemapGenerator.DrawSingleBasicWall(position, neighboursBinaryType);
            }
        }

        private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions,
            List<Vector2Int> directionList)
        {
            var wallPositions = new HashSet<Vector2Int>();
            foreach (var position in floorPositions)
                // check for wallpositions in cardinal directions
            foreach (var direction in directionList)
            {
                var neighborPosition = position + direction;
                // if the neighborpos is not a floorposition, it must be a wall
                if (!floorPositions.Contains(neighborPosition)) wallPositions.Add(neighborPosition);
            }

            return wallPositions;
        }
    }
}