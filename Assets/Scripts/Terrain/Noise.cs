using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise  {

	public enum NormaliseMode{local,global};

	public static float[,] GenerateNoiseMap(int mapWidth,int mapHeight,float scale,int octaves, float persistance, float lacunarity,int seed,Vector2 offset,NormaliseMode normaliseMode){

		System.Random prng = new System.Random (seed);
		Vector2[] octavesOffsets = new Vector2[octaves];

		float maxPossibleHeight=0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < octaves; i++) {//randomly offset octabes from each other to make random landscape
			float offsetx = prng.Next (-100000, 100000)+offset.x;//empirical values
			float offsety = prng.Next (-100000, 100000)-offset.y;
			octavesOffsets [i] = new Vector2 (offsetx, offsety);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		float[,] noiseMap = new float [mapWidth, mapHeight];

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight=float.MaxValue;


		for (int y=0; y<mapHeight;y++){
			for (int x=0;x<mapWidth;x++){

				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x + octavesOffsets [i].x) / scale * frequency; // + offset.x;
					float sampleY =( y + octavesOffsets[i].y)/ scale * frequency;  //+ offset.y;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY)*2-1;// multiply by 2 and negate one to ensure in range -1 to 1
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				//get range of noise height values to defina a max and a min value (to allow for normalisation)
				if(noiseHeight>maxLocalNoiseHeight){
					maxLocalNoiseHeight=noiseHeight;
				}else if(noiseHeight<minLocalNoiseHeight){
					minLocalNoiseHeight=noiseHeight;
				}

				noiseMap [x, y] = noiseHeight;

			}
	}

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				if (normaliseMode == NormaliseMode.local) { //if generating single chunk - ie not procedural
					noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]);//inverselerp method standardises the noiseheingt (it returns a value between 0-1)
				} else {
					float normalisedHeight = (noiseMap [x, y] + 1) / (2f * maxPossibleHeight/5f); //reverses the operation above. The final float is an empirical correction to compensate for scaling by the maxpossible height
					noiseMap[x,y]=Mathf.Clamp(normalisedHeight,0,int.MaxValue);
				}
				}
		}

		return noiseMap;
	}
}
