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

public class GameTile
{
    #region Field Declarations

    public Vector3Int gridPosition { get; set; }
    public TileBase tilebase { get; set; }
    public Tilemap parentTilemap { get; set; }
    public bool isTrapped { get; set; }
    public bool isExplored { get; set; }
    public GameObject occupant { get; set; }

    #endregion
}