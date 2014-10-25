using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*	MainMenuScript contains all functionality required by the main menu
 *	scene in the game. It only needs to be attached to a single object
 *	in the scene. Buttons can trigger public functions as necessary. */
public class MainMenuScript : MonoBehaviour {

	// These instance variables will be obsolete by the final release
	private const int MAX_CONNECTIONS = 3;
	private const int PORT_NO = 9001;
	private const string HOST_IP = "127.0.0.1";
	private int slots = 3;

	public GameObject GameListOverlay;
	public GameObject GameNameOverlay;
	public GameObject WaitingOverlay;
	
	public GameObject magpie;
	public GameObject pelican;
	public GameObject penguin;
	
	
	public Button btnPrefab;
	
	private List<Button> serverButtons;

	public Text shipNameText;
	public Text loadingText;

	public Slider hpSlider;
	public Slider dmgSlider;
	public Slider speedSlider;
	public Text shipDescText;
	
	HostData[] servers;
	
	public GameObject GameList;
	
	public Text GameName;
	
	public Text wGameName;
	public Text wGameStatus;
	public Button wGo;
	public Button wLeave;
	
	private float lastListPoll;

	private GameObject[] ships = new GameObject[3];
	public int shipChooser = 0;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1.0f;
		serverButtons = new List<Button>();
		MasterServer.ipAddress = "123.100.141.5";
		MasterServer.port = 23466;
		MasterServer.dedicatedServer = false;
		//ConOverlay.SetActive(false);
		GameListOverlay.SetActive(false);
		GameNameOverlay.SetActive(false);
		WaitingOverlay.SetActive(false);
		magpie = GameObject.Find ("Magpie");
		pelican = GameObject.Find ("Pelican");
		penguin = GameObject.Find ("penguin");
		ships [0] = magpie;
		ships [1] = pelican;
		ships [2] = penguin;
		magpie.collider.enabled = false;
		pelican.collider.enabled = false;
		penguin.collider.enabled = false;

		loadingText.text = "";

