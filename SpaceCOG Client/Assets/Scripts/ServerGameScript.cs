using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour {

	int pCount = 1 + Network.connections.Length;
	int initCount = 0;
	bool initialized = false;

	float[] lastShotTime = new float[4];
	float[] playerHP = new float[4];
	int[] playerShipChoices = new int[4];
	float[] baddieHP = new float[1000];
	float[] baddieLastShotTime = new float[1000];
	
	// Prefabs
	public GameObject magpiePrefab;
	public GameObject pelicanPrefab;
	public GameObject penguinPrefab;
	GameObject baddiePrefab;
	public GameObject sparrowPrefab;
	public GameObject bomberPrefab;
	GameObject bulletPrefab;
	GameObject enemyBullet;
	
	// Game Objects
	GameObject[] playerShips = new GameObject[4];
	PlayerShipScript[] playerShipScripts = new PlayerShipScript[4];
	GameObject[] baddies = new GameObject[1000];
	enemyScript[] enemyScripts = new enemyScript[1000];
	GameObject[] baddieTargets = new GameObject[1000];
	
	// Client Scripts
	ClientScript[] player = new ClientScript[4];
	private int livingPlayers = 0;
	private int livingEnemies = 0;
	private int waveNumber = 1;
	private int maxWaves = 21;
	private int totalEnemies = 0;

	public float spawnWait = 0.1f;
	public float startWait = 1f;
	public float waveWait = 3f;

	bool bossSpawned = false;

	// Ship stats & attributes
	private const float thrust = 60000f; // Thrust applied to enemies
	private const float angleThrust = 7070f; // Thrust applied to ship moving diagonally.
	private const float bulletForce = 20000f;
	private const float minShotInterval = 0.1f;
	private const float maxSpeed = 20f;

	void Start() {
		// Put initialization stuff into InitializeGame() instead of here
	}
	
	void InitializeGame() {
		Time.timeScale = 1.0f;
		CreatePlayerShips();

		sparrowPrefab = (GameObject) Resources.Load ("Sparrow");
		bomberPrefab = (GameObject) Resources.Load  ("Bomber");
		enemyBullet = (GameObject)Resources.Load ("enemyBullet");

		StartCoroutine (CreateBaddies());
	}
	
	void Update() {
		if (!initialized || !Network.isServer) {
			return;
		}
		//Debug.Log (livingPlayers + " Players left alive");
	}


	public void Damage(GameObject obj, float dmg) {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] == obj) {
				networkView.RPC ("Hurt", RPCMode.All, playerShips[i].networkView.viewID, dmg);
				playerShipScripts[i].hp -= (int) dmg;
				if (playerShipScripts[i].hp < 0f) {
					KillPlayer(i);
					livingPlayers -= 1;
					//Debug.Log ("Destroyed goodie.  Goodies left: " + livingPlayers);
					if (livingPlayers == 0) {
						//Debug.Log ("Gameover 1");
						GameOver();
						Time.timeScale = 0.0f;
					}
					return;
				}
			}
		}
		for (int i = 0; i < baddies.Length; ++i) {
			if (baddies[i] == obj) {
				baddieHP[i] -= dmg;
				if (baddieHP[i] < 0f) {
					Network.Destroy (baddies[i]);
					baddies[i] = null;
					livingEnemies -= 1;
					//Debug.Log ("Baddie destroyed. waveNumber: " + waveNumber + " max waves: " + maxWaves + " living enemies: " + livingEnemies);
					if (livingEnemies == 0 && waveNumber == maxWaves) {
						//Debug.Log ("Gameover 2");
						GameOver();
						Time.timeScale = 0.0f;
					}
					return;
				}
			}
		}
	}
	
	private void GameOver() {
		Network.Disconnect();
	}


	// Spawns waves of asteroids that start flying towards the player.
	IEnumerator CreateBaddies() {
	
		// Wait a short time before we spawn
		yield return new WaitForSeconds (startWait);

		while (waveNumber < maxWaves) {
			// Spawn a wave
			for (int i = 0; i < waveNumber; i++) {
				GameObject target = GetRandomPlayerShip ();
				//Debug.Log ("Target " + target.name);

				if (i % 2 == 1) {
					baddiePrefab = sparrowPrefab;
				} else {
					baddiePrefab = bomberPrefab;
				}

				Vector3 playerPosition = target.transform.position;
				
				// Get a point on the unit circle
				Vector2 randomPointOnCircle = Random.insideUnitCircle;
				randomPointOnCircle.Normalize();
				randomPointOnCircle *= 25f;
				
				float randomX = randomPointOnCircle.x;
				float randomY = randomPointOnCircle.y;
				
				
				Vector3 spawnPosition = new Vector3(playerPosition.x + randomX, playerPosition.y + randomY, 0);

				baddieHP [totalEnemies] = ((enemyScript) baddiePrefab.GetComponent ("enemyScript")).hp;
				GameObject newBaddie = (GameObject) Network.Instantiate (baddiePrefab, spawnPosition, Quaternion.identity, 0);
				if (livingPlayers > 0) {
					((enemyScript) (newBaddie.GetComponent ("enemyScript"))).target = GetRandomPlayerShip ();
				}
				baddies [totalEnemies++] = newBaddie;
				livingEnemies += 1;
				networkView.RPC("UpdateEnemyCount", RPCMode.All, totalEnemies);
				networkView.RPC("ServerSendBaddieRef", RPCMode.All, newBaddie.networkView.viewID);
				yield return new WaitForSeconds (spawnWait);
			}
			// Increase the number of asteroids for next time.
			waveNumber += 1;
			networkView.RPC("NextWave", RPCMode.All);

			// Wait a short time before spawning the next wave
			yield return new WaitForSeconds (waveWait);
		}
	}

	public void KillPlayer(int i) {
		networkView.RPC ("Kill", RPCMode.All, playerShips[i].networkView.viewID);
		Network.Destroy(playerShips[i]);
		
	}

	public void spawnBoss() {
		baddieHP[totalEnemies] = 100f;
		baddiePrefab = (GameObject )Resources.Load ("Sparrow");
		Vector3 spawnPoint = new Vector3((Random.value - 0.5f)*200f, (Random.value - 0.5f)*200f, 0f);
		GameObject newBaddie = (GameObject) Network.Instantiate(baddiePrefab, spawnPoint, Quaternion.identity, 0);
		baddies [totalEnemies++] = newBaddie;
		networkView.RPC("UpdateEnemyCount", RPCMode.All, totalEnemies);
		networkView.RPC("ServerSendBaddieRef", RPCMode.All, newBaddie.networkView.viewID);
	}

	public void Move () {
		// Update logic here
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] != null) {
				bool w = (player[i].w && !player[i].s) ? true : false;
				bool a = (player[i].a && !player[i].d) ? true : false;
				bool s = (player[i].s && !player[i].w) ? true : false;
				bool d = (player[i].d && !player[i].a) ? true : false;
				Vector3 dir = new Vector3(0, 0, 0);
				if (w)
					dir = dir + Vector3.up;
				if (a)
					dir = dir + Vector3.left;
				if (s)
					dir = dir + Vector3.down;
				if (d)
					dir = dir + Vector3.right;
				dir.Normalize();
				playerShips[i].rigidbody.AddForce(dir * playerShipScripts[i].thrust);
				playerShips[i].rigidbody.velocity = Vector3.ClampMagnitude(playerShips[i].rigidbody.velocity, playerShipScripts[i].maxSpeed);
			}
		}
	}
	
	void Turn () {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] != null) {
				Vector3 diff = player[i].cursor;
				diff.Normalize();
				float rot = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
				rot -= 90f;
				playerShips[i].transform.rotation = Quaternion.Euler (0f, 0f, rot);
			}
		}
	}

	public void Shoot() {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] != null) {
				if (player[i].mb1) {
					float tmpTime = Time.time;
					if (tmpTime - lastShotTime[i] > minShotInterval) {
						lastShotTime[i] = tmpTime;
						Rigidbody ship = playerShips[i].rigidbody;
						GameObject tmp = (GameObject) Network.Instantiate (playerShipScripts[i].bullet, ship.transform.position, ship.transform.rotation, 0);
						tmp.rigidbody.velocity = ship.transform.rigidbody.velocity + ship.transform.up * 40f;
						Physics.IgnoreCollision(ship.collider, tmp.collider, true);
						tmp.collider.enabled = true;
					}
				}
			}
		}
	}

	void MoveBaddies() {
		for (int i = 0; i < baddies.Length; ++i) {
			if (baddies[i] != null) {
				if (baddieTargets[i] == null) {
					baddieTargets[i] = GetRandomPlayerShip();
				}
				enemyScript enemy = ((enemyScript) baddies[i].GetComponent ("enemyScript"));
				
				Vector3 diff = baddieTargets[i].transform.position - baddies[i].transform.position;
				Vector3 dir = baddieTargets[i].transform.position - baddies[i].transform.position;
				dir.Normalize ();
				baddies[i].rigidbody.velocity = diff.normalized * enemy.speed;
				
				float rot = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
				rot -= 90f;
				baddies[i].transform.rotation = Quaternion.Euler (0f, 0f, rot);
				
				if (diff.magnitude < 25f && enemy.canShoot) {
					float tmpTime = Time.time;
					if (tmpTime - baddieLastShotTime[i] > enemy.secondsPerShot) {
						baddieLastShotTime[i] = tmpTime;
						Rigidbody ship = baddies[i].rigidbody;
						GameObject tmp = (GameObject) Network.Instantiate (enemyBullet, ship.transform.position, Quaternion.identity, 0);
						tmp.collider.enabled = true;
						Physics.IgnoreCollision(ship.collider, tmp.collider, true);
						tmp.transform.position = ship.transform.position;
						tmp.transform.rotation = ship.transform.rotation;
						tmp.rigidbody.velocity = ship.transform.rigidbody.velocity;
						tmp.rigidbody.AddForce(ship.transform.up * bulletForce);
					}
				}
			}
		}
	}
	
	void FixedUpdate() {
		if (!initialized || !Network.isServer) {
			return;
		}
		Move ();
		Turn ();
		Shoot ();
		MoveBaddies ();
	}
	
	GameObject GetRandomPlayerShip() {
		int s = Random.Range (0, pCount);
		if (playerShips [s]) return playerShips [s];
		return GetRandomPlayerShip ();
	}

	
	void CreatePlayerShips() {
		magpiePrefab = (GameObject) Resources.Load("Magpie");
		pelicanPrefab = (GameObject)Resources.Load ("Pelican");
		penguinPrefab = (GameObject)Resources.Load ("penguin");
		GameObject[] prefabs = new GameObject[]{magpiePrefab, pelicanPrefab, penguinPrefab};
		bulletPrefab = (GameObject)Resources.Load ("prefabBullet");
		for (int i = 0; i < pCount; ++i) {
			playerShips[i] = (GameObject) Network.Instantiate(prefabs[playerShipChoices[i]], new Vector3 ( -5f + 10 * (i % 2), -5f + 10 * (i / 2), 0f), Quaternion.identity, 0);
			playerShipScripts[i] = (PlayerShipScript) playerShips[i].GetComponent ("PlayerShipScript");
			playerHP[i] = playerShipScripts[i].hp;
			livingPlayers += 1;
		}
	}
	
	[RPC]
	public void LocatePlayerScript(NetworkPlayer owner, NetworkViewID pScript, int shipChoice) {
		if (pScript.isMine) {
			ClientScript cs = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent("ClientScript");
			Debug.Log (0 + " Found client script " + cs);
			player[0] = cs;
			playerShipChoices[0] = shipChoice;
			initCount++;
		} else {
			for (int i = 1; i < pCount; ++i) {
				if (Network.connections[i-1] == owner) {
					player[i] = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent(typeof(ClientScript));
					Debug.Log (i + " Found client script");
					playerShipChoices[i] = shipChoice;
					initCount++;
					break;
				}
			}
		}
		
		
		if (initCount == pCount) {
			for(int i = 0; i < 4; ++i) {
				if (player[i] != null)
					Debug.Log("WOOGA: " + player[i].networkView.viewID);
			}
			InitializeGame();
			initialized = true;
			((LocalGameScript)gameObject.GetComponent("LocalGameScript")).ServerSuccessfullyInitialized(playerShips[0].networkView.viewID, pCount);
			for (int i = 1; i < pCount; ++i) {
				//Debug.Log ("Sending initialise to " + playerShips[i].networkView.viewID);
				networkView.RPC("ServerSuccessfullyInitialized", Network.connections[i - 1], playerShips[i].networkView.viewID, pCount);
			}
			for (int i = 0; i < pCount; ++i) {
				networkView.RPC("ServerSendAllyRef", RPCMode.All, playerShips[i].networkView.viewID);
			}
			networkView.RPC ("Initialize", RPCMode.All);
			//Debug.Log ("Initialisation complete");
		}
	}



	public void CleanUp() {
		//GameObject[] playerShips = new GameObject[4];
		//PlayerShipScript[] playerShipScripts = new PlayerShipScript[4];
		//GameObject[] baddies = new GameObject[1000];
		//enemyScript[] enemyScripts = new enemyScript[1000];
		//GameObject[] baddieTargets = new GameObject[1000];
		foreach (GameObject ship in playerShips) {
			Network.Destroy (ship);
		}
		foreach (PlayerShipScript script in playerShipScripts) {
			Destroy (script);
		}
		foreach (GameObject enemy in baddies) {
			Network.Destroy (enemy);
		}
		foreach (enemyScript script in enemyScripts) {
			Destroy (script);
		}
		foreach (GameObject ship in baddieTargets) {
			Destroy (ship);
		}
	}
}