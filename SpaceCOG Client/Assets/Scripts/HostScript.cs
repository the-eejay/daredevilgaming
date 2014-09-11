using UnityEngine;
using System.Collections;

/*	HostScript controls all network coordination amongst various clients.
 *	It is instantiated within a conditional statement in GameScript, only
 *	getting called when run by the server (not the clients). */
public class HostScript : MonoBehaviour {

	int pCount;
	bool[] playerAlive;

	// Use this for initialization
	void Start () {
		pCount = 1 + Network.connections.Length;
		playerAlive = new bool[pCount];
		// Initialize player ship locations (only 1 other player at the moment).

		if (pCount > 1) {
			networkView.RPC ("SetInitialLocation", Network.connections[0], 5f, 0f);
		}
	}	
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// KillPlayer is a remote procedure call that allows a client to report that
	// they have been killed. At this point a dead player means Game Over.
	[RPC] 
	void KillPlayer (NetworkPlayer player) {
		Debug.Log ("Player Killed.");
		if (player == Network.player) {
			playerAlive[0] = false;
			GameOver ();
		} else {
			for (int i = 0; i < pCount - 1; ++i) {
				if (player == Network.connections[i]) {
					playerAlive[i + 1] = false;
					GameOver ();
				 } 
			}
		}
	}
	
	// GameOver tells all clients (and the server) that the game is finished.
	void GameOver () {
		networkView.RPC ("ServerEndsGame", RPCMode.All);
	}
	
}
