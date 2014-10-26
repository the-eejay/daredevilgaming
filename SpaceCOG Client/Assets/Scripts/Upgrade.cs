using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour {

	public GameObject ship;
	public GameObject player;
	public PlayerShipScript shipScript;

	Color[] colours = new Color[6];

	public float currency;

	// Use this for initialization
	void Start () {
		colours [0] = Color.cyan;
		colours [1] = Color.red;
		colours [2] = Color.green;
		colours [3] = Color.yellow;
		colours [4] = Color.magenta;
		colours [5] = Color.blue;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//Upgrade the ships 
	public void UpgradeButton () {
		

		


		currency = PlayerPrefs.GetFloat("Money");
		if (currency >= 20) {
			player = GameObject.FindGameObjectWithTag ("Player");
			shipScript = (PlayerShipScript)player.GetComponent ("PlayerShipScript");
			shipScript.bullet.renderer.material.color = colours [Random.Range (0, colours.Length)];
			currency -= 20;
			PlayerPrefs.SetFloat ("Money", currency);

			Debug.Log ("Upgraded !");
		}
	}
}
