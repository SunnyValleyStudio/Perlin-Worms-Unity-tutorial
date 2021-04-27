using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public NoiseSettings riverSettings;
    public Vector2 riverStartPosition;
    public int riverLength = 50;
    public bool bold = true;
    public bool converganceOn = true;

    public void GenerateRivers()
    {
        var result = NoiseHelper.FindLocalMaxima(mapGenerator.noiseMap);
        var toCreate = result.Where(pos => mapGenerator.noiseMap[pos.x, pos.y] > mapGenerator.snowHeight).OrderBy(a => Guid.NewGuid()).Take(5).ToList();
        var waterMinimas = NoiseHelper.FindLocalMinima(mapGenerator.noiseMap);
        waterMinimas = waterMinimas.Where(pos => mapGenerator.noiseMap[pos.x, pos.y] < mapGenerator.waterHeight).OrderBy(pos => mapGenerator.noiseMap[pos.x, pos.y]).Take(20).ToList();
        foreach (var item in toCreate)
        {
            //SetTileTo(item.x, item.y, maxPosTile);
            CreateRiver(item, waterMinimas);
            //return;
        }
    }

    private void CreateRiver(Vector2Int startPosition, List<Vector2Int> waterMinimas)
    {
        PerlinWorm worm;
        if (converganceOn)
        {
            var closestWaterPos = waterMinimas.OrderBy(pos => Vector2.Distance(pos, startPosition)).First();
            worm = new PerlinWorm(riverSettings, startPosition, closestWaterPos);
        }
        else
        {
            worm = new PerlinWorm(riverSettings, startPosition);
        }

        var position = worm.MoveLength(riverLength);
        StartCoroutine(PlaceRiverTile(position));
    }

    IEnumerator PlaceRiverTile(List<Vector2> positons)
    {
        foreach (var pos in positons)
        {
            var tilePos = mapGenerator.mapRenderer.GetCellposition(pos);
            if (tilePos.x < 0 || tilePos.x >= mapGenerator.mapSize || tilePos.y < 0 || tilePos.y >= mapGenerator.mapSize)
                break;
            mapGenerator.mapRenderer.SetTileTo(tilePos, mapGenerator.waterTile);
            if (bold && mapGenerator.noiseMap[tilePos.x, tilePos.y] < mapGenerator.hillHeight)
            {
                mapGenerator.mapRenderer.SetTileTo(tilePos + Vector3Int.right, mapGenerator.waterTile);
                mapGenerator.mapRenderer.SetTileTo(tilePos + Vector3Int.left, mapGenerator.waterTile);
                mapGenerator.mapRenderer.SetTileTo(tilePos + Vector3Int.up, mapGenerator.waterTile);
                mapGenerator.mapRenderer.SetTileTo(tilePos + Vector3Int.down, mapGenerator.waterTile);
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
}
