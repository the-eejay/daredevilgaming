using UnityEngine;
using System.Collections;

public class PlayerShipScript : MonoBehaviour {

	private GameScript gameController;

	// Use this for initialization
	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameScript>();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate () {
	
	}
	
	// Temporarily programmed to cause the player's death whenever it is knocked by anything.
	private void OnCollisionEnter (Collision col) {
		gameController.ReportDeath ();
	}
	
}
