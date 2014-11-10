using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour {

	// Networking vars
	int pCount = 1 + Network.connections.Length;
	int initCount = 0;
	bool initialized = false;

	// Game vars
	float[] lastShotTime = new float[4];
	float[] playerHP = new float[4];
	int[] playerShipChoices = new int[4];
	float[] baddieHP = new float[1000];
	float[] baddieLastShotTime = new float[1000];

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
	ClientScript[] player = new ClientScript[4];


	void Start() {
		// Put initialization stuff into InitializeGame() instead of here
	}
	
	void InitializeGame() {
		// Set the timescale back to 1
		Time.timeScale = 1.0f;
		CreatePlayerShips();

		// Spawn prefabs
		sparrowPrefab = (GameObject) Resources.Load ("Sparrow");
		bomberPrefab = (GameObject) Resources.Load  ("Bomber");
		enemyBullet = (GameObject)Resources.Load ("enemyBullet");

		// Start spawning bad guys
		StartCoroutine (CreateBaddies());
	}
	
	void Update() {
		// Use FixedUpdate
	}


	public void Damage(GameObject obj, float dmg) {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] == obj) {
				// Player was damaged
				networkView.RPC ("Hurt", RPCMode.All, playerShips[i].networkView.viewID, dmg);
				playerShipScripts[i].hp -= (int) dmg;
				if (playerShipScripts[i].hp < 0f) {
					// Player died
					KillPlayer(i);
					livingPlayers -= 1;
					if (livingPlayers == 0) {
						// All players are dead - send gameover to all
						GameOver();
						// Pause the game
						Time.timeScale = 0.0f;
					}
					return;
				}
			}
		}
		for (int i = 0; i < baddies.Length; ++i) {
			if (baddies[i] == obj) {
				// Bad guy was damaged
				baddieHP[i] -= dmg;
				if (baddieHP[i] < 0f) {
					// Bad guy died
					Network.Destroy (baddies[i]);
					baddies[i] = null;
					livingEnemies -= 1;
					//Debug.Log ("Baddie destroyed. waveNumber: " + waveNumber + " max waves: " + maxWaves + " living enemies: " + livingEnemies);
					if (livingEnemies == 0 && waveNumber == maxWaves) {
						// All enemies are dead - send game over
						//Debug.Log ("Gameover 2");
						GameOver();
						// Pause the game
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


	// Spawns waves of bad guys
	IEnumerator CreateBaddies() {
	
		// Wait a short time before we start spawning
		yield return new WaitForSeconds (startWait);

		while (waveNumber < maxWaves) {
			// Spawn a wave
			for (int i = 0; i < waveNumber; i++) {
				GameObject target = GetRandomPlayerShip ();
				//Debug.Log ("Target " + target.name);

				// Alternate bombers and sparrows
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

				// Extract X and Y
				float randomX = randomPointOnCircle.x;
				float randomY = randomPointOnCircle.y;
				
				// Spawn on a circle around its target
				Vector3 spawnPosition = new Vector3(playerPosition.x + randomX, playerPosition.y + randomY, 0);

				baddieHP [totalEnemies] = ((enemyScript) baddiePrefab.GetComponent ("enemyScript")).hp;
				GameObject newBaddie = (GameObject) Network.Instantiate (baddiePrefab, spawnPosition, Quaternion.identity, 0);
				((enemyScript) (newBaddie.GetComponent ("enemyScript"))).target = target;

				baddies [totalEnemies++] = newBaddie;
				livingEnemies += 1;
				networkView.RPC("UpdateEnemyCount", RPCMode.All, totalEnemies);
				networkView.RPC("ServerSendBaddieRef", RPCMode.All, newBaddie.networkView.viewID);
				yield return new WaitForSeconds (spawnWait);
			}
			// Increase the number of enemies for the next wave.
			waveNumber += 1;
			// Send notification of next wave
			networkView.RPC("NextWave", RPCMode.All);

			// Wait a short time before spawning the next wave
			yield return new WaitForSeconds (waveWait);
		}
	}

	public void KillPlayer(int i) {
		// Send notification to all players of dead player
		networkView.RPC ("Kill", RPCMode.All, playerShips[i].networkView.viewID);
		Network.Destroy(playerShips[i]);
		
	}

	/* Moves players according to their wsad */
	public void Move () {
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
				// Add thrust and velocity according to the player's respective speed
				playerShips[i].rigidbody.AddForce(dir * playerShipScripts[i].thrust);
				playerShips[i].rigidbody.velocity = Vector3.ClampMagnitude(playerShips[i].rigidbody.velocity, playerShipScripts[i].maxSpeed);
			}
		}
	}
	
	void Turn () {
		// Changes a player's direction according to their mouse position
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
		// Shoots according to a player's left mouse button
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] != null) {
				if (player[i].mb1) {
					float tmpTime = Time.time;
					// Limit the rate of fire
					if (tmpTime - lastShotTime[i] > minShotInterval) {
						lastShotTime[i] = tmpTime;
						Rigidbody ship = playerShips[i].rigidbody;

						// Instantiate a bullet
						GameObject tmp = (GameObject) Network.Instantiate (playerShipScripts[i].bullet, ship.transform.position, ship.transform.rotation, 0);
						tmp.rigidbody.velocity = ship.transform.rigidbody.velocity + ship.transform.up * 40f;

						// Ignore collision so a player's shot won't hurt themself.
						Physics.IgnoreCollision(ship.collider, tmp.collider, true);
						tmp.collider.enabled = true;
					}
				}
			}
		}
	}

	void MoveBaddies() {
		// Perform bad guy moves and shots
		for (int i = 0; i < baddies.Length; ++i) {
			if (baddies[i] != null) {
				if (baddieTargets[i] == null) {
					// If their target died, get a new one
					baddieTargets[i] = GetRandomPlayerShip();
				}

				enemyScript enemy = ((enemyScript) baddies[i].GetComponent ("enemyScript"));

				// Fly towards the target
				Vector3 diff = baddieTargets[i].transform.position - baddies[i].transform.position;
				Vector3 dir = baddieTargets[i].transform.position - baddies[i].transform.position;
				dir.Normalize ();
				baddies[i].rigidbody.velocity = diff.normalized * enemy.speed;
				
				float rot = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
				rot -= 90f;
				baddies[i].transform.rotation = Quaternion.Euler (0f, 0f, rot);
				
				if (diff.magnitude < 25f && enemy.canShoot) {
					float tmpTime = Time.time;
					// Limit the rate of fire
					if (tmpTime - baddieLastShotTime[i] > enemy.secondsPerShot) {
						baddieLastShotTime[i] = tmpTime;
						Rigidbody ship = baddies[i].rigidbody;
						GameObject tmp = (GameObject) Network.Instantiate (enemyBullet, ship.transform.position, Quaternion.identity, 0);
						tmp.collider.enabled = true;

						// Ignore collider between the ship and his bullet
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
		// Perform all game logic
		if (!initialized || !Network.isServer) {
			return;
		}
		Move ();
		Turn ();
		Shoot ();
		MoveBaddies ();
	}
	
	GameObject GetRandomPlayerShip() {
		// Used by the bad guys to choose a random target
		int s = Random.Range (0, pCount);
		if (playerShips [s]) return playerShips [s];
		return GetRandomPlayerShip ();
	}

	
	void CreatePlayerShips() {
		// Load prefabs
		magpiePrefab = (GameObject) Resources.Load("Magpie");
		pelicanPrefab = (GameObject)Resources.Load ("Pelican");
		penguinPrefab = (GameObject)Resources.Load ("penguin");
		GameObject[] prefabs = new GameObject[]{magpiePrefab, pelicanPrefab, penguinPrefab};
		bulletPrefab = (GameObject)Resources.Load ("prefabBullet");

		for (int i = 0; i < pCount; ++i) {
			// Spawn players according to their order of connection
			playerShips[i] = (GameObject) Network.Instantiate(prefabs[playerShipChoices[i]], new Vector3 ( -5f + 10 * (i % 2), -5f + 10 * (i / 2), 0f), Quaternion.identity, 0);
			playerShipScripts[i] = (PlayerShipScript) playerShips[i].GetComponent ("PlayerShipScript");
			playerHP[i] = playerShipScripts[i].hp;
			livingPlayers += 1;
		}
	}
	
	[RPC]
	public void LocatePlayerScript(NetworkPlayer owner, NetworkViewID pScript, int shipChoice) {
		// Get all references to client scripts
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
			// Send all necessary info to clients
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
		// Clean all the unneeded objects
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