using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public MapRenderer mapRenderer;
    public TileBase grassTile, waterTile, hillTile, snowTile, maxPosTile;
    public int mapSize;
    public NoiseSettings mapSettings;
    public float hillHeight = 0.5f, snowHeight = 0.6f, waterHeight = 0.4f;
    public float[,] noiseMap;

    private void Start()
    {
        noiseMap = new float[mapSize, mapSize];
        PrepareMap();
    }

    public void PrepareMap()
    {
        Debug.Log("Generating map");
        mapRenderer.ClearMap();
        
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                var noise = NoiseHelper.SumNoise(x, y, mapSettings);
                noiseMap[x, y] = noise;
                if (noise > snowHeight)
                {
                    mapRenderer.SetTileTo(x, y, snowTile);
                }else if(noise > hillHeight)
                {
                    mapRenderer.SetTileTo(x, y, hillTile);
                }else if (noise < waterHeight)
                {
                    mapRenderer.SetTileTo(x, y, waterTile);
                }
                else
                {
                    mapRenderer.SetTileTo(x, y, grassTile);
                }

            }
        }
    }

    public void ShowMaximas()
    {
        var result = NoiseHelper.FindLocalMaxima(noiseMap);
        result = result.Where(pos => noiseMap[pos.x, pos.y] > snowHeight).OrderBy(pos => noiseMap[pos.x, pos.y]).Take(20).ToList();
        foreach (var item in result)
        {
            mapRenderer.SetTileTo(item.x, item.y, maxPosTile);
        }
    }

    public void ShowMinimas()
    {
        var result = NoiseHelper.FindLocalMinima(noiseMap);
        result = result.Where(pos => noiseMap[pos.x, pos.y] < waterHeight).OrderBy(pos => noiseMap[pos.x, pos.y]).Take(20).ToList();
        foreach (var item in result)
        {
            mapRenderer.SetTileTo(item.x, item.y, maxPosTile);
        }
    }

}
