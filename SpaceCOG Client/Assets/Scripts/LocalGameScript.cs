using UnityEngine;
using System.Collections;

public class LocalGameScript : MonoBehaviour {
	// State variables
	private bool initialized = false;
	private bool compassInitialized = false;
	int pCount;
	int enemyCount = 0;
	int WaveCounter = 0;
	int x = 0;
	int y = 0;
	public GameObject pScriptPrefab;
	public GameObject CompassHeadPrefab;
	public GameObject CompassBaddieHeadPrefab;
	public GameObject CompassPanel;
	private GameObject pScript;
	private bool isAlive = true;
	private bool gameOver = false;
	private bool boss = false;
	
	// Object References
	private GameObject ship;
	private GameObject[] compassAllies;
	private GameObject[] compassAllyBeacons;
	private GameObject[] compassBaddies = new GameObject[1000];
	private GameObject[] compassBaddieBeacons = new GameObject[1000];

	private ArrayList enemies = new ArrayList();

	public int final_wave;
	public float currency;
	
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
			((ServerGameScript)gameObject.GetComponent("ServerGameScript")).LocatePlayerScript(Network.player, pScript.networkView.viewID, PlayerPrefs.GetInt ("ship"));
		} else {
			networkView.RPC("LocatePlayerScript", RPCMode.All, Network.player, pScript.networkView.viewID, PlayerPrefs.GetInt ("ship"));
		}

	}
	
	void CentreCamera () {
		if (ship) {
			Camera.main.transform.position = ship.transform.position - 20 * Vector3.forward;
		}
	}
	
	void Update() {
		if (!initialized) {
			return;
		}
		if (!compassInitialized) {
			InitCompass ();
		}
		CentreCamera();
		UpdateCompass();
	}
	
	void UpdateCompass() {
		// Allies
		for (int i = 0; i < pCount - 1; ++i) {
			if (compassAllies[i] == null) {
				if (compassAllyBeacons[i] != null) {
					Destroy(compassAllyBeacons[i]);
				}
			} else {
				if (ship) {
					Vector3 dir = compassAllies[i].transform.position - ship.transform.position;
					dir.Normalize ();
					compassAllyBeacons[i].transform.localPosition = dir * 80;
				}
			}
		}
		// Enemies
		for (int i = 0; i < compassBaddies.Length; ++i) {
			if (compassBaddies[i] == null) {
				if (compassBaddieBeacons[i] != null) {
					Destroy(compassBaddieBeacons[i]);
					//Earn 20 when player kill enemies
					currency = PlayerPrefs.GetFloat("Money");
					currency += 20;
					PlayerPrefs.SetFloat("Money", currency);
					PlayerPrefs.Save();
				}
			} else {
				if (ship) {
					Vector3 dir = compassBaddies[i].transform.position - ship.transform.position;
					dir.Normalize ();
					compassBaddieBeacons[i].transform.localPosition = dir * 80;
				}
			}
		}
	}
	
	void InitCompass() {
		for (int i = 0; i < pCount - 1; ++i) {
			compassAllyBeacons[i] = (GameObject) Instantiate(CompassHeadPrefab, Vector3.zero, Quaternion.identity);
			compassAllyBeacons[i].transform.parent = CompassPanel.transform;
			compassAllyBeacons[i].GetComponent<UnityEngine.UI.Image>().color = compassAllies[i].renderer.material.color;
		}
		compassInitialized = true;
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
			compassAllies[y++] = NetworkView.Find(ship).gameObject;	
		}
	}

	[RPC]
	public void UpdateEnemyCount(int enemies) {
		enemyCount = enemies;
		Debug.Log ("Enemy count: " + enemyCount);
		System.Array.Resize (ref compassBaddies, enemies);
		System.Array.Resize (ref compassBaddieBeacons, enemies);
	}
	
	[RPC]
	public void ServerSendBaddieRef(NetworkViewID baddie) {
		Debug.Log ("Baddie " + x + " " + enemyCount);
		compassBaddies[x] = NetworkView.Find (baddie).gameObject;
		compassBaddieBeacons[x] = (GameObject) Instantiate(CompassBaddieHeadPrefab, Vector3.zero, Quaternion.identity);
		compassBaddieBeacons[x].transform.parent = CompassPanel.transform;
		compassBaddieBeacons[x].GetComponent<UnityEngine.UI.Image>().color = compassBaddies[x].renderer.material.color;
		x++;

	}

	[RPC]
	public void Kill(NetworkViewID ship) {
		if (NetworkView.Find(ship).gameObject == this.ship) {
			isAlive = false;
		}
	}
	[RPC]
	public void GameOver() {
		Debug.Log ("Game Over");
		Destroy(GameObject.Find ("Magpie"));
		gameOver = true;

	}

	[RPC]
	public void NextWave() {
		WaveCounter++;
	}

	[RPC]
	public void BossMode() {
		boss = true;
	}

	[RPC]
	public void Initialize() {
		initialized = true;
	}

	private void OnGUI() {
		//Display the money. It generates 1 money in second (In unity time)
		currency = PlayerPrefs.GetFloat("Money");
		string money_status = "Money: " + currency.ToString("0");
		GUI.Label (new Rect ((Screen.width-(Screen.width / 6)), 50, 100, 30), money_status);

		string waveString = boss ? "Final Boss! " : "Wave " + WaveCounter;
		GUI.Label (new Rect ((Screen.width / 2), 50, 150, 150), waveString);

		final_wave = PlayerPrefs.GetInt("finalWave");
		string waveScore = "The last wave score: " + final_wave.ToString();
		GUI.Label (new Rect ((Screen.width / 2), 70, 130, 150), waveScore);

		if (gameOver) {
			string aliveString = isAlive ? "You win! " : "You lose! ";
			GUI.Label (new Rect ((Screen.width - 150) / 2, (Screen.height - 150) / 2, 300, 300), "Game over. " + aliveString);
			PlayerPrefs.SetInt("finalWave", WaveCounter);
			PlayerPrefs.Save();
			//Set the money 0 when the game is done.
			PlayerPrefs.SetFloat("Money", 0);
			PlayerPrefs.Save();
			if (GUI.Button (new Rect ((Screen.width - 150) / 2, (Screen.height + 100) / 2, 250, 100), "Continue"))
				Application.LoadLevel ("Menu");
		}
	}


}
