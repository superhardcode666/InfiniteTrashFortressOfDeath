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
    public class RandomWalk : AbstractDungeonGenerator
    {
        #region Field Declarations

        protected RandomWalkParamterObject randomWalkParameters;

        #endregion

        protected override void RunProceduralGeneration()
        {
            var floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
            tilemapGenerator.Clear();
            tilemapGenerator.DrawFloorTiles(floorPositions);
            WallGenerator.CreateWalls(floorPositions, tilemapGenerator);
        }

        protected HashSet<Vector2Int> RunRandomWalk(RandomWalkParamterObject parameters, Vector2Int position)
        {
            var currentPosition = position;
            var floorPositions = new HashSet<Vector2Int>();

            for (var i = 0; i < parameters.iterations; i++)
            {
                var path = PCGAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
                floorPositions.UnionWith(path);
                if (parameters.startRandomlyEachIteration)
                    currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }

            return floorPositions;
        }
    }
}