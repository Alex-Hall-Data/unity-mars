using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//info at https://www.youtube.com/watch?v=4RpVBYW1r5M&index=5&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3

public static class MeshGenerator  {

	public static MeshData GenerateTerrainMesh(float[,] heightMap,float heightMultiplier,int levelOfDetail){
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);
		float topLeftx = (width - 1) / -2f;
		float topLeftz = (height - 1) / 2f;
		int meshSimplificationIncrement=(levelOfDetail==0)?1:levelOfDetail*2; //shorthand for ifelse - prevents a zero value in this case
		int verticesPerLine=((width-1)/(meshSimplificationIncrement))+1;

		MeshData meshData = new MeshData (verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y+=meshSimplificationIncrement) {
			for (int x = 0; x < width; x+=meshSimplificationIncrement) {

				meshData.vertices[vertexIndex]=new Vector3(topLeftx+x,heightMap[x,y]*heightMultiplier,topLeftz-y);
				meshData.UVs [vertexIndex] = new Vector2 (x / (float)width, y / (float)height);

				//add triangles - 10 min mark in this video explains it: https://www.youtube.com/watch?v=4RpVBYW1r5M&index=5&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
				//ignore edge cases
				if (x < width - 1 && y < height - 1) {
					meshData.AddTriangle (vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
					meshData.AddTriangle (vertexIndex+verticesPerLine+1, vertexIndex, vertexIndex + 1);

				}

				vertexIndex++;
			}
		}
		return meshData;
	}
}

public class MeshData{

	//vertices of the mesh and resulting triangles (see link at top)
	public Vector3[] vertices;
	public int[] triangles;
	int triangleIndex;
	public Vector2[] UVs;

	public MeshData(int meshWidth,int meshHeight){
		vertices =new Vector3[meshWidth*meshHeight];
		triangles=new int [(meshWidth-1)*(meshHeight-1)*6];
		UVs = new Vector2[meshWidth * meshHeight];
	}

	public void AddTriangle(int a, int b, int c){
		triangles [triangleIndex] = a;
		triangles [triangleIndex+1] = b;
		triangles [triangleIndex+2] = c;
		triangleIndex += 3;
	}

	public Mesh CreateMesh(){
		Mesh mesh=new Mesh();
		mesh.vertices=vertices;
		mesh.triangles=triangles;
		mesh.uv=UVs;
		mesh.RecalculateNormals();
		return mesh;
	}
}