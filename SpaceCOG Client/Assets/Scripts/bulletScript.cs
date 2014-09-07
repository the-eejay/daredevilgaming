using UnityEngine;
using System.Collections;

public class bulletScript : MonoBehaviour {

	float spawnTime = 0.0f;
	float lifetime = 1.0f;

	// Use this for initialization
	void Start () {
		spawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - spawnTime > lifetime) {
			Destroy (this);
			Destroy (gameObject);
		}
	}
}