		for (int i = 0; i < ships.Length; i++) {
			// Set max health on slider
			PlayerShipScript playership = ((PlayerShipScript)ships[i].GetComponent ("PlayerShipScript"));
			float damage = ((bulletScript) playership.bullet.GetComponent("bulletScript")).damage;
			if (playership.hp > hpSlider.maxValue) {
				hpSlider.maxValue = playership.hp;
			}
			// Set max damage on slider
			if (damage> dmgSlider.maxValue) {
				dmgSlider.maxValue = damage;
			}
			// Set max speed on slider
			if (playership.maxSpeed > speedSlider.maxValue) {
				speedSlider.maxValue = playership.maxSpeed;
			}
		}

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
		}
	}
	
	public void LeaveServerList() {
		GameListOverlay.SetActive(false);
	}
	
	public void ConnectToNumber(int i) {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameListOverlay.activeSelf) {
			if (Time.time - lastListPoll > 5.0) {
				Debug.Log("Refreshing list...");
				MasterServer.ClearHostList();
				lastListPoll = Time.time;
				MasterServer.RequestHostList("SpaceCOG");
			}
			servers = MasterServer.PollHostList();
			Debug.Log(servers.Length);
			if (serverButtons.Count != servers.Length) {
				Debug.Log ("Creating List");
				while(serverButtons.Count > 0) {
					DestroyImmediate(serverButtons[0].gameObject);
					serverButtons.RemoveAt(0);
				}
				serverButtons.Clear();
				for (int i = 0; i < servers.Length; ++i) {
					//Debug.Log ("NewServerButton");
					UnityEngine.UI.Button tmp = Instantiate(btnPrefab, Vector3.zero, Quaternion.identity) as UnityEngine.UI.Button;
					tmp.transform.parent = GameList.transform;
					tmp.transform.localPosition = Vector3.zero + new Vector3(0, -100 * i + 50 * (servers.Length-1), 0);
					tmp.transform.localScale = new Vector3(1, 1, 1);
					tmp.GetComponentsInChildren<UnityEngine.UI.Text>()[0].text = servers[i].gameName;
					tmp.onClick.AddListener(delegate {
							Debug.Log ("Attempting to connect to server...");
							//Debug.Log (i);
							//Debug.Log (servers[i-1]);
							
							NetworkConnectionError e;
							e = Network.Connect (servers[i-1]);
							if (e != NetworkConnectionError.NoError) {
								Debug.Log ("Connection Error: " + e.ToString ());
							} else {
								wGameName.text = servers[i-1].gameName;
								
							}
							GameListOverlay.SetActive(false);
						});
					Destroy(tmp, 5.0f);
					serverButtons.Add(tmp);
				}
			}
			
		} else if (!GameNameOverlay.activeSelf && !WaitingOverlay.activeSelf){

			if (Input.GetKeyDown (KeyCode.A)) {
				ships[shipChooser].renderer.enabled = false;
				shipChooser--;
				if (shipChooser < 0) shipChooser = 2;
				ships[shipChooser].renderer.enabled = true;
			}
			if (Input.GetKeyDown (KeyCode.D)) {
				ships[shipChooser].renderer.enabled = false;
				shipChooser++;
				if (shipChooser > 2) shipChooser = 0;
				ships[shipChooser].renderer.enabled = true;
			}

			PlayerShipScript playership = ((PlayerShipScript)ships [shipChooser].GetComponent ("PlayerShipScript"));
			float damage = ((bulletScript)playership.bullet.GetComponent ("bulletScript")).damage;
			hpSlider.value = playership.hp;
			dmgSlider.value = damage;
			speedSlider.value = playership.maxSpeed;
			shipNameText.text = playership.name;
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
			e = Network.InitializeServer (MAX_CONNECTIONS, PORT_NO, !Network.HavePublicAddress());
			
			if (e == NetworkConnectionError.NoError) {
				Debug.Log ("Listening for connections...");
				MasterServer.RegisterHost("SpaceCOG", GameName.text);
				GameNameOverlay.SetActive(false);
				WaitingOverlay.SetActive(true);
				wGameName.text = GameName.text;
				wGameStatus.text = "3 slots still empty...";
				//ConOverlay.SetActive(true);
				//p1Ready.GetComponentsInChildren<UnityEngine.UI.Text>()[0].text = ((PlayerShipScript)ships [shipChooser].GetComponent ("PlayerShipScript")).name;
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
		lastListPoll = Time.time;
	}
	
	public void ConnectServer () {
		Debug.Log ("Attempting to connect to server...");
		NetworkConnectionError e;
		//e = Network.Connect (ConAddress.GetComponent<UnityEngine.UI.Text>().text, PORT_NO);
		
		//if (e != NetworkConnectionError.NoError) {
		//	Debug.Log ("Connection Error: " + e.ToString ());
		//} else {
		//	
		//}
	}
	
	public void GoAnyway () {
		if(Network.isServer) {
			Debug.Log ("Loading game...");
			MasterServer.UnregisterHost();
			networkView.RPC ("Game", RPCMode.All);
		}
	}
	
	public void LeaveLobby () {
		Network.Disconnect();
		WaitingOverlay.SetActive(false);
	}

	public void GameNameCancel () {
		GameNameOverlay.SetActive(false);
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when a server is successfully initialized locally.
	private void OnServerInitialized () {
		wGo.interactable = true;
		Debug.Log ("Server successfully initialized.");
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when a client successfully connects to this server.
	private void OnPlayerConnected (NetworkPlayer player) {
		Debug.Log (player.ToString () + " has joined the server.");
		if (Network.connections.Length == slots) {
			Debug.Log ("Loading game...");
			networkView.RPC ("Game", RPCMode.All);
		} else {
			networkView.RPC ("NewConnected", RPCMode.All, slots - Network.connections.Length);
		}
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when this client successfully connects to a server.
	private void OnConnectedToServer () {
		Debug.Log ("Successfully connected to the server.");
		wGo.interactable = false;
		WaitingOverlay.SetActive(true);
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when this client is disconnected from the server.
	private void OnDisconnectedFromServer (NetworkDisconnection info) {
		Debug.Log ("Disconnected from the server: " + info.ToString ());
		wGameStatus.text = "Host has left the game. Please leave the lobby.";
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when a client disconnects from this server.
	private void OnPlayerDisconnected (NetworkPlayer player) {
		Debug.Log ("Player disconnected.");
		networkView.RPC ("NewConnected", RPCMode.All, slots - Network.connections.Length + 1);
	}
	
	// This function transitions the game into the actual gameplay.
	[RPC] // This function can be called remotely over the network.
	private void NewConnected (int slots) {
		wGameStatus.text = slots + " slots still empty...";
	}
	
	// This function transitions the game into the actual gameplay.
	[RPC] // This function can be called remotely over the network.
	private void Game () {
		PlayerPrefs.SetInt ("ship", shipChooser);
		loadingText.text = "Loading...";
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
