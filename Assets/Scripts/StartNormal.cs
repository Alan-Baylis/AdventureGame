using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNormal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
		mapGenerator.DrawMapInEditor();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
