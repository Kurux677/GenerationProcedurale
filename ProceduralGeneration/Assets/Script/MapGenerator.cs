using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
    public enum GenerationType
    {
        RANDOM,PERLINNOISE
    }

    public GenerationType generationType;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;
    public Tilemap tilemap;

    public TerrainType[] regions;
    public TerrainType[] minerals;

    public void GenerateMap()
    {
        if(generationType==GenerationType.PERLINNOISE)
        {
            GenerateMapWithNoise();
        }
        else if(generationType==GenerationType.RANDOM)
        {
            GenerateMapWithRandom();
        }

    }

    public void GenerateMapWithRandom()
    {
        TileBase[] customTilemap = new TileBase[mapWidth*mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = UnityEngine.Random.Range(0f, 1f);
                customTilemap[y * mapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        SetTileMap(customTilemap);
    }

    private void SetTileMap(TileBase[] customTilemap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTilemap[y * mapWidth + x]);
            }
        }
    }

    private TileBase FindTileFromRegion(float rnd)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if(rnd<=regions[i].height)
            {
                return regions[i].tile;
            }
        }
        return regions[0].tile;
    }

    private TileBase FindTileFromMinerals(float rnd)
    {
        for (int i = 0; i < minerals.Length; i++)
        {
            if (rnd <= minerals[i].height)
            {
                return minerals[i].tile;
            }
        }
        return minerals[0].tile;
    }

    public void GenerateMapWithNoise()
    {
        float[,] noiseMapGround = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapGisement= Noise.GenerateNoiseMap(mapWidth, mapHeight, seed * 2, noiseScale, octaves * 2, persistance * 6, lacunarity, offset);
        float[,] noiseMapCavern= Noise.GenerateNoiseMap(mapWidth, mapHeight, seed * 2, noiseScale, octaves * 2, persistance * 6, lacunarity, offset);
        float[,] noiseMapMineral = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed*3, noiseScale, octaves*2, persistance*6, lacunarity, offset);
        TileBase[] customTilemap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = noiseMapGround[x, y];
                float rndGisement = noiseMapGisement[x, y];
                float rndCavern = noiseMapCavern[x, y];
                if (rndGisement > 0.7)
                {
                    rnd = noiseMapMineral[x, y];
                    customTilemap[y * mapWidth + x] = FindTileFromMinerals(rnd);
                }
                else if(rndCavern <0.4)
                {
                    customTilemap[y * mapWidth + x] = minerals[3].tile;
                }
                else
                {
                    customTilemap[y * mapWidth + x] = FindTileFromRegion(rnd);
                }
                
            }
        }
        SetTileMap(customTilemap);
    }

    private void OnValidate()
    {
        if(mapHeight<1)
        {
            mapHeight = 1;
        }
        if(mapWidth<1)
        {
            mapWidth = 1;
        }
        if(lacunarity<1)
        {
            lacunarity = 1;
        }
        if(octaves<1)
        {
            octaves = 1;
        }
    }
}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public TileBase tile;
}
