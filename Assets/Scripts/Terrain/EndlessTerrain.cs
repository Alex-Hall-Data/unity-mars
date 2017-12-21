using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO - make active object the viewer (should be easy - it's a cube right now)
//change maxviewdistance with height
//consider fixing normals at seams (ideo 11) - left out for now as may not be noticeable

public class EndlessTerrain : MonoBehaviour {

	//important - this dictates the scaling of the entire terrain
	const float scale = 1;

	const float viewerMoveThresholdForChunkUpdate=25f;//stops chunks updating every frame - viewer must move this distance first
	const float sqrviewerMoveThresholdForChunkUpdate=viewerMoveThresholdForChunkUpdate*viewerMoveThresholdForChunkUpdate; //(getting square distance is quicker than getting actual distance)

	public static float maxViewDistance; //this will have tochange when launching glider etc, it currenctly depends on LOD levels in start method
	public Transform viewer;
	public Material mapMaterial;
	public LODInfo[] detailLevels;

	public static Vector2 viewerPosition;
	Vector2 viewerPositionOld;
	static MapGenerator mapGenerator;
	int chunkSize;
	int chunksVisibleInViewDistance;


	Dictionary<Vector2,TerrainChunk> terainChunkDictionary=new Dictionary<Vector2,TerrainChunk>(); //keep a dictionary of chunks to prevent duplication
	static List<TerrainChunk> terrainChunksVisibleLastUpdate=new List<TerrainChunk>();

	void Start(){
		mapGenerator = FindObjectOfType<MapGenerator> ();

		maxViewDistance = detailLevels [detailLevels.Length - 1].visibleDistanceThreshold;
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDistance = Mathf.RoundToInt (maxViewDistance / chunkSize);

		//update at start to set landscape
		UpdateVisibleChunks ();
	}

	void Update(){
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z)/scale;

		//only update surrounding chunks if viewer moves more than threshold
		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrviewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
		}
	}

	void UpdateVisibleChunks(){

		//set all chunks as invisible first (ensures old ones dissapear)
		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();

		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {
				Vector2 ViewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
			
				if (terainChunkDictionary.ContainsKey (ViewedChunkCoord)) {
					terainChunkDictionary [ViewedChunkCoord].UpdateTerrainChunk ();

				} else {
					terainChunkDictionary.Add (ViewedChunkCoord, new TerrainChunk (ViewedChunkCoord,chunkSize,detailLevels,transform,mapMaterial));
				}
			}
		}
	}

		public class TerrainChunk{

		GameObject meshObject;
		Vector2 position;
		Bounds bounds;
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;
		LODInfo[] detailLevels;
		LODMesh[] lodMeshes;
		LODMesh collisionLODMesh;

		MapData mapData;
		bool mapDataReceived;
		int previousLODIndex=-1;

		public TerrainChunk(Vector2 coord,int size,LODInfo[] detailLevels ,Transform parent,Material material){
			this.detailLevels=detailLevels;

			position=coord*size; //position to instantiate at 
			bounds=new Bounds(position,Vector2.one*size);
			Vector3 positionV3=new Vector3(position.x,0,position.y);

			meshObject=new GameObject("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider=meshObject.AddComponent<MeshCollider>();

			meshRenderer.material=material;

			meshObject.transform.position=positionV3*scale;
			meshObject.transform.parent=parent;
			meshObject.transform.localScale=Vector3.one*scale;
			SetVisible(false);//not visible by default - update method sets chunk as visible if appropriate
		
			lodMeshes=new LODMesh[detailLevels.Length];
			for(int i=0;i<detailLevels.Length;i++){
				lodMeshes[i]=new LODMesh(detailLevels[i].lod,UpdateTerrainChunk);
				if(detailLevels[i].useForCollider){
					collisionLODMesh=lodMeshes[i];
				}
			}

			mapGenerator.RequestMapData(position,OnMapDataReceived);
		}

		void OnMapDataReceived(MapData mapData){
		//	mapGenerator.RequestMeshData (mapData, OnMeshDataReceived);
			this.mapData=mapData;
			mapDataReceived = true;

			Texture2D texture = TextureGenerator.TextureFromColorMap (mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
			meshRenderer.material.mainTexture = texture;

			UpdateTerrainChunk ();
		}

		/*void OnMeshDataReceived(MeshData meshData){
			meshFilter.mesh = meshData.CreateMesh ();
		}*/

		//enable mesh object if within view distance
		public void UpdateTerrainChunk(){
			if (mapDataReceived) {
				float viewerDistancefromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));
				bool visible = viewerDistancefromNearestEdge <= maxViewDistance;
				if (visible) {
					int lodIndex = 0;
					for (int i = 0; i < detailLevels.Length - 1; i++) {
						if (viewerDistancefromNearestEdge > detailLevels [i].visibleDistanceThreshold) {
							lodIndex = i + 1;
						} else {
							break;
						}
					}
					if (lodIndex != previousLODIndex) {
						LODMesh lodMesh = lodMeshes [lodIndex];
						if (lodMesh.hasMesh) {
							previousLODIndex = lodIndex;
							meshFilter.mesh = lodMesh.mesh;
							meshCollider.sharedMesh = lodMesh.mesh;
						} else if (!lodMesh.hasRequestedMesh) {
							lodMesh.RequestMesh (mapData);
						}
					}

					//only add collision mesh to nearest chunk
					if(lodIndex==0){
						if (collisionLODMesh.hasMesh) {
							meshCollider.sharedMesh = collisionLODMesh.mesh;
						} else if (!collisionLODMesh.hasRequestedMesh) {
							collisionLODMesh.RequestMesh (mapData);

						}
					}

					terrainChunksVisibleLastUpdate.Add (this);
				}

				SetVisible (visible);
			}
		}

		public void SetVisible(bool visible){
			meshObject.SetActive (visible);
		}

		//determines whether a chunk is visible
		public bool IsVisible(){
			return meshObject.activeSelf;
		}
		}

	class LODMesh{

		public Mesh mesh;
		public bool hasRequestedMesh;
		public bool hasMesh;
		int lod;
		System.Action updateCallback;

		public LODMesh(int lod,System.Action updateCallback){
			this.lod=lod;
			this.updateCallback=updateCallback;
		}

		void OnMeshDataReceived(MeshData meshData){
			mesh = meshData.CreateMesh();
			hasMesh = true;
			updateCallback ();
		}

		public void RequestMesh(MapData mapData){
			hasRequestedMesh=true;
			mapGenerator.RequestMeshData(mapData,lod,OnMeshDataReceived);
		}
	}

	[System.Serializable]
	public struct LODInfo{
		public int lod;
		public float visibleDistanceThreshold;//switch to next LOD at this distance
		public bool useForCollider;
	}
}
