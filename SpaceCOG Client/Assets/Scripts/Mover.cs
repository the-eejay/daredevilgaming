using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;

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

		Vector3 targetPos = gameController.GetPos ();
		rigidbody.velocity = (targetPos - this.transform.position).normalized * speed;
	}
}
