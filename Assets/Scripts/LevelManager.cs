using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R)) {
			if(SceneManager.GetActiveScene().buildIndex == 2)
				SceneManager.LoadScene(2);
		}
	}

	public void LoadInfinite(){
		SceneManager.LoadScene(1);
	}

	public void LoadNormal(){
		SceneManager.LoadScene(2);
	}
}
