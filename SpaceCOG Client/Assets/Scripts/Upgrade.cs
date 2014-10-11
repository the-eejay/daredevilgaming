﻿using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour {

	public GameObject ship;
	public GameObject player;
	public PlayerShipScript shipScript;

	Color[] colours = new Color[6];

	//Upgrade the ships 
	public void UpgradeButton () {

		colours [0] = Color.cyan;
		colours [1] = Color.red;
		colours [2] = Color.green;
		colours [3] = Color.yellow;
		colours [4] = Color.magenta;
		colours [5] = Color.blue;

		player = GameObject.FindGameObjectWithTag ("Player");
		shipScript = (PlayerShipScript)player.GetComponent("PlayerShipScript");
		shipScript.bullet.renderer.material.color = colours[Random.Range(0, colours.Length)];

		Debug.Log ("Upgraded !");
	}
}
