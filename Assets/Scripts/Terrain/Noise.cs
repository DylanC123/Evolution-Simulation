using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float[,] noiseGen(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        for (int y = 0; y < height; y ++)
        {
            for (int x = 0; x < width; x ++)
            {
                float perlinNoise = Mathf.PerlinNoise(x/scale, y/scale);

                noiseMap[x, y] = perlinNoise;
            }
        }

        return noiseMap;
    }
}

