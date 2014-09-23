using UnityEngine;
using System.Collections;

public class LocalGameScript : MonoBehaviour {
	// State variables
	private bool initialized = false;
	int pCount;
	int x = 0;
	public GameObject pScriptPrefab;
	public GameObject CompassHeadPrefab;
	public GameObject CompassBaddieHeadPrefab;
	public GameObject CompassPanel;
	private GameObject pScript;
	private bool isAlive = true;
	private bool gameOver = false;
	
	// Object References
	private GameObject ship;
	private GameObject[] compassAllies;
	private GameObject[] compassAllyBeacons;
	private GameObject[] compassBaddies;
	private GameObject[] compassBaddieBeacons;
	
	void Start() {
		// Simulate a network if playing singleplayer
		if (Network.peerType == NetworkPeerType.Disconnected) {
			Network.InitializeServer(0, 0, false);
		}
		// Launch server script if server
		//if (Network.isServer) {
			gameObject.AddComponent("ServerGameScript");
		//}
		
		pScript = (GameObject) Network.Instantiate(pScriptPrefab, Vector3.zero, Quaternion.identity, 0);
		if (Network.peerType == NetworkPeerType.Server) {
			((ServerGameScript)gameObject.GetComponent("ServerGameScript")).LocatePlayerScript(Network.player, pScript.networkView.viewID);
		} else {
			networkView.RPC("LocatePlayerScript", RPCMode.All, Network.player, pScript.networkView.viewID);
		}
	}
	
	void CentreCamera () {
		if (ship) {
			Camera.main.transform.position = ship.transform.position - 10 * Vector3.forward;
		}
	}
	
	void Update() {
		if (!initialized) {
			return;
		}
		
		CentreCamera();
		UpdateCompass();
	}
	
	void UpdateCompass() {
		// Allies
		for (int i = 0; i < pCount - 1; ++i) {
			if (compassAllies[i] == null) {
				if (compassAllyBeacons[i] != null) {
					Destroy(compassAllyBeacons[i]);
				}
			} else {
				Vector3 dir = compassAllies[i].transform.position - ship.transform.position;
				dir.Normalize ();
				compassAllyBeacons[i].transform.localPosition = dir * 80;
			}
		}
		// Enemies
		for (int i = 0; i < compassBaddies.Length; ++i) {
			if (compassBaddies[i] == null) {
				if (compassBaddieBeacons[i] != null) {
					Destroy(compassBaddieBeacons[i]);
				}
			} else {
				if (ship) {
					Vector3 dir = compassBaddies[i].transform.position - ship.transform.position;
					dir.Normalize ();
					compassBaddieBeacons[i].transform.localPosition = dir * 80;
				}
			}
		}
	}
	
	void InitCompass() {
		for (int i = 0; i < pCount - 1; ++i) {
			compassAllyBeacons[i] = (GameObject) Instantiate(CompassHeadPrefab, Vector3.zero, Quaternion.identity);
			compassAllyBeacons[i].transform.parent = CompassPanel.transform;
			compassAllyBeacons[i].GetComponent<UnityEngine.UI.Image>().color = compassAllies[i].renderer.material.color;
		}
		for (int i = 0; i < compassBaddies.Length; ++i) {
			compassBaddieBeacons[i] = (GameObject) Instantiate(CompassBaddieHeadPrefab, Vector3.zero, Quaternion.identity);
			compassBaddieBeacons[i].transform.parent = CompassPanel.transform;
			compassBaddieBeacons[i].GetComponent<UnityEngine.UI.Image>().color = compassBaddies[i].renderer.material.color;
		}
	}
	
	[RPC]
	public void ServerSuccessfullyInitialized(NetworkViewID ship, int AllyCount) {
		this.ship = NetworkView.Find(ship).gameObject;
		pCount = AllyCount;
		compassAllies = new GameObject[pCount-1];
		compassAllyBeacons = new GameObject[pCount-1];
	}
	
	[RPC]
	public void ServerSendAllyRef(NetworkViewID ship) {
		if (NetworkView.Find(ship).gameObject != this.ship) {
			compassAllies[x++] = NetworkView.Find(ship).gameObject;	
		}
	}
	
	[RPC]
	public void ServerSendBaddieCount (int c) {
		x = 0;
		compassBaddies = new GameObject[c];
		compassBaddieBeacons = new GameObject[c];
	}
	
	[RPC]
	public void ServerSendBaddieRef(NetworkViewID baddie) {
		Debug.Log ("Baddie " + x + " " + compassBaddies.Length);
		compassBaddies[x++] = NetworkView.Find (baddie).gameObject;
		if (x == compassBaddies.Length) {
			InitCompass();
			initialized = true;
			Debug.Log ("Initialized");
		}
	}

	[RPC]
	public void Kill(NetworkViewID ship) {
		if (NetworkView.Find(ship).gameObject == this.ship) {
			isAlive = false;
		}
	}
	[RPC]
	public void GameOver() {
		Debug.Log ("Game Over");
		gameOver = true;

	}

	private void OnGUI() {
		if (gameOver) {
			string aliveString = isAlive ? "You win! " : "You lose! ";
			GUI.Label (new Rect ((Screen.width - 150) / 2, (Screen.height - 150) / 2, 300, 300), "Game over. " + aliveString);
			if (GUI.Button (new Rect ((Screen.width - 150) / 2, (Screen.height + 100) / 2, 250, 100), "Continue"))
				Application.LoadLevel ("Menu");
		}
	}


}
