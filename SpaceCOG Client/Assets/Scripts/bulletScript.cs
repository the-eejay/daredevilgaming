using UnityEngine;
using System.Collections;

public class bulletScript : MonoBehaviour {

	public static GameObject master;

	float spawnTime = 0.0f;
	float lifetime = 3.0f;
	float inTime = 0.06f;
	bool visible = false;
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
		if (!visible && Time.time - spawnTime > inTime ) {
			visible = true;
			GetComponent<MeshRenderer>().enabled = true;
		}
		if (Time.time - spawnTime > lifetime && networkView.isMine) {
			Network.Destroy (gameObject);
		}
	}
	
	void OnCollisionEnter (Collision col) {
		if(networkView.isMine) {
			if (col.gameObject.tag == "Player" || col.gameObject.tag == "Enemy") {
				((ServerGameScript)master.GetComponent("ServerGameScript")).Damage(col.gameObject, damage);
				Network.Destroy (gameObject);
			}
		} else {
			GetComponent<MeshRenderer>().enabled = false;
		}
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
		GetComponent<MeshRenderer>().enabled = false;
	}
}
