using UnityEngine;
using System.Collections;

public class bulletScript : MonoBehaviour {

	public static GameObject master;

	float spawnTime = 0.0f;
	float lifetime = 3.0f;

	// Use this for initialization
	void Start () {
		spawnTime = Time.time;
		if(master == null) {
			master = GameObject.Find("WorldScriptObject");
		}
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
			((ServerGameScript)master.GetComponent("ServerGameScript")).Damage(col.gameObject, 1f);
			Network.Destroy(gameObject);
		}
	}
}
