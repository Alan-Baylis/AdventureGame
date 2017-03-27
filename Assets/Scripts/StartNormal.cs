using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNormal : MonoBehaviour {
public Transform cat;

	// Use this for initialization
	void Start () {
		int x,z = 0;
	MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
		mapGenerator.DrawMapInEditor();
		for (int i = 0; i < 200; i++) {
			x = UnityEngine.Random.Range(-1000,1000);
			z = UnityEngine.Random.Range(-1000,1000);
			Instantiate(cat, new Vector3(x,80,z), Quaternion.identity);
		}


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
