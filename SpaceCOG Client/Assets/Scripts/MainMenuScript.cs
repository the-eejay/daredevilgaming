using UnityEngine;
using System.Collections;

/*	MainMenuScript contains all functionality required by the main menu
 *	scene in the game. It only needs to be attached to a single object
 *	in the scene. Buttons can trigger public functions as necessary. */
public class MainMenuScript : MonoBehaviour {

	// These instance variables will be obsolete by the final release
	private const int MAX_CONNECTIONS = 1;
	private const int PORT_NO = 9001;
	private const string HOST_IP = "127.0.0.1";
	private int slots = 1;
	
	public GameObject ConOverlay;
	public GameObject ConAddress;
	public GameObject ConButton;

	// Use this for initialization
	void Start () {
		ConOverlay.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Function to be called when the 'Host' button is pressed.
	// Begins listening for a connection attempt by another client.
	// Shuts down server if clicked again.
	public void HostServer () {
		if (!Network.isServer) {
			Debug.Log ("Attempting to host server...");
			NetworkConnectionError e;
			e = Network.InitializeServer (MAX_CONNECTIONS, PORT_NO, false);
			
			if (e == NetworkConnectionError.NoError) {
				Debug.Log ("Listening for connections...");
			} else {
				Debug.Log ("Initialization Error: " + e.ToString ());
			}
		} else {
			Debug.Log ("Shutting server down.");
			Network.Disconnect (200);
		}
	}
	
	public void ConnectServer () {
		Debug.Log ("Attempting to connect to server...");
		NetworkConnectionError e;
		ConButton.SetActive (false);
		e = Network.Connect (ConAddress.GetComponent<UnityEngine.UI.Text>().text, PORT_NO);
		if (e != NetworkConnectionError.NoError) {
			Debug.Log ("Connection Error: " + e.ToString ());
		}
	}
	
	// Function to be called when the 'Join' button is pressed.
	// Attempts to connect to a server on the local machine.
	public void JoinServer () {
		ConOverlay.SetActive(true);
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when a server is successfully initialized locally.
	private void OnServerInitialized () {
		Debug.Log ("Server successfully initialized.");
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when a client successfully connects to this server.
	private void OnPlayerConnected (NetworkPlayer player) {
		Debug.Log (player.ToString () + " has joined the server.");
		if (Network.connections.Length == slots) {
			Debug.Log ("Loading game...");
			networkView.RPC ("Game", RPCMode.All);
		}
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when this client successfully connects to a server.
	private void OnConnectedToServer () {
		Debug.Log ("Successfully connected to the server.");
		ConButton.SetActive (true);
		ConOverlay.SetActive (false);
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when this client is disconnected from the server.
	private void OnDisconnectedFromServer (NetworkDisconnection info) {
		Debug.Log ("Disconnected from the server: " + info.ToString ());
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when a client disconnects from this server.
	private void OnPlayerDisconnected (NetworkPlayer player) {
		Debug.Log ("Player disconnected.");
	}
	
	// This function transitions the game into the actual gameplay.
	[RPC] // This function can be called remotely over the network.
	private void Game () {
		Application.LoadLevel ("Game");
	}
	
	// Function to be called when the 'Quit' button is pressed.
	// Closes the program.
	public void Quit () {
		Debug.Log ("Application terminated by player.");
		Application.Quit ();
	}

	public void loadSinglePlayer() {
		Application.LoadLevel ("Game");
	}
}
