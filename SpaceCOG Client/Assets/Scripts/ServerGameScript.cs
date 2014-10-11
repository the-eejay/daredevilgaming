using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour
{

	int pCount = 1 + Network.connections.Length;
	int initCount = 0;
	bool initialized = false;
<<<<<<< HEAD
=======

>>>>>>> origin/master
	float[] lastShotTime = new float[4];
	float[] playerHP = new float[4];
	int[] playerShipChoices = new int[4];
	float[] baddieHP = new float[1000];
	float[] baddieLastShotTime = new float[1000];
<<<<<<< HEAD

=======
	float[] currency = new float[4];
	
>>>>>>> origin/master
	// Prefabs
	public GameObject magpiePrefab;
	public GameObject pelicanPrefab;
	public GameObject penguinPrefab;
	GameObject baddiePrefab;
	public GameObject sparrowPrefab;
	public GameObject bomberPrefab;
	GameObject bulletPrefab;

	// Player Status Prefab
	public GameObject pStatusScriptPrefab;
	private GameObject pStatusScript;
	PlayerStatusScript[] playerStatuses = new PlayerStatusScript[4];

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

<<<<<<< HEAD
		
	// Ship stats & attributes		
	private const float thrust = 60000f; // Thrust applied to ship moving along axis.		
	private const float angleThrust = 7070f; // Thrust applied to ship moving diagonally.		
	private const float bulletForce = 20000f;		
=======
	// Ship stats & attributes
	private const float thrust = 60000f; // Thrust applied to enemies
	private const float angleThrust = 7070f; // Thrust applied to ship moving diagonally.
	private const float bulletForce = 20000f;
>>>>>>> origin/master
	private const float minShotInterval = 0.1f;
	private const float maxSpeed = 20f;

	// Enemy Ship Types list
	private EnemyShipType.Ship[] ships = new EnemyShipType.Ship[4];
	// Power Ups list
	private PowerUp.PowerUpType[] powerups = new PowerUp.PowerUpType[3];

<<<<<<< HEAD
	void Start ()
	{
			// Put initialization stuff into InitializeGame() instead of here
=======
		sparrowPrefab = (GameObject) Resources.Load ("Sparrow");
		bomberPrefab = (GameObject) Resources.Load  ("Bomber");

		StartCoroutine (CreateBaddies());
>>>>>>> origin/master
	}
	

	void InitializeGame ()
	{
		Time.timeScale = 1.0f;
		CreatePlayerShips ();
		CreatePlayerStatusPrefabs ();
		StartCoroutine (CreateBaddies ());
		pStatusScript = (GameObject)Network.Instantiate (pStatusScriptPrefab, Vector3.zero, Quaternion.identity, 0);
//		if (Network.peerType == NetworkPeerType.Server) {
//			((LocalGameScript)gameObject.GetComponent ("LocalGameScript")).LocatePlayerStatusScript (Network.player, pStatusScript.networkView.viewID);
//		} else {
//			networkView.RPC ("LocatePlayerStatusScript", RPCMode.All, Network.player, pStatusScript.networkView.viewID);
//		}
				// Create player status list
		for (int i = 0; i < pCount; ++i) {
			PlayerStatusScript ps = new PlayerStatusScript();
			playerStatuses [i] = ps;		
		}
<<<<<<< HEAD

		// Initialising the basic enemy types
		ships [0] = new EnemyShipType.Ship ("Speedy", 50, 150, 100);
		ships [1] = new EnemyShipType.Ship ("Tanky", 150, 50, 100);
		ships [2] = new EnemyShipType.Ship ("Attacker", 75, 75, 150);
		ships [3] = new EnemyShipType.Ship ("Drone", 10, 150, 150);

		// Initialising power ups
		powerups [0] = new PowerUp.PowerUpType (150, 100, 100); // Health
		powerups [1] = new PowerUp.PowerUpType (100, 150, 100);	// Speed
		powerups [2] = new PowerUp.PowerUpType (100, 100, 150); // Damage
	}
=======
	
		public void KillPlayer (int i)
		{
				networkView.RPC ("Kill", RPCMode.All, playerShips [i].networkView.viewID);
				Network.Destroy (playerShips [i]);
	}

	public void DamagePlayer(int i, float dmg) {
		networkView.RPC ("Hurt", RPCMode.All, dmg, playerShips [i].networkView.viewID);
	}
	
	public void Damage(GameObject obj, float dmg) {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] == obj) {
				DamagePlayer(i, dmg);
				playerHP[i] -= dmg;

				// Willies Edits
				//playerStatuses[i].health -= dmg;
				//((LocalGameScript)gameObject.GetComponent ("LocalGameScript")).UpdatePlayerHealth (playerConnections, playerStatuses[i].health);
>>>>>>> origin/Willies_branch

	void Update ()
	{
		if (!initialized || !Network.isServer) {
			return;
		}
	}

	public void KillPlayer (int i)
	{
		networkView.RPC ("Kill", RPCMode.All, playerShips [i].networkView.viewID);
		Network.Destroy (playerShips [i]);
	}

	public void Damage (GameObject obj, float dmg)
	{
		for (int i = 0; i < pCount; ++i) {
			if (playerShips [i] == obj) {
				playerHP [i] -= dmg;		
					// Willies Edits
				playerStatuses [i].health -= dmg;
				Debug.Log ("Player " + i + " HP : " + playerHP [i] + " OR statusHP : " + playerStatuses [i].health); 
				((LocalGameScript)gameObject.GetComponent ("LocalGameScript")).UpdatePlayerHealth 
					(playerShips [i].networkView.viewID, 	       
					 playerStatuses [i].health);
				if (playerHP [i] <= 0f || playerStatuses [i].health <= 0f) {	
			
					KillPlayer (i);
					livingPlayers -= 1;
					Debug.Log ("Destroyed goodie.  Goodies left: " + livingPlayers);						
				
					if (livingPlayers == 0) {
						networkView.RPC ("GameOver", RPCMode.All);
						Time.timeScale = 0.0f;
					}
					return;
				}
			}
		}
				for (int i = 0; i < baddies.Length; ++i) {
					if (baddies [i] == obj) {
						baddieHP [i] -= dmg;
						if (baddieHP [i] < 0f) {
							Network.Destroy (baddies [i]);
							baddies [i] = null;
							livingEnemies -= 1;
							if (livingEnemies == 0 && waveNumber == maxWaves) {
								if (bossSpawned) {
									networkView.RPC ("GameOver", RPCMode.All);
									Time.timeScale = 0.0f;
								} else {
									bossSpawned = true;
									networkView.RPC ("BossMode", RPCMode.All);
									livingEnemies++;
									spawnBoss ();
							}
						}
						return;
					}
				}
			}
		}

<<<<<<< HEAD
		/*
	public void CreateBaddies() {
		baddies = new GameObject[1];
		baddieTargets = new GameObject[1];
		baddieHP = new float[1];
		baddieLastShotTime = new float[1];
		baddieHP[0] = 100f;
		baddiePrefab = (GameObject)Resources.Load ("Magpie");
		Vector3 spawnPoint = new Vector3((Random.value - 0.5f)*1000f, (Random.value - 0.5f)*1000f, 0f);
		baddies[0] = (GameObject) Network.Instantiate(baddiePrefab, spawnPoint, Quaternion.identity, 0);
		livingEnemies += 1;
	}
	*/

		// Spawns waves of asteroids that start flying towards the player.
		IEnumerator CreateBaddies ()
		{
=======
	// Spawns waves of asteroids that start flying towards the player.
	IEnumerator CreateBaddies() {
>>>>>>> origin/master
	
				// Wait a short time before we spawn
				yield return new WaitForSeconds (startWait);

<<<<<<< HEAD
				while (waveNumber < maxWaves) {
						// Spawn a wave
						for (int i = 0; i < waveNumber; i++) {
								baddieHP [totalEnemies] = 5f;
								baddiePrefab = (GameObject)Resources.Load ("Magpie");
								Vector3 spawnPoint = new Vector3 ((Random.value - 0.5f) * 200f, (Random.value - 0.5f) * 200f, 0f);
								GameObject newBaddie = (GameObject)Network.Instantiate (baddiePrefab, spawnPoint, Quaternion.identity, 0);
								baddies [totalEnemies++] = newBaddie;
								livingEnemies += 1;
								networkView.RPC ("UpdateEnemyCount", RPCMode.All, totalEnemies);
								networkView.RPC ("ServerSendBaddieRef", RPCMode.All, newBaddie.networkView.viewID);
								yield return new WaitForSeconds (spawnWait);
						}
						// Increase the number of asteroids for next time.
						waveNumber += 1;
						networkView.RPC ("NextWave", RPCMode.All);

						// Wait a short time before spawning the next wave
						yield return new WaitForSeconds (waveWait);
				}
		}

		public void spawnBoss ()
		{
				baddieHP [totalEnemies] = 100f;
				baddiePrefab = (GameObject)Resources.Load ("Magpie");
				Vector3 spawnPoint = new Vector3 ((Random.value - 0.5f) * 200f, (Random.value - 0.5f) * 200f, 0f);
				GameObject newBaddie = (GameObject)Network.Instantiate (baddiePrefab, spawnPoint, Quaternion.identity, 0);
=======
		while (waveNumber < maxWaves) {
			// Spawn a wave
			for (int i = 0; i < waveNumber*2; i++) {
				GameObject target = GetRandomPlayerShip ();
				Debug.Log ("Target " + target.name);
				baddieHP [totalEnemies] = 5f;
				if (i % 2 == 1) {
					baddiePrefab = sparrowPrefab;
				} else {
					baddiePrefab = bomberPrefab;
				}
				Vector3 spawnPoint = new Vector3 ((Random.value - 0.5f) * target.rigidbody.position.x, (Random.value - 0.5f) * target.rigidbody.position.y, 0f);
				GameObject newBaddie = (GameObject) Network.Instantiate (baddiePrefab, spawnPoint, Quaternion.identity, 0);
				((enemyScript) (newBaddie.GetComponent ("enemyScript"))).target = GetRandomPlayerShip ();
>>>>>>> origin/master
				baddies [totalEnemies++] = newBaddie;
				networkView.RPC ("UpdateEnemyCount", RPCMode.All, totalEnemies);
				networkView.RPC ("ServerSendBaddieRef", RPCMode.All, newBaddie.networkView.viewID);
		}
<<<<<<< HEAD

		public void Move ()
		{
				// Update logic here
				for (int i = 0; i < pCount; ++i) {
						if (playerShips [i] != null) {
								bool w = (player [i].w && !player [i].s) ? true : false;
								bool a = (player [i].a && !player [i].d) ? true : false;
								bool s = (player [i].s && !player [i].w) ? true : false;
								bool d = (player [i].d && !player [i].a) ? true : false;
								Vector3 dir = new Vector3 (0, 0, 0);
								if (w)
										dir = dir + Vector3.up;
								if (a)
										dir = dir + Vector3.left;
								if (s)
										dir = dir + Vector3.down;
								if (d)
										dir = dir + Vector3.right;
								dir.Normalize ();
								playerShips [i].rigidbody.AddForce (dir * thrust);
								playerShips [i].rigidbody.velocity = Vector3.ClampMagnitude (playerShips [i].rigidbody.velocity, maxSpeed);
						}
				}
=======
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
>>>>>>> origin/master
		}
	
		void Turn ()
		{
				for (int i = 0; i < pCount; ++i) {
						if (playerShips [i] != null) {
								Vector3 diff = player [i].cursor - playerShips [i].transform.position;
								diff.Normalize ();
								float rot = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
								rot -= 90f;
								playerShips [i].transform.rotation = Quaternion.Euler (0f, 0f, rot);
						}
				}
		}

<<<<<<< HEAD
		public void Shoot ()
		{
				for (int i = 0; i < pCount; ++i) {
						if (playerShips [i] != null) {
								if (player [i].mb1) {
										float tmpTime = Time.time;
										if (tmpTime - lastShotTime [i] > minShotInterval) {
												lastShotTime [i] = tmpTime;
												Rigidbody ship = playerShips [i].rigidbody;
												GameObject tmp = (GameObject)Network.Instantiate (bulletPrefab, ship.transform.position, Quaternion.identity, 0);
												tmp.collider.enabled = true;
												Physics.IgnoreCollision (ship.collider, tmp.collider, true);
												tmp.transform.position = ship.transform.position;
												tmp.transform.rotation = ship.transform.rotation;
												tmp.transform.rigidbody.velocity = ship.transform.rigidbody.velocity;
												tmp.rigidbody.AddForce (ship.transform.up * bulletForce);
										}
								}
						}
=======
	public void Shoot() {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] != null) {
				if (player[i].mb1) {
					float tmpTime = Time.time;
					if (tmpTime - lastShotTime[i] > minShotInterval) {
						lastShotTime[i] = tmpTime;
						Rigidbody ship = playerShips[i].rigidbody;
						GameObject tmp = (GameObject) Network.Instantiate (playerShipScripts[i].bullet, ship.transform.position, Quaternion.identity, 0);
						tmp.collider.enabled = true;
						Physics.IgnoreCollision(ship.collider, tmp.collider, true);
						tmp.transform.position = ship.transform.position;
						tmp.transform.rotation = ship.transform.rotation;
						tmp.transform.rigidbody.velocity = ship.transform.rigidbody.velocity;
						tmp.rigidbody.AddForce(ship.transform.up * bulletForce);
					}
>>>>>>> origin/master
				}
		}
	
		void FixedUpdate ()
		{
				if (!initialized || !Network.isServer) {
						return;
				}
				Move ();
				Turn ();
				Shoot ();
				MoveBaddies ();
		}
<<<<<<< HEAD
	
		GameObject GetRandomPlayerShip ()
		{
				int s = (int)Random.value * playerShips.Length;
				for (int i = 0; i < playerShips.Length; ++i) {
						if (playerShips [(i + s) % playerShips.Length] != null) {
								return playerShips [(i + s) % playerShips.Length];
						}
				}
				return null;
		}
	
		void MoveBaddies ()
		{
				for (int i = 0; i < baddies.Length; ++i) {
						if (baddies [i] != null) {
								if (baddieTargets [i] == null) {
										baddieTargets [i] = GetRandomPlayerShip ();
								}
				
								Vector3 diff = baddieTargets [i].transform.position - baddies [i].transform.position;
								Vector3 dir = baddieTargets [i].transform.position - baddies [i].transform.position;
								dir.Normalize ();
								baddies [i].rigidbody.AddForce (dir * (thrust / 10));
								baddies [i].rigidbody.velocity = Vector3.ClampMagnitude (baddies [i].rigidbody.velocity, maxSpeed);
				
								float rot = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
								rot -= 90f;
								baddies [i].transform.rotation = Quaternion.Euler (0f, 0f, rot);
				
								if (diff.magnitude < 25f) {
										float tmpTime = Time.time;
										if (tmpTime - baddieLastShotTime [i] > minShotInterval) {
												baddieLastShotTime [i] = tmpTime;
												Rigidbody ship = baddies [i].rigidbody;
												GameObject tmp = (GameObject)Network.Instantiate (bulletPrefab, ship.transform.position, Quaternion.identity, 0);
												tmp.collider.enabled = true;
												Physics.IgnoreCollision (ship.collider, tmp.collider, true);
												tmp.transform.position = ship.transform.position;
												tmp.transform.rotation = ship.transform.rotation;
												tmp.rigidbody.velocity = ship.transform.rigidbody.velocity;
												tmp.rigidbody.AddForce (ship.transform.up * bulletForce);
										}
								}
						}
				}
=======
		Move ();
		Turn ();
		Shoot ();
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
			currency[i] = playerShipScripts[i].currency;
			livingPlayers += 1;
>>>>>>> origin/master
		}
	
<<<<<<< HEAD
		void CreatePlayerShips ()
		{
				shipPrefab = (GameObject)Resources.Load ("Magpie");
				bulletPrefab = (GameObject)Resources.Load ("prefabBullet");
				for (int i = 0; i < pCount; ++i) {
						playerShips [i] = (GameObject)Network.Instantiate (shipPrefab, new Vector3 (-5f + 10 * (i % 2), -5f + 10 * (i / 2), 0f), Quaternion.identity, 0);
						playerHP [i] = 100f;
						livingPlayers += 1;
						GameObject magpie = GameObject.Find ("Magpie");
						Color color = Color.white;
						if (magpie) {
								color = magpie.renderer.material.color;
								magpie.renderer.enabled = false;
						}
						playerShips [i].renderer.material.color = color;
=======
	[RPC]
	public void LocatePlayerScript(NetworkPlayer owner, NetworkViewID pScript, int shipChoice) {
		if (pScript.isMine) {
			ClientScript cs = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent("ClientScript");
			player[0] = cs;
			playerShipChoices[0] = shipChoice;
			initCount++;
		} else {
			for (int i = 1; i < pCount; ++i) {
				if (Network.connections[i-1] == owner) {
					player[i] = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent(typeof(ClientScript));
					playerShipChoices[i] = shipChoice;
					initCount++;
					break;
>>>>>>> origin/master
				}
		}

		void CreatePlayerStatusPrefabs ()
		{
				pStatusScriptPrefab = (GameObject)Resources.Load ("PlayerStatusScriptObject");

		}

		[RPC]
		public void LocatePlayerScript (NetworkPlayer owner, NetworkViewID pScript)
		{
				if (pScript.isMine) {
						ClientScript cs = (ClientScript)NetworkView.Find (pScript).gameObject.GetComponent ("ClientScript");
						player [0] = cs;
						initCount++;
				} else {
						for (int i = 1; i < pCount; ++i) {
								if (Network.connections [i - 1] == owner) {
										player [i] = (ClientScript)NetworkView.Find (pScript).gameObject.GetComponent (typeof(ClientScript));
										initCount++;
										break;
								}
						}
				}
		
				if (initCount == pCount) {
						InitializeGame ();
						initialized = true;
						((LocalGameScript)gameObject.GetComponent ("LocalGameScript")).ServerSuccessfullyInitialized (playerShips [0].networkView.viewID, pCount);
						for (int i = 1; i < pCount; ++i) {
								networkView.RPC ("ServerSuccessfullyInitialized", Network.connections [i - 1], playerShips [i].networkView.viewID, pCount);
						}
						for (int i = 0; i < pCount; ++i) {
								networkView.RPC ("ServerSendAllyRef", RPCMode.All, playerShips [i].networkView.viewID);
						}
						networkView.RPC ("Initialize", RPCMode.All);
				}
		}
<<<<<<< HEAD

=======
	}

	public void CleanUp() {
		GameObject[] playerShips = new GameObject[4];
		PlayerShipScript[] playerShipScripts = new PlayerShipScript[4];
		GameObject[] baddies = new GameObject[1000];
		enemyScript[] enemyScripts = new enemyScript[1000];
		GameObject[] baddieTargets = new GameObject[1000];
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
>>>>>>> origin/master
}