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
    public static class Direction2D
    {
        public static List<Vector2Int> cardinalDirectionsList = new()
        {
            new Vector2Int(0, 1), //UP
            new Vector2Int(1, 0), //RIGHT
            new Vector2Int(0, -1), //DOWN
            new Vector2Int(-1, 0) //LEFT
        };

        public static List<Vector2Int> diagonalDirectionsList = new()
        {
            new Vector2Int(1, 1), //UP RIGHT
            new Vector2Int(1, -1), //DOWN RIGHT
            new Vector2Int(-1, -1), //DOWN LEFT
            new Vector2Int(-1, 1) //UP LEFT
        };

        public static List<Vector2Int> eightDirectionsList = new()
        {
            new(0, 1), //UP
            new(1, 1), //UP RIGHT
            new(1, 0), //RIGHT
            new(1, -1), //DOWN RIGHT
            new(0, -1), //DOWN
            new(-1, -1), //DOWN LEFT
            new(-1, 0), //LEFT
            new(-1, 1) //UP LEFT
        };

        public static Vector2Int GetRandomCardinalDirection()
        {
            return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
        }
    }
}