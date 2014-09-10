using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreText : MonoBehaviour {

	GameController gameController;

	public static int score;

	Text text;

	// Use this for initialization
	void Start () {

		text = GetComponent<Text>();

		score = 0;

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
		text.text = "Score: " + gameController.GetScore();
	}

	void onGUI() {

	}

}
