﻿using UnityEngine;
using System.Collections;

public class bulletScript : MonoBehaviour {

	public static GameObject master;

	float spawnTime = 0.0f;
	float lifetime = 3.0f;
	public float damage;

	// Use this for initialization
	void Start () {
		spawnTime = Time.time;
		if(master == null) {
			master = GameObject.Find("WorldScriptObject");
		}

		Physics.IgnoreLayerCollision (8, 8);

	}
	
	// Update is called once per frame
	void Update () {
		// Limit bullet lifetime to prevent memory leaks.
		if (Time.time - spawnTime > lifetime && networkView.isMine) {
			Network.Destroy (gameObject);
		}
	}
	
	void OnCollisionEnter (Collision col) {
		if(networkView.isMine) {
			if (col.gameObject.tag == "Player") {
				((PlayerShipScript) col.gameObject.GetComponent ("PlayerShipScript")).Damage (damage);
				Network.Destroy (gameObject);
			} else if (col.gameObject.tag == "Enemy") {
				Debug.Log ("Hit enemy");
				((enemyScript)col.gameObject.GetComponent ("enemyScript")).Damage (damage);
				Network.Destroy (gameObject);
			}
		}
	}
}
