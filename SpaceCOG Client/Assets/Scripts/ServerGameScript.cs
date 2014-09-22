using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour {

	int pCount = 1 + Network.connections.Length;
	int initCount = 0;
	bool initialized = false;
	
	float[] lastShotTime = new float[4];
	float[] playerHP = new float[4];
	float[] baddieHP;
	float[] baddieLastShotTime;
	
	// Prefabs
	GameObject shipPrefab;
	GameObject baddiePrefab;
	GameObject bulletPrefab;
	
	// Game Objects
	GameObject[] playerShips = new GameObject[4];
	GameObject[] baddies;
	GameObject[] baddieTargets;
	
	// Client Scripts
	ClientScript[] player = new ClientScript[4];

	// Ship stats & attributes
	private const float thrust = 60000f; // Thrust applied to ship moving along axis.
	private const float angleThrust = 7070f; // Thrust applied to ship moving diagonally.
	private const float bulletForce = 20000f;
	private const float minShotInterval = 0.1f;
	private const float maxSpeed = 20f;

	void Start() {
		// Put initialization stuff into InitializeGame() instead of here
	}
	
	void InitializeGame() {
		CreatePlayerShips();
		CreateBaddies();
	}
	
	void Update() {
		if (!initialized || !Network.isServer) {
			return;
		}

	}
	
	public void KillPlayer(int i) {
		Network.Destroy(playerShips[i]);
		
	}
	
	public void Damage(GameObject obj, float dmg) {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] == obj) {
				playerHP[i] -= dmg;
				if (playerHP[i] < 0f) {
					KillPlayer(i);
					return;
				}
			}
		}
		for (int i = 0; i < baddies.Length; ++i) {
			if (baddies[i] == obj) {
				baddieHP[i] -= dmg;
				if (baddieHP[i] < 0f) {
					Network.Destroy (baddies[i]);
					return;
				}
			}
		}
	}
	
	public void CreateBaddies() {
		baddies = new GameObject[1];
		baddieTargets = new GameObject[1];
		baddieHP = new float[1];
		baddieLastShotTime = new float[1];
		baddieHP[0] = 100f;
		baddiePrefab = (GameObject)Resources.Load ("Magpie");
		Vector3 spawnPoint = new Vector3((Random.value - 0.5f)*1000f, (Random.value - 0.5f)*1000f, 0f);
		baddies[0] = (GameObject) Network.Instantiate(baddiePrefab, spawnPoint, Quaternion.identity, 0);
		
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
				playerShips[i].rigidbody.AddForce(dir * thrust);
				playerShips[i].rigidbody.velocity = Vector3.ClampMagnitude(playerShips[i].rigidbody.velocity, maxSpeed);
			}
		}
	}
	
	void Turn () {
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] != null) {
				Vector3 diff = player[i].cursor - playerShips[i].transform.position;
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
						GameObject tmp = (GameObject) Network.Instantiate (bulletPrefab, ship.transform.position, Quaternion.identity, 0);
						tmp.collider.enabled = true;
						Physics.IgnoreCollision(ship.collider, tmp.collider, true);
						tmp.transform.position = ship.transform.position;
						tmp.transform.rotation = ship.transform.rotation;
						tmp.transform.rigidbody.velocity = ship.transform.rigidbody.velocity;
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
		MoveBaddies();
	}
	
	GameObject GetRandomPlayerShip() {
		int s = (int) Random.value * playerShips.Length;
		for (int i = 0; i < playerShips.Length; ++i) {
			if (playerShips[(i + s) % playerShips.Length] != null) {
				return playerShips[(i + s) % playerShips.Length];
			}
		}
		return null;
	}
	
	void MoveBaddies() {
		for (int i = 0; i < baddies.Length; ++i) {
			if (baddies[i] != null) {
				if (baddieTargets[i] == null) {
					baddieTargets[i] = GetRandomPlayerShip();
				}
				
				Vector3 diff = baddieTargets[i].transform.position - baddies[i].transform.position;
				Vector3 dir = baddieTargets[i].transform.position - baddies[i].transform.position;
				dir.Normalize ();
				baddies[i].rigidbody.AddForce(dir*(thrust/10));
				baddies[i].rigidbody.velocity = Vector3.ClampMagnitude(baddies[i].rigidbody.velocity, maxSpeed);
				
				float rot = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
				rot -= 90f;
				baddies[i].transform.rotation = Quaternion.Euler (0f, 0f, rot);
				
				if (diff.magnitude < 25f) {
					float tmpTime = Time.time;
					if (tmpTime - baddieLastShotTime[i] > minShotInterval) {
						baddieLastShotTime[i] = tmpTime;
						Rigidbody ship = baddies[i].rigidbody;
						GameObject tmp = (GameObject) Network.Instantiate (bulletPrefab, ship.transform.position, Quaternion.identity, 0);
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
	
	void CreatePlayerShips() {
		shipPrefab = (GameObject) Resources.Load("Magpie");
		bulletPrefab = (GameObject)Resources.Load ("prefabBullet");
		for (int i = 0; i < pCount; ++i) {
			playerShips[i] = (GameObject) Network.Instantiate(shipPrefab, new Vector3 ( -5f + 10 * (i % 2), -5f + 10 * (i / 2), 0f), Quaternion.identity, 0);
			playerHP[i] = 100f;
		}
	}
	
	[RPC]
	public void LocatePlayerScript(NetworkPlayer owner, NetworkViewID pScript) {
		if (pScript.isMine) {
			ClientScript cs = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent("ClientScript");
			player[0] = cs;
			initCount++;
		} else {
			for (int i = 1; i < pCount; ++i) {
				if (Network.connections[i-1] == owner) {
					player[i] = (ClientScript) NetworkView.Find(pScript).gameObject.GetComponent(typeof(ClientScript));
					initCount++;
					break;
				}
			}
		}
		
		if (initCount == pCount) {
			InitializeGame();
			initialized = true;
			((LocalGameScript)gameObject.GetComponent("LocalGameScript")).ServerSuccessfullyInitialized(playerShips[0].networkView.viewID, pCount);
			for (int i = 1; i < pCount; ++i) {
				networkView.RPC("ServerSuccessfullyInitialized", Network.connections[i - 1], playerShips[i].networkView.viewID, pCount);
			}
			for (int i = 0; i < pCount; ++i) {
				networkView.RPC("ServerSendAllyRef", RPCMode.All, playerShips[i].networkView.viewID);
			}
			networkView.RPC("ServerSendBaddieCount", RPCMode.All, baddies.Length);
			for (int i = 0; i < baddies.Length; ++i) {
				networkView.RPC("ServerSendBaddieRef", RPCMode.All, baddies[i].networkView.viewID);
			}
		}
	}
}