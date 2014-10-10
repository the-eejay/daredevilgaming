using UnityEngine;
using UnityEngine.UI;
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
	public GameObject GameListOverlay;
	public GameObject GameNameOverlay;
	public GameObject magpie;
	public GameObject pelican;
	public GameObject penguin;
	
	public UnityEngine.UI.Button p1Ready;
	public UnityEngine.UI.Button p2Ready;
	public UnityEngine.UI.Button p3Ready;
	public UnityEngine.UI.Button p4Ready;
	
	bool p1 = false;
	bool p2 = false;
	bool p3 = false;
	bool p4 = false;

	public Text shipNameText;
	public Text shipDescText;
	
	public Text GameName;

	private GameObject[] ships = new GameObject[3];
	public int shipChooser = 0;

	// Use this for initialization
	void Start () {
		MasterServer.ipAddress = "123.100.141.5";
		MasterServer.port = 23466;
		MasterServer.dedicatedServer = false;
		ConOverlay.SetActive(false);
		GameListOverlay.SetActive(false);
		GameNameOverlay.SetActive(false);
		magpie = GameObject.Find ("Magpie");
		pelican = GameObject.Find ("Pelican");
		penguin = GameObject.Find ("penguin");
		ships [0] = magpie;
		ships [1] = pelican;
		ships [2] = penguin;

		magpie.collider.enabled = false;
		pelican.collider.enabled = false;
		penguin.collider.enabled = false;

		magpie.renderer.enabled = true;
		pelican.renderer.enabled = false;
		penguin.renderer.enabled = false;

	}
	
	public void JoinServer(int btn) {
		Debug.Log ("Attempting to connect to server...");
		HostData[] servers = MasterServer.PollHostList();
		NetworkConnectionError e;
		e = Network.Connect (servers[btn]);
		
		if (e != NetworkConnectionError.NoError) {
			Debug.Log ("Connection Error: " + e.ToString ());
		} else {
			GameListOverlay.SetActive(false);
			ConOverlay.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameListOverlay.activeSelf) {
			Debug.Log("What");
			HostData[] servers = MasterServer.PollHostList();
			UnityEngine.UI.Text[] buttons = GameListOverlay.GetComponentsInChildren<UnityEngine.UI.Text>();
			buttons[0].text = "Games";
			for (int i = 0; i < 5; ++i) {
				if (i == servers.Length) {
					break;
				}
				buttons[i+1].text = servers[i].gameName;
			}
			
		
		} else {

			if (Input.GetKeyDown ("left")) {
				ships[shipChooser].renderer.enabled = false;
				shipChooser--;
				if (shipChooser < 0) shipChooser = 2;
				ships[shipChooser].renderer.enabled = true;
			}
			if (Input.GetKeyDown ("right")) {
				ships[shipChooser].renderer.enabled = false;
				shipChooser++;
				if (shipChooser > 2) shipChooser = 0;
				ships[shipChooser].renderer.enabled = true;
			}
	
			shipNameText.text = ((PlayerShipScript)ships [shipChooser].GetComponent ("PlayerShipScript")).name;
			shipDescText.text = ((PlayerShipScript)ships [shipChooser].GetComponent ("PlayerShipScript")).description;
		}
	}
	
	public void CreateLobby() {
		if (!Network.isServer) {
			GameNameOverlay.SetActive(true);
		}
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
				MasterServer.RegisterHost("SpaceCOG", GameName.text);
				GameNameOverlay.SetActive(false);
				ConOverlay.SetActive(true);
				p1Ready.GetComponentsInChildren<UnityEngine.UI.Text>()[0].text = ((PlayerShipScript)ships [shipChooser].GetComponent ("PlayerShipScript")).name;
			} else {
				Debug.Log ("Initialization Error: " + e.ToString ());
			}
		} else {
			Debug.Log ("Shutting server down.");
			Network.Disconnect (200);
		}
	}
	
	public void ServerList () {
		GameListOverlay.SetActive(true);
		MasterServer.RequestHostList("SpaceCOG");
		foreach (UnityEngine.UI.Text btn in GameListOverlay.GetComponentsInChildren<UnityEngine.UI.Text>()) {
			btn.text = "";
		}
	}
	
	public void ConnectServer () {
		Debug.Log ("Attempting to connect to server...");
		NetworkConnectionError e;
		//e = Network.Connect (ConAddress.GetComponent<UnityEngine.UI.Text>().text, PORT_NO);
		/*
		if (e != NetworkConnectionError.NoError) {
			Debug.Log ("Connection Error: " + e.ToString ());
		}
		*/
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
		//ConButton.SetActive (true);
		//ConOverlay.SetActive (false);
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
		PlayerPrefs.SetInt ("ship", shipChooser);
		Application.LoadLevel ("Game");
	}
	
	// Function to be called when the 'Quit' button is pressed.
	// Closes the program.
	public void Quit () {
		Debug.Log ("Application terminated by player.");
		Application.Quit ();
	}

	public void loadSinglePlayer() {
		Game ();
	}
}
