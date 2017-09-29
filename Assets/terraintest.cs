using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainGenerator{

public class terraintest : MonoBehaviour {

	private TerrainChunk terrainGenerator;

	// Use this for initialization
	void Start () {
		terrainGenerator = GetComponent<TerrainChunk> ();
			terrainGenerator.Test ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
}
