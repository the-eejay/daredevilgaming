using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour {

	int pCount = 1 + Network.connections.Length;
	int initCount = 0;
	bool initialized = false;
	
	float[] lastShotTime = new float[4];
	float[] playerHP = new float[4];
	
	// Prefabs
	GameObject shipPrefab;
	GameObject bulletPrefab;
	
	// Game Objects
	GameObject[] playerShips = new GameObject[4];
	
	// Client Scripts
	ClientScript[] player = new ClientScript[4];

	// Ship stats & attributes
	private const float thrust = 10000f; // Thrust applied to ship moving along axis.
	private const float angleThrust = 7070f; // Thrust applied to ship moving diagonally.
	private const float bulletForce = 8000f;
	private const float minShotInterval = 0.1f;

	void Start() {
		// Put initialization stuff into InitializeGame() instead of here
	}
	
	void InitializeGame() {
		CreatePlayerShips();
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
				}
			}
		}
	}

	public void Move () {
		// Update logic here
		for (int i = 0; i < pCount; ++i) {
			if (playerShips[i] != null) {
				if (player[i].w && player[i].a)
					playerShips[i].rigidbody.AddForce(new Vector3(-angleThrust, angleThrust, 0));
				if (player[i].w && player[i].d)
					playerShips[i].rigidbody.AddForce(new Vector3(angleThrust, angleThrust, 0));
				else if (player[i].w)
					playerShips[i].rigidbody.AddForce(thrust * Vector3.up);
				else if (player[i].a && player[i].s)
					playerShips[i].rigidbody.AddForce(new Vector3(-angleThrust, -angleThrust, 0));
				else if (player[i].a)
					playerShips[i].rigidbody.AddForce(thrust * Vector3.left);
				else if (player[i].s && player[i].d)
					playerShips[i].rigidbody.AddForce(new Vector3(angleThrust, -angleThrust, 0));
				else if (player[i].s)
					playerShips[i].rigidbody.AddForce(thrust * Vector3.down);
				else if (player[i].d)
					playerShips[i].rigidbody.AddForce(thrust * Vector3.right);
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
		}
	}
}