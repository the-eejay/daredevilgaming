using UnityEngine;
using UnityEngine.UI;
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
	public GameObject GameOverButton;
	public Slider hpBar;
	public Text waveText;
	public Text gameOverText;
	public Text currencyText;
	public Text scoreText;
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
	public int score;
	
	public Vector3 cursor;
	private static Plane targettingPlane;
	
	void Start() {
		// Simulate a network if playing singleplayer
		if (Network.peerType == NetworkPeerType.Disconnected) {
			Network.InitializeServer(0, 0, false);
		}
		Time.timeScale = 1.0f;
		// Launch server script if server
		//if (Network.isServer) {
			gameObject.AddComponent("ServerGameScript");
		//}

		targettingPlane = new Plane (Vector3.forward, Vector3.zero);
		GameOverButton.SetActive (false);

		pScript = (GameObject) Network.Instantiate(pScriptPrefab, Vector3.zero, Quaternion.identity, 0);
		if (Network.peerType == NetworkPeerType.Server) {
			((ServerGameScript)gameObject.GetComponent("ServerGameScript")).LocatePlayerScript(Network.player, pScript.networkView.viewID, PlayerPrefs.GetInt ("ship"));
		} else {
			networkView.RPC("LocatePlayerScript", RPCMode.All, Network.player, pScript.networkView.viewID, PlayerPrefs.GetInt ("ship"));
		}
		gameOverText.text = "";

		/*
		switch (((PlayerShipScript)ship.GetComponent ("PlayerShipScript")).name) {
			case "Magpie": hpBar.maxValue = 100f;
			break;
			case "Pelican": hpBar.maxValue = 75f;
			break;
			case "Penguin" : hpBar.maxValue = 150f;
			break;
		}
		*/

	}
	
	void CentreCamera () {
		Camera.main.transform.position = ship.transform.position - 20 * Vector3.forward;
	}
	
	void Update() {
		if (!initialized) {
			return;
		}
		if (!compassInitialized) {
			InitCompass ();
		}
		if (ship) {
			CentreCamera ();
			UpdateCompass ();

			// 
			hpBar.GetComponentInChildren<Text> ().text = hpBar.value.ToString ();

			waveText.text = "Wave: " + WaveCounter;

			currency = PlayerPrefs.GetFloat ("Money");
			currency += Time.deltaTime;
			PlayerPrefs.SetFloat ("Money", currency);
			currencyText.text = "Cash: $" + currency.ToString ("0");

			Move ();
		}
	}
	
	void Move () {
		if (Application.loadedLevel == 1) {
			// Cursor
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float dist = 2000f;
			if (targettingPlane.Raycast(ray, out dist)) {
				cursor = ray.GetPoint(dist);
			}
			
			Vector3 diff = cursor - ship.transform.position;
			diff.Normalize();
			float rot = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			rot -= 90f;
			ship.transform.rotation = Quaternion.Euler (0f, 0f, rot);
		}
	}
	
	void UpdateCompass() {
		// Allies
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
		// Enemies
		for (int i = 0; i < compassBaddies.Length; ++i) {
			if (compassBaddies[i] == null) {
				if (compassBaddieBeacons[i] != null) {
					Destroy(compassBaddieBeacons[i]);
					//Earn 10 points when player kill enemies
					score = PlayerPrefs.GetInt("Score");
					score += 10;
					PlayerPrefs.SetInt("Score", score);
					scoreText.text = "Score: " + score;

				}
			} else {
				Vector3 dir = compassBaddies[i].transform.position - ship.transform.position;
				dir.Normalize ();
				compassBaddieBeacons[i].transform.localPosition = dir * 80;
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
		Debug.Log ("Server Successfully Initialised: ship = " + ship.ToString ());
		hpBar.value = ((PlayerShipScript)this.ship.GetComponent ("PlayerShipScript")).hp;
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
		//Debug.Log ("Enemy count: " + enemyCount);
		System.Array.Resize (ref compassBaddies, enemies);
		System.Array.Resize (ref compassBaddieBeacons, enemies);
	}
	
	[RPC]
	public void ServerSendBaddieRef(NetworkViewID baddie) {
		//Debug.Log ("Baddie " + x + " " + enemyCount);
		compassBaddies[x] = NetworkView.Find (baddie).gameObject;
		compassBaddieBeacons[x] = (GameObject) Instantiate(CompassBaddieHeadPrefab, Vector3.zero, Quaternion.identity);
		compassBaddieBeacons[x].transform.parent = CompassPanel.transform;
		compassBaddieBeacons[x].GetComponent<UnityEngine.UI.Image>().color = compassBaddies[x].renderer.material.color;
		x++;

	}

	[RPC]
	public void Kill(NetworkViewID ship) {
	Debug.Log ("Someone died...");
		if (NetworkView.Find(ship).gameObject == this.ship) {
			isAlive = false;
			Debug.Log ("It was you!");
		} else {
			Debug.Log ("Someone else!");
		}
	}

	[RPC]
	public void Hurt(NetworkViewID ship, float damage) {
		if (NetworkView.Find (ship).gameObject == this.ship) {
			hpBar.value -= (int) damage;
		}
	}


	
	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		GameOver();
	}
	
	public void GameOver() {
		Debug.Log ("Game Over");
		gameOver = true;

		if (ship != null && ((PlayerShipScript)ship.GetComponent ("PlayerShipScript")).hp <= 0) {
			isAlive = false;
		}
		gameOverText.text = isAlive ? "You win! Congratulations!" : "You lost.  Better luck next time!";
		GameOverButton.SetActive (true);
		Time.timeScale = 0.0f;

		currency = PlayerPrefs.GetFloat("Money");
		currency = 0;
		PlayerPrefs.SetFloat("Money", currency);
		PlayerPrefs.Save();

		score = PlayerPrefs.GetInt("Score");
		score = 0;
		PlayerPrefs.SetInt("Score", score);
		PlayerPrefs.Save();
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




}
