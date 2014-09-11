using UnityEngine;
using System.Collections;

/*	HostScript controls all network coordination amongst various clients.
 *	It is instantiated within a conditional statement in GameScript, only
 *	getting called when run by the server (not the clients). */
public class HostScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Initialize player ship locations (only 1 other player at the moment.
		networkView.RPC ("SetInitialLocation", Network.connections[0], 2f, 0f);
	}	
	
	// Update is called once per frame
	void Update () {
	
	}
}
