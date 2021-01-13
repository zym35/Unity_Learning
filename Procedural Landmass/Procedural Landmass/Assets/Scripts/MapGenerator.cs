using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColorMap, Mesh};
    public DrawMode drawMode;

    //for LOD convenience: factors of 1, 2, 4, 8, 10, 12
    [Range(0, 6)]
    public int LOD;
    const int mapChunkSize = 241;
    public float heightMultiplier = 1;
    public float noiseScale;
    public AnimationCurve heightCurve;

    [Range(1, 10)]  
    public int octaves;
    [Range(0, 1)]  
    public float persistence;
    [Range(1, 20)] 
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public bool autoUpdate;
    public TerrainType[] regions;


    public void GenerateMap() 
    {
        float [,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) 
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++) 
                {
                    if (currentHeight <= regions[i].height) 
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColorMap)
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        else if (drawMode == DrawMode.Mesh)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve, LOD), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
    }


    /// Called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    void OnValidate()
    {
        
    }
}

[System.Serializable]
public struct TerrainType 
{
    public string type;
    public float height;
    public Color color;
}