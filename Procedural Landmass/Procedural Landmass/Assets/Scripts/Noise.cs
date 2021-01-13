using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public static float [,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale,
        int octaves, float persistence, float lacunarity, Vector2 offset) {

        //clamp
        if (scale <= 0) {
            scale = 0.0001f;
        }

        //pseudo-random number generator
        System.Random prng = new System.Random(seed);
        //handle seed and custom offset
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 10000) + offset.x;
            float offsetY = prng.Next(-100000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float [,] noiseMap = new float[mapWidth, mapHeight];

        //for normalize
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //scaling from the middle
        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){

                //amplitude from 0~1, smaller as larger octaves, determined by persistence
                float amplitude = 1;
                //frequency above 1, determined by lacunarity
                float frequency = 1;
                float noiseheight = 0;

                for (int i = 0; i < octaves; i++) {
                    //not integer for avoiding same values
                    //higher the frequency, further apart the sample points, the height values change more rapidly
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    //can generate negative perlin values (-1~1), for decreasing heights
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseheight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                //keep track of max and min noise height
                if (noiseheight > maxNoiseHeight)
                    maxNoiseHeight = noiseheight;
                else if (noiseheight < minNoiseHeight)
                    minNoiseHeight = noiseheight;

                noiseMap[x, y] = noiseheight;
            }
        }

        //normalize
        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
        }

        return noiseMap;
    }
}