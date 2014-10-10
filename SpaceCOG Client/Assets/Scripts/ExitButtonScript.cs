using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExitButtonScript : MonoBehaviour {

	public void GameOver() {
		Application.LoadLevel ("Menu");
	}
}
