using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
	public enum NormalizeMode{Local, Global};

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode) {
		float [,] noiseMap = new float[mapWidth,mapHeight];
		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;
		System.Random random = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i <octaves; i++) {	
			float offsetX = random.Next(-100000, 100000) + offset.x;
			float offsetY = random.Next(-100000, 100000) - offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if(scale <= 0)
			scale = 0.0001f; 

			float maxLocalNoiseHeight = float.MinValue;
			float minLocalNoiseHeight = float.MaxValue;
			float halfWidth = mapWidth / 2f;
			float halfHeight = mapHeight / 2f;
	
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				float noiseHeight = 0;
				amplitude = 1;
				frequency = 1;

				for(int i = 0; i < octaves; i++){
					float scaleX = (x-halfWidth+ octaveOffsets[i].x)/scale * frequency;
					float scaleY = (y-halfHeight+ octaveOffsets[i].y)/scale * frequency;
					float perlinVal = Mathf.PerlinNoise(scaleX, scaleY) * 2 - 1;

					noiseHeight += perlinVal * amplitude;
					amplitude *= persistance;
					frequency *= lacunarity;
				}
				if(noiseHeight > maxLocalNoiseHeight) {
					maxLocalNoiseHeight = noiseHeight;
				}
				else if (noiseHeight <minLocalNoiseHeight) {
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x,y] = noiseHeight;
			}
		}
		// Normalizes NoiseMap 
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
			if(normalizeMode == NormalizeMode.Local){
				noiseMap[x,y] = Mathf.InverseLerp (minLocalNoiseHeight,maxLocalNoiseHeight, noiseMap[x,y]);
				}else {
					float normalizedHeight = (noiseMap[x,y] + 1)/(2f * maxPossibleHeight/1.75f);
					noiseMap[x,y] = Mathf.Clamp(normalizedHeight,0,int.MaxValue);
				}
			}
		}
		return noiseMap;
	}
}
 