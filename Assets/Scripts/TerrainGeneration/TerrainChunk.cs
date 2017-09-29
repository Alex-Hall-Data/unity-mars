using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//class for terrain chunks. Each chunk is defined by its Xand Z position
namespace TerrainGenerator{
public class TerrainChunk:MonoBehaviour {

	public int X { get; private set;}
	public int Z { get; private set;}

	public Vector2 Position { get; private set; }
	private Terrain terrain{ get; set;}
	private TerrainChunkSettings settings{ get; set;}
	private NoiseProvider noiseProvider { get; set;}
	private TerrainData data { get; set; }
	private float[,] Heightmap { get; set; }
	

		public TerrainChunk(TerrainChunkSettings Settings, NoiseProvider NoiseProvider, int x, int z)
		{
			//HeightmapThreadLockObject = new object();

			settings = Settings;
			noiseProvider = NoiseProvider;
			//Neighborhood = new TerrainChunkNeighborhood();

			Position = new Vector2(x, z);
		}


		//creates terrain from Heightmap
		public void CreateTerrain()
		{
			var terrainData = new TerrainData();
			terrainData.heightmapResolution = settings.HeightMapResolution;
			terrainData.alphamapResolution = settings.AlphaMapResolution;

			var Heightmap = GetHeightmap();
			terrainData.SetHeights(0, 0, Heightmap);
			terrainData.size = new Vector3(settings.Length, settings.Height, settings.Length);

			var newTerrainGameObject = Terrain.CreateTerrainGameObject(terrainData);
			newTerrainGameObject.transform.position = new Vector3(X * settings.Length, 0, Z * settings.Length);
			terrain = newTerrainGameObject.GetComponent<Terrain>();
			terrain.Flush();
		}


		//creates Heightmap using noiseprovider
		private float[,] GetHeightmap()
		{
			var Heightmap = new float[settings.HeightMapResolution, settings.HeightMapResolution];

			for (var zRes = 0; zRes < settings.HeightMapResolution; zRes++)
			{
				for (var xRes = 0; xRes < settings.HeightMapResolution; xRes++)
				{
					var xCoordinate = X + (float)xRes / (settings.HeightMapResolution - 1);
					var zCoordinate = Z + (float)zRes / (settings.HeightMapResolution - 1);

					Heightmap[zRes, xRes] = noiseProvider.GetValue(xCoordinate, zCoordinate);
				}
			}

			return Heightmap;
		}


		public void Test()
		{
			var settings = new TerrainChunkSettings(129, 129, 100, 20);
			var noiseProvider = new NoiseProvider();
			var terrain = new TerrainChunk(settings, noiseProvider, 0, 0);
			terrain.CreateTerrain();
		}
}
}
