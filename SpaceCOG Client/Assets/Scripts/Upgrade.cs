using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour {

	public GameObject ship;
	public GameObject player;
	public PlayerShipScript shipScript;

	Color[] colours = new Color[6] {
		Color.cyan,
		Color.red,
		Color.green,
		Color.yellow,
		Color.magenta,
		Color.blue
	};

	public float currency;
	
	//Upgrade the ships 
	public void UpgradeButton () {
		currency = PlayerPrefs.GetFloat("Money");
		if (currency >= 20) {
			player = GameObject.FindGameObjectWithTag ("Player");
			shipScript = (PlayerShipScript)player.GetComponent ("PlayerShipScript");
			// Pick a random colour to change the bullet to
			shipScript.bullet.renderer.material.color = colours [Random.Range (0, colours.Length)];
			currency -= 20;
			PlayerPrefs.SetFloat ("Money", currency);
		}
	}
	
}
