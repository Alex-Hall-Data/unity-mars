using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainGenerator
{

public class TerrainChunkSettings:MonoBehaviour  {

	//these define resolution - must be power of two plus one (start with 129)
	public int HeightMapResolution { get; private set;}
	public int AlphaMapResolution{ get; private set; }

	public int Length{ get; private set; }//length of chunk
	public int Height{ get; private set; }//max terrain height of chunk


	public TerrainChunkSettings (int heightMapResolution, int alphaMapResolution, int length, int height ){
		HeightMapResolution=heightMapResolution;
		AlphaMapResolution=alphaMapResolution;
		Length=length;
		Height=height;
	}
}
}
