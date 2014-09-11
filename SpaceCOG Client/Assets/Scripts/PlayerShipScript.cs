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
		
	}
	
	void FixedUpdate () {
	
	}
	
	// Temporarily programmed to cause the player's death whenever it is knocked by anything.
	private void OnCollisionEnter (Collision col) {
		controller.ReportDeath ();
	}
	
}
