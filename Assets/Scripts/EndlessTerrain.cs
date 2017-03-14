using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {

	public const float maxViewDistance = 450;
	public Transform viewer;
	public static Vector2 viewerPosition;
	int chunkSize;
	int chunksVisibleInViewDistance;
	List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

	void Start() {
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
	}

	void Update() {
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		UpdateVisibleChunks();
	}

	void UpdateVisibleChunks() {

		for(int i = 0; i< terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate[i].setVisible(false);
		}
		terrainChunksVisibleLastUpdate.Clear();

		int currChunkCoordX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
		int currChunkCoordY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

		for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2(currChunkCoordX + xOffset, currChunkCoordY + yOffset);

				if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
					terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					if(terrainChunkDictionary[viewedChunkCoord].IsVisible()){
						terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
					}
				} 
				else{
					terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize,transform));
				}
			}
		}	
	}

	public class TerrainChunk {
		Vector2 position;
		GameObject meshObject;
		Bounds bounds;

		public TerrainChunk(Vector2 coord, int size, Transform parent) {
			position = coord * size; 
			bounds = new Bounds(position, Vector2.one *size);
			Vector3 positioV3 = new Vector3(position.x, 0 , position.y);

			meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			meshObject.transform.position = positioV3;
			meshObject.transform.localScale = Vector3.one * size /10f; 
			setVisible(false);
			meshObject.transform.parent = parent;
		}

		public void UpdateTerrainChunk() {
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			bool visible = viewerDstFromNearestEdge <= maxViewDistance;
			setVisible(visible);
		}

		public void setVisible(bool visible){
			meshObject.SetActive(visible);
		}

		public bool IsVisible(){
			return meshObject.activeSelf;
		}
	}
}
