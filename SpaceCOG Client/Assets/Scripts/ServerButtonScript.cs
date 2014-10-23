using UnityEngine;
using System.Collections;

public class ServerButtonScript : MonoBehaviour {

	private MainMenuScript creator;

	// Use this for initialization
	void Start () {
		creator = GameObject.Find("Menu").GetComponents<MainMenuScript>()[0];
		Debug.Log ("Menu script located" + creator);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ButtonPress() {
	
	}
}
