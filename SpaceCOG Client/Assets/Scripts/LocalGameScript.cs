using UnityEngine;
using System.Collections;

public class LocalGameScript : MonoBehaviour {
	// State variables
	private bool initialized = false;
	int pCount;
	int x = 0;
	public GameObject pScriptPrefab;
	public GameObject CompassHeadPrefab;
	public GameObject CompassPanel;
	private GameObject pScript;
	
	// Object References
	private GameObject ship;
	private GameObject[] compassAllies;
	private GameObject[] compassAllyBeacons;
	
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
		Camera.main.transform.position = ship.transform.position - 10 * Vector3.forward;
	}
	
	void Update() {
		if (!initialized) {
			return;
		}
		
		CentreCamera();
		UpdateCompass();
	}
	
	void UpdateCompass() {
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
	}
	
	void InitCompass() {
		for (int i = 0; i < pCount - 1; ++i) {
			compassAllyBeacons[i] = (GameObject) Instantiate(CompassHeadPrefab, Vector3.zero, Quaternion.identity);
			compassAllyBeacons[i].transform.parent = CompassPanel.transform;
			compassAllyBeacons[i].GetComponent<UnityEngine.UI.Image>().color = compassAllies[i].renderer.material.color;
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
		if (x == pCount - 1) {
			InitCompass();
			initialized = true;
		}
	}
}
