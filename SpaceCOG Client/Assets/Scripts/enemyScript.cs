using UnityEngine;
using System.Collections;

public class enemyScript : MonoBehaviour {

	public int scoreValue;
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
		transform.Rotate (Vector3.up * Time.deltaTime * 500);
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.name == "prefabBullet(Clone)") {

			Network.Destroy(col.gameObject);
			Network.Destroy (gameObject);
			Destroy (this);
			gameController.AddScore (scoreValue);
		} else if (col.gameObject.name == "Magpie(Clone)") {
			// Network.Destroy (col.gameObject);
			Network.Destroy (gameObject);
			Destroy (this);
		}
	}
}
