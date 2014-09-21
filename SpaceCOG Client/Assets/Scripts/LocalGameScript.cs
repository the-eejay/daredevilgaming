using UnityEngine;
using System.Collections;

public class LocalGameScript : MonoBehaviour {
	// State variables
	private bool initialized = false;
	private GameObject pScriptPrefab;
	private GameObject pScript;
	
	// Object References
	private GameObject ship;
	
	void Start() {
		// Simulate a network if playing singleplayer
		if (Network.peerType == NetworkPeerType.Disconnected) {
			Network.InitializeServer(0, 0, false);
		}
		// Launch server script if server
		if (Network.isServer) {
			gameObject.AddComponent("ServerGameScript");
		}
		
		pScriptPrefab = (GameObject) Resources.Load("PlayerScriptObject");
		pScript = (GameObject) Network.Instantiate(pScriptPrefab, Vector3.zero, Quaternion.identity, 0);
		networkView.RPC("LocatePlayerScript", RPCMode.Server, Network.player, pScript.networkView.viewID);
	}
	
	void CentreCamera () {
		Camera.main.transform.position = ship.transform.position - 10 * Vector3.forward;
	}
	
	void Update() {
		if (!initialized) {
			return;
		}
		
		CentreCamera();
	}
	
	[RPC]
	public void ServerSuccessfullyInitialized(NetworkViewID ship) {
		initialized = true;
		this.ship = NetworkView.Find(ship).gameObject;
	}
}
