using UnityEngine;
using System.Collections;

public class PlayerShipScript : MonoBehaviour {

	GameScript controller;

	// Use this for initialization
	void Start () {
		controller = GameObject.Find ("WorldScriptObject").GetComponent<GameScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		//this.transform.position = new Vector3 (this.transform.position.x, 0.0f, 0.0f);
	}
	
	void FixedUpdate () {
		//this.transform.position = new Vector3 (this.transform.position.x, 0.0f, 0.0f);
	}
	
	// Temporarily programmed to cause the player's death whenever it is knocked by anything.
	private void OnCollisionEnter (Collision col) {
		controller.ReportDeath ();
	}
	
}
