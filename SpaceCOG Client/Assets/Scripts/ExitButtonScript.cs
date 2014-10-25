using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExitButtonScript : MonoBehaviour {

	public void GameOver() {
		Network.Disconnect ();
		Application.LoadLevel ("Menu");
	}
}
