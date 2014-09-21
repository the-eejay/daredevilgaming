using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour {
	//
	int pCount;
	int initCount;
	bool initialized;
	
	// Prefabs
	GameObject shipPrefab;
	
	// Game Objects
	GameObject[] playerShips;
	
	// Client Scripts
	ClientScript[] player;
	
	void Start() {
		// Initialize variables
		pCount = 1 + Network.connections.Length;
		initCount = 0;
		initialized = false;
		
		// Load prefabs
		shipPrefab = (GameObject) Resources.Load("Magpie");
		
		// Spawn players
		playerShips = new GameObject[pCount];
		player = new ClientScript[pCount];
		
		// Send Successful Initialization Messages
		playerShips[0] = (GameObject) Network.Instantiate(shipPrefab, new Vector3 (-5f, -5f, 0f), Quaternion.identity, 0);
		for (int i = 1; i < pCount; ++i) {
			playerShips[i] = (GameObject) Network.Instantiate(shipPrefab, new Vector3 ( -5f + 10 * (i % 2), -5f + 10 * (i / 2), 0f), Quaternion.identity, 0);
		}
	}
	
	void Update() {
		if (!initialized) {
			return;
		}
		// Update logic here
	}
	
	[RPC]
	public void LocatePlayerScript(NetworkPlayer owner, NetworkViewID pScript) {
		if (pScript.isMine) {
			player[0] = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent(typeof(ClientScript));
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
			initialized = true;
			networkView.RPC("ServerSuccessfullyInitialized", RPCMode.Server, playerShips[0].networkView.viewID);
			for (int i = 1; i < pCount; ++i) {
				networkView.RPC("ServerSuccessfullyInitialized", Network.connections[i - 1], playerShips[i].networkView.viewID);
			}
		}
	}
}