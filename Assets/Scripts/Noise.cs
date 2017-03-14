﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		float [,] noiseMap = new float[mapWidth,mapHeight];

		System.Random random = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i <octaves; i++) {	
			float offsetX = random.Next(-100000, 100000) + offset.x;
			float offsetY = random.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if(scale <= 0)
			scale = 0.0001f;

			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;
			float halfWidth = mapWidth / 2f;
			float halfHeight = mapHeight / 2f;
	
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for(int i = 0; i < octaves; i++){
					float scaleX = (x-halfWidth)/scale * frequency + octaveOffsets[i].x;
					float scaleY = (y-halfHeight)/scale * frequency + octaveOffsets[i].y;
					float perlinVal = Mathf.PerlinNoise(scaleX, scaleY) * 2 - 1;

					noiseHeight += perlinVal * amplitude;
					amplitude *= persistance;
					frequency *= lacunarity;
				}
				if(noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				}
				else if (noiseHeight <minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				noiseMap[x,y] = noiseHeight;
			}
		}
		// Normalizes NoiseMap 
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				noiseMap[x,y] = Mathf.InverseLerp (minNoiseHeight,maxNoiseHeight, noiseMap[x,y]);
			}
		}
		return noiseMap;
	}
}