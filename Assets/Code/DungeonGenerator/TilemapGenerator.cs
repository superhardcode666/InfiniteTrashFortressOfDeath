using System;
using System.Collections.Generic;
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
namespace Hardcode.ITFOD.Procedural
{
    public class TilemapGenerator : MonoBehaviour
    {
        public void DrawFloorTiles(IEnumerable<Vector2Int> floorPositions)
        {
            DrawRandomFloorTiles(floorPositions, floorTilemap, floorTiles);
        }

        public void DrawMiniMapFloorTiles(IEnumerable<Vector2Int> floorPositions)
        {
            DrawTiles(floorPositions, miniMapFloorTilemap, miniMapFloorTile);
        }

        public void DrawMiniMapWallTiles(IEnumerable<Vector2Int> wallPositions)
        {
            DrawTiles(wallPositions, miniMapWallTilemap, miniMapWallTile);
        }

        public void DrawTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
        {
            foreach (var position in positions) DrawSingleTile(tilemap, tile, position);
        }

        public void DrawRandomFloorTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase[] tiles)
        {
            foreach (var position in positions)
            {
                var randomFloorTile = floorTiles[Random.Range(0, floorTiles.Length)];
                DrawSingleTile(tilemap, randomFloorTile, position);
            }
        }

        public void DrawFog(IEnumerable<Vector2Int> fogPositions)
        {
            DrawTiles(fogPositions, fogTilemap, fogTile);
        }

        public void DrawSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
        {
            var tilePosition = tilemap.WorldToCell((Vector3Int) position);
            tilemap.SetTile(tilePosition, tile);
        }

        public void DrawSingleBasicWall(Vector2Int position, string binaryType)
        {
            var typeAsInt = Convert.ToInt32(binaryType, 2);
            TileBase tile = null;
            if (WallTypesHelper.wallTop.Contains(typeAsInt))
                tile = wallTop;
            else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
                tile = wallSideRight;
            else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
                tile = wallSideLeft;
            else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
                tile = wallBottom;
            else if (WallTypesHelper.wallFull.Contains(typeAsInt)) tile = wallFull;

            if (tile != null)
                DrawSingleTile(wallTilemap, tile, position);
        }

        public void Clear()
        {
            floorTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();

            fogTilemap.ClearAllTiles();

            miniMapWallTilemap.ClearAllTiles();
            miniMapFloorTilemap.ClearAllTiles();
        }

        public void DrawSingleCornerWall(Vector2Int position, string binaryType)
        {
            var typeASInt = Convert.ToInt32(binaryType, 2);
            TileBase tile = null;

            if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
                tile = wallInnerCornerDownLeft;
            else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
                tile = wallInnerCornerDownRight;
            else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
                tile = wallDiagonalCornerDownLeft;
            else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
                tile = wallDiagonalCornerDownRight;
            else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
                tile = wallDiagonalCornerUpRight;
            else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
                tile = wallDiagonalCornerUpLeft;
            else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt))
                tile = wallFull;
            else if (WallTypesHelper.wallBottomEightDirections.Contains(typeASInt)) tile = wallBottom;

            if (tile != null)
                DrawSingleTile(wallTilemap, tile, position);
        }

        #region Field Declarations

        [SerializeField] private Tilemap floorTilemap, wallTilemap, fogTilemap, miniMapWallTilemap, miniMapFloorTilemap;

        //Turn this into an Array later to Select random floor Tiles
        [SerializeField] private TileBase floorTile;
        [SerializeField] private TileBase[] floorTiles;

        [SerializeField] private TileBase wallTop,
            wallSideRight,
            wallSideLeft,
            wallBottom,
            wallFull,
            wallInnerCornerDownLeft,
            wallInnerCornerDownRight,
            wallDiagonalCornerDownRight,
            wallDiagonalCornerDownLeft,
            wallDiagonalCornerUpRight,
            wallDiagonalCornerUpLeft;

        [SerializeField] private TileBase fogTile;

        [SerializeField] private TileBase miniMapWallTile;
        [SerializeField] private TileBase miniMapFloorTile;

        #endregion
    }
}