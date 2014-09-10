using UnityEngine;
using System.Collections;

public class enemyScript : MonoBehaviour {

	public int scoreValue;
	private GameController gameController;

	// Use this for initialization
	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up * Time.deltaTime * 500);
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.name == "prefabBullet(Clone)") {

			Destroy (col.gameObject);
			Destroy (gameObject);
			Destroy (this);
			gameController.AddScore (scoreValue);
		} else if (col.gameObject.name == "PlayerSphere") {
			Destroy (col.gameObject);
			Destroy (gameObject);
			Destroy (this);
		}
	}
}
