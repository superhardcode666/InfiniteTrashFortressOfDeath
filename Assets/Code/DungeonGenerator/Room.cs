using System;
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
namespace Hardcode.ITFOD.Game
{
    [Serializable]
    public class Room
    {
        // basic collection of a Rooms Information
        public BoundsInt bounds;
        public Vector3Int center;
        public RoomType type;

        public void SpawnObjects()
        {
            
        }
    }

    public enum RoomType
    {
        StartRoom,
        HostileRoom,
        TrapRoom,
        BossRoom,
        ShopRoom
    }
}