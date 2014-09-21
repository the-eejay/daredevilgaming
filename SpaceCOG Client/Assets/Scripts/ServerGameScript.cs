using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour {
	//
	int pCount = 1 + Network.connections.Length;
	int initCount = 0;
	bool initialized = false;
	
	// Prefabs
	GameObject shipPrefab;
	
	// Game Objects
	GameObject[] playerShips = new GameObject[4];
	
	// Client Scripts
	ClientScript[] player = new ClientScript[4];
	
	void Start() {
		// Put initialization stuff into InitializeGame() instead of here
	}
	
	void InitializeGame() {
		CreatePlayerShips();
	}
	
	void Update() {
		if (!initialized || !Network.isServer) {
			return;
		}
		// Update logic here
	}
	
	void FixedUpdate() {
		if (!Network.isServer) {
			return;
		}
		// Logic here
	}
	
	void CreatePlayerShips() {
		shipPrefab = (GameObject) Resources.Load("Magpie");
		playerShips[0] = (GameObject) Network.Instantiate(shipPrefab, new Vector3 (-5f, -5f, 0f), Quaternion.identity, 0);
		for (int i = 1; i < pCount; ++i) {
			playerShips[i] = (GameObject) Network.Instantiate(shipPrefab, new Vector3 ( -5f + 10 * (i % 2), -5f + 10 * (i / 2), 0f), Quaternion.identity, 0);
		}
	}
	
	[RPC]
	public void LocatePlayerScript(NetworkPlayer owner, NetworkViewID pScript) {
		if (pScript.isMine) {
			ClientScript cs = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent("ClientScript");
			player[0] = cs;
			initCount++;
		} else {
			for (int i = 1; i < pCount; ++i) {
				if (Network.connections[i-1] == owner) {
					player[i] = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent(typeof(ClientScript));
					initCount++;
					break;
				}
			}
		}
		
		if (initCount == pCount) {
			InitializeGame();
			initialized = true;
			((LocalGameScript)gameObject.GetComponent("LocalGameScript")).ServerSuccessfullyInitialized(playerShips[0].networkView.viewID);
			for (int i = 1; i < pCount; ++i) {
				networkView.RPC("ServerSuccessfullyInitialized", Network.connections[i - 1], playerShips[i].networkView.viewID);
			}
		}
	}
}