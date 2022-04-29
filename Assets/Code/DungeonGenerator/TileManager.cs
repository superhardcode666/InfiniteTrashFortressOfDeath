using System.Collections.Generic;
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

public class TileManager : MonoBehaviour
{
    #region Field Declarations

    public static TileManager instance;
    [SerializeField] private Tilemap tilemap;

    public Dictionary<Vector3Int, GameTile> tiles;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);

        InitTiles();
    }

    private void Start()
    {
        Exit.triggerMapChange += InitTiles;
    }

    private void OnDestroy()
    {
        Exit.triggerMapChange -= InitTiles;
    }

    private void InitTiles()
    {
        tiles = new Dictionary<Vector3Int, GameTile>();
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var gridPosition = new Vector3Int(position.x, position.y, position.z);

            if (!tilemap.HasTile(gridPosition)) continue;

            var tile = new GameTile
            {
                gridPosition = gridPosition,
                tilebase = tilemap.GetTile(gridPosition),
                parentTilemap = tilemap,
                isTrapped = false,
                isExplored = false,
                occupant = null
            };

            tiles.Add(tile.gridPosition, tile);
        }

        Debug.Log("<color=white>Extended Tile System:</color> Extended Tiles generated successfully.");
    }

    #endregion
}