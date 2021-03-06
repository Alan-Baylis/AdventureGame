﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
	const float scale = 2.5f;
	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

	public static float maxViewDistance;
	public LODInfo[] detailLevels;
	public Transform viewer;
	public static Vector2 viewerPosition;
	Vector2 viewerPositionOld;
	static MapGenerator mapGenerator;
	int chunkSize;
	int chunksVisibleInViewDistance;
	public Material mapMaterial;
	static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

	public void Start() {
		maxViewDistance = detailLevels[detailLevels.Length -1].visibleDstThreshold;
		mapGenerator = FindObjectOfType<MapGenerator>();
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);

		UpdateVisibleChunks();
	}

	void Update() {

		viewerPosition = new Vector2(viewer.position.x, viewer.position.z)/scale;

		if((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks();
		}
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
				} 
				else{
					terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
				}
			}
		}	
	}

	public class TerrainChunk {
		Vector2 position;
		GameObject meshObject;
		Bounds bounds;
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;
		LODInfo[] detailLevels;
		LODMesh[] lodMeshes;

		MapData mapData;
		bool mapDataReceived;
		int previousLODIndex = -1;
		 
		public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material) {
			this.detailLevels = detailLevels;
			position = coord * size;  
			bounds = new Bounds(position, Vector2.one *size);
			Vector3 positioV3 = new Vector3(position.x, 0 , position.y);

			meshObject = new GameObject("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider = meshObject.AddComponent<MeshCollider>();
			meshObject.transform.position = positioV3 * scale;
			meshObject.transform.localScale = Vector3.one * scale;
			setVisible(false);
			meshObject.transform.parent = parent;
			meshRenderer.material = material;
			lodMeshes = new LODMesh[detailLevels.Length];

			for(int i = 0; i < detailLevels.Length; i++) {
				lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
			}

			mapGenerator.RequestMapData(position, OnMapDataReceived);
		}

		void OnMapDataReceived(MapData mapData) {
			this.mapData = mapData;
			mapDataReceived = true;

			Texture2D texture = textureGenerator.TextureFromColorMap(mapData.ColorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
			meshRenderer.material.mainTexture = texture;
			UpdateTerrainChunk();
		}

		public void UpdateTerrainChunk() {
			if(mapDataReceived){
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			bool visible = viewerDstFromNearestEdge <= maxViewDistance;

			if(visible) {
				int lodIndex = 0;
				for(int i = 0; i < detailLevels.Length -1; i++){
					if(viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold){
						lodIndex = i + 1;
					} else {
						break;
					}
				}
				if(lodIndex != previousLODIndex) {
					LODMesh lodMesh = lodMeshes[lodIndex];
					if(lodMesh.hasMesh){
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
						meshCollider.sharedMesh = lodMesh.mesh;
					}
					else if(!lodMesh.hasRequestedMesh){
						lodMesh.RequestMesh(mapData);
					}
				}
				terrainChunksVisibleLastUpdate.Add(this);
			}
			setVisible(visible);
			}
		}

		public void setVisible(bool visible){
			meshObject.SetActive(visible);
		}

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

		public LODMesh(int lod, System.Action updateCallback) {
			this.updateCallback = updateCallback;
			this.lod = lod;
		}

		void OnMeshDataReceived(MeshData meshData) {
			mesh = meshData.CreateMesh();
			hasMesh = true;

			updateCallback();
		}

		public void RequestMesh(MapData mapData) {
			hasRequestedMesh = true;
			mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
		}
	}

	[System.Serializable]
	public struct LODInfo {
		public int lod;
		public float visibleDstThreshold;

	}
}
