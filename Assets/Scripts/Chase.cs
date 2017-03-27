using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour {
	Transform player;
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		player = FindObjectOfType<Camera>().transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(player.position, this.transform.position) < 36){
			Vector3 direction = player.position - this.transform.position;
			direction.y = 0;

			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.2f);
	
			if(direction.magnitude > 5)
			{
				anim.SetBool("isRunning",true);
				this.transform.Translate(0,0,.1f);
			}
		} else {
			anim.SetBool("isRunning", false);
		}
	}
}
