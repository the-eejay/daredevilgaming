using UnityEngine;
using System.Collections;

public class HostScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		networkView.RPC ("SetInitialLocation", Network.connections[0], 2f, 0f);
	}	
	
	// Update is called once per frame
	void Update () {
	
	}
}
