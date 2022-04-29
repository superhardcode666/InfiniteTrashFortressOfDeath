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
public class FogOfWar : MonoBehaviour
{
    private void RefreshTileMap()
    {
        fogMap.RefreshAllTiles();
    }

    private bool IsInsideCircle(Vector3 center, Vector3 tilePosition, float radius)
    {
        float dx = center.x - tilePosition.x,
            dy = center.y - tilePosition.y;
        var distanceSquared = dx * dx + dy * dy;
        return distanceSquared <= radius * radius;
    }

    private void ClearTilesInCircle(Bounds areaToClear)
    {
        int top = Mathf.FloorToInt(areaToClear.center.y - fovRadius),
            bottom = Mathf.CeilToInt(areaToClear.center.y + fovRadius),
            left = Mathf.FloorToInt(areaToClear.center.x - fovRadius),
            right = Mathf.CeilToInt(areaToClear.center.x + fovRadius);

        for (var y = top; y <= bottom; y++)
        for (var x = left; x <= right; x++)
        {
            var tilePos = new Vector3Int(x, y, 0);
            if (IsInsideCircle(areaToClear.center, tilePos, fovRadius))
                if (fogMap.HasTile(tilePos))
                {
                    fogMap.SetTile(tilePos, null);
                    fogMap.RefreshTile(tilePos);
                }
        }
    }

    private bool isEmpty(List<Vector3Int> listOfTiles)
    {
        if (listOfTiles == null) return true;

        return listOfTiles.Count == 0;
    }

    private List<Vector3Int> GetTilesInCircleArea(Bounds areaToFade)
    {
        var tiles = new List<Vector3Int>();

        int top = Mathf.FloorToInt(areaToFade.center.y - fovRadius),
            bottom = Mathf.CeilToInt(areaToFade.center.y + fovRadius),
            left = Mathf.FloorToInt(areaToFade.center.x - fovRadius),
            right = Mathf.CeilToInt(areaToFade.center.x + fovRadius);

        for (var y = top; y <= bottom; y++)
        for (var x = left; x <= right; x++)
        {
            var tilePos = new Vector3Int(x, y, 0);
            if (IsInsideCircle(areaToFade.center, tilePos, fovRadius))
                if (fogMap.HasTile(tilePos))
                    tiles.Add(tilePos);
        }

        return tiles;
    }

    public void FadeFogOfWarArea(Vector3 playerPosition)
    {
        if (isFading) return;

        var areaWorldCenter = playerPosition;
        var areaCellCenter = fogMap.WorldToCell(areaWorldCenter);

        var areaToClear = new Bounds(areaCellCenter, new Vector3(fovRadius * 2, fovRadius * 2));

        tilesToFade = GetTilesInCircleArea(areaToClear);

        if (!isEmpty(tilesToFade))
        {
            totalDeltaTime = 0f;
            isFading = true;
        }
    }

    public void OnCreated()
    {
        FadeFogOfWarArea(playerPosition.position);
    }

    public void OnUpdate()
    {
        if (isFading && !isEmpty(tilesToFade))
        {
            totalDeltaTime += Time.deltaTime;
            var step = totalDeltaTime / fadeSpeed;

            currentColor = Color.Lerp(Color.black, Color.clear, step);

            foreach (var position in tilesToFade)
            {
                fogMap.SetColor(position, currentColor);
                fogMap.RefreshTile(position);
            }

            if (step >= 1.0f)
            {
                foreach (var position in tilesToFade)
                {
                    fogMap.SetTile(position, null);
                    fogMap.RefreshTile(position);
                }

                tilesToFade.Clear();
                isFading = false;
            }
        }
        else if (!isFading && isEmpty(tilesToFade))
        {
            FadeFogOfWarArea(playerPosition.position);
        }
    }

    public void ClearFogOfWarArea(Vector3 originPosition)
    {
        var areaWorldCenter = originPosition;
        var areaCellCenter = fogMap.WorldToCell(areaWorldCenter);

        var areaToClear = new Bounds(areaCellCenter, new Vector3(fovRadius * 2, fovRadius * 2));

        ClearTilesInCircle(areaToClear);
        RefreshTileMap();
    }

    #region Field Declarations

    [SerializeField] public Tilemap fogMap;
    [SerializeField] private float fovRadius = 2.5f;

    [SerializeField] private Transform playerPosition;

    [SerializeField] private float fadeSpeed = 0.5f;
    private float totalDeltaTime;
    [SerializeField] private bool isFading;
    private Color currentColor;
    [SerializeField] private List<Vector3Int> tilesToFade;

    #endregion
}