﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalGameScript : MonoBehaviour
{
		// State variables
		private bool initialized = false;
		private bool compassInitialized = false;
		int pCount;
		int enemyCount = 0;
		int WaveCounter = 0;
		int x = 0;
		public GameObject pScriptPrefab;
		public GameObject CompassHeadPrefab;
		public GameObject CompassBaddieHeadPrefab;
		public GameObject CompassPanel;
		public Slider HealthSlider;
		private GameObject pScript;
		private bool isAlive = true;
		private bool gameOver = false;
		private bool boss = false;
		private float health = 100f;
	
		// Object References
		private GameObject ship;
		private GameObject[] compassAllies;
		private GameObject[] compassAllyBeacons;
		private GameObject[] compassBaddies = new GameObject[1];
		private GameObject[] compassBaddieBeacons = new GameObject[1];
		private ArrayList enemies = new ArrayList ();

		// Status Script
		private PlayerStatusScript playerStats = new PlayerStatusScript ();

		void Start ()
		{
				// Simulate a network if playing singleplayer
				if (Network.peerType == NetworkPeerType.Disconnected) {
						Network.InitializeServer (0, 0, false);
				}
				// Launch server script if server
				//if (Network.isServer) {
				gameObject.AddComponent ("ServerGameScript");
				//}
		
				pScript = (GameObject)Network.Instantiate (pScriptPrefab, Vector3.zero, Quaternion.identity, 0);
				if (Network.peerType == NetworkPeerType.Server) {
						((ServerGameScript)gameObject.GetComponent ("ServerGameScript")).LocatePlayerScript (Network.player, pScript.networkView.viewID);
				} else {
						networkView.RPC ("LocatePlayerScript", RPCMode.All, Network.player, pScript.networkView.viewID);
				}

		}
	
		void CentreCamera ()
		{
				if (ship) {
						Camera.main.transform.position = ship.transform.position - 20 * Vector3.forward;
				}
		}
	
		void Update ()
		{
				if (!initialized) {
						return;
				}
				if (!compassInitialized) {
						InitCompass ();
				}

				CentreCamera ();
				UpdateCompass ();
				UpdateHealth ();
		}

		void UpdateHealth ()
		{
				HealthSlider.value = health;
				Debug.Log (health);
		}
	
		void UpdateCompass ()
		{
				// Allies
				for (int i = 0; i < pCount - 1; ++i) {
						if (compassAllies [i] == null) {
								if (compassAllyBeacons [i] != null) {
										Destroy (compassAllyBeacons [i]);
								}
						} else {
								Vector3 dir = compassAllies [i].transform.position - ship.transform.position;
								dir.Normalize ();
								compassAllyBeacons [i].transform.localPosition = dir * 80;
						}
				}
				// Enemies
				for (int i = 0; i < compassBaddies.Length; ++i) {
						if (compassBaddies [i] == null) {
								if (compassBaddieBeacons [i] != null) {
										Destroy (compassBaddieBeacons [i]);
								}
						} else {
								if (ship) {
										Vector3 dir = compassBaddies [i].transform.position - ship.transform.position;
										dir.Normalize ();
										compassBaddieBeacons [i].transform.localPosition = dir * 80;
								}
						}
				}
		}
	
		void InitCompass ()
		{
				for (int i = 0; i < pCount - 1; ++i) {
						compassAllyBeacons [i] = (GameObject)Instantiate (CompassHeadPrefab, Vector3.zero, Quaternion.identity);
						compassAllyBeacons [i].transform.parent = CompassPanel.transform;
						compassAllyBeacons [i].GetComponent<UnityEngine.UI.Image> ().color = compassAllies [i].renderer.material.color;
				}
				compassInitialized = true;
		}
	
		[RPC]
		public void UpdatePlayerHealth (NetworkViewID ship, float h)
		{
				if (NetworkView.Find (ship).gameObject == this.ship) {
						health = h;
				}
		}

		[RPC]
		public void ServerSuccessfullyInitialized (NetworkViewID ship, int AllyCount)
		{
				this.ship = NetworkView.Find (ship).gameObject;
				pCount = AllyCount;
				compassAllies = new GameObject[pCount - 1];
				compassAllyBeacons = new GameObject[pCount - 1];
		}
	
		[RPC]
		public void ServerSendAllyRef (NetworkViewID ship)
		{
				if (NetworkView.Find (ship).gameObject != this.ship) {
						compassAllies [x++] = NetworkView.Find (ship).gameObject;	
				}
		}

		[RPC]
		public void UpdateEnemyCount (int enemies)
		{
				enemyCount = enemies;
				Debug.Log ("Enemy count: " + enemyCount);
				System.Array.Resize (ref compassBaddies, enemies);
				System.Array.Resize (ref compassBaddieBeacons, enemies);
		}
	
		[RPC]
		public void ServerSendBaddieRef (NetworkViewID baddie)
		{
				Debug.Log ("Baddie " + x + " " + enemyCount);
				compassBaddies [x] = NetworkView.Find (baddie).gameObject;
				compassBaddieBeacons [x] = (GameObject)Instantiate (CompassBaddieHeadPrefab, Vector3.zero, Quaternion.identity);
				compassBaddieBeacons [x].transform.parent = CompassPanel.transform;
				compassBaddieBeacons [x].GetComponent<UnityEngine.UI.Image> ().color = compassBaddies [x].renderer.material.color;
				x++;

		}

		[RPC]
		public void Kill (NetworkViewID ship)
		{
				if (NetworkView.Find (ship).gameObject == this.ship) {
						isAlive = false;
				}
		}

		[RPC]
		public void Hurt(float damage, NetworkViewID ship) {
			if (NetworkView.Find (ship).gameObject == this.ship) {
				health -= damage;
			}
		}

		[RPC]
		public void GameOver ()
		{
				Debug.Log ("Game Over");
				Destroy (GameObject.Find ("Magpie"));
				gameOver = true;

		}

		[RPC]
		public void NextWave ()
		{
				WaveCounter++;
		}

		[RPC]
		public void BossMode ()
		{
				boss = true;
		}

		[RPC]
		public void Initialize ()
		{
				initialized = true;
		}

		[RPC]
		public void LocatePlayerStatusScript (NetworkPlayer owner, NetworkViewID pStatusScript)
		{
				if (pStatusScript.isMine) {
						PlayerStatusScript pss = (PlayerStatusScript)NetworkView.Find (pStatusScript).gameObject.GetComponent ("PlayerStatusScript");
				}
		}

		private void OnGUI ()
		{
				string waveString = boss ? "Final Boss! " : "Wave " + WaveCounter;
				GUI.Label (new Rect ((Screen.width / 2), 50, 150, 150), waveString);

				if (gameOver) {
						string aliveString = isAlive ? "You win! " : "You lose! ";
						GUI.Label (new Rect ((Screen.width - 150) / 2, (Screen.height - 150) / 2, 300, 300), "Game over. " + aliveString);
						if (GUI.Button (new Rect ((Screen.width - 150) / 2, (Screen.height + 100) / 2, 250, 100), "Continue"))
								Application.LoadLevel ("Menu");
				}
		}


}
