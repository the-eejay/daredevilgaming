using UnityEngine;
using System.Collections;

public class ServerGameScript : MonoBehaviour {

	// Parameters for wave spawning
	public GameObject hazard;
	public int spawnDist;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	int pCount = 1 + Network.connections.Length;
	int initCount = 0;
	bool initialized = false;
	
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

	public void Move () {
		// Update logic here
		for (int i = 0; i < pCount - 1; ++i) {
			if (player[i].w && player[i].a)
				playerShips[i].rigidbody.AddForce(new Vector3(-angleThrust, angleThrust, 0));
			else if (player[i].w && player[i].d)
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

	public void Shoot() {
		for (int i = 0; i < pCount - 1; ++i) {
			if (player[i].mb1) {
				Rigidbody ship = player[i].rigidbody;
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
	
	void FixedUpdate() {
		if (!Network.isServer) {
			return;
		}
		Move ();
		Shoot ();
	}
	
	void CreatePlayerShips() {
		shipPrefab = (GameObject) Resources.Load("Magpie");
		bulletPrefab = (GameObject)Resources.Load ("prefabBullet");
		playerShips[0] = (GameObject) Network.Instantiate(shipPrefab, new Vector3 (-5f, -5f, 0f), Quaternion.identity, 0);
		for (int i = 1; i < pCount; ++i) {
			playerShips[i] = (GameObject) Network.Instantiate(shipPrefab, new Vector3 ( -5f + 10 * (i % 2), -5f + 10 * (i / 2), 0f), Quaternion.identity, 0);
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
			((LocalGameScript)gameObject.GetComponent("LocalGameScript")).ServerSuccessfullyInitialized(playerShips[0].networkView.viewID);
			for (int i = 1; i < pCount; ++i) {
				networkView.RPC("ServerSuccessfullyInitialized", Network.connections[i - 1], playerShips[i].networkView.viewID);
			}
		}
	}

	IEnumerator SpawnWaves() {
		// Wait a short time before we spawn
		yield return new WaitForSeconds (startWait);

		while (true) {
				// Spawn a wave
			for (int i = 0; i < hazardCount; i++) {
				for (int j = 0; j < pCount - 1; j++) {
					Vector3 playerPosition = player[i].rigidbody.transform.position;
	
					// Get a point on the unit circle
					Vector2 randomPointOnCircle = Random.insideUnitCircle;
					randomPointOnCircle.Normalize ();
					randomPointOnCircle *= spawnDist;
	
					float randomX = randomPointOnCircle.x;
					float randomY = randomPointOnCircle.y;
	
	
					Vector3 spawnPosition = new Vector3 (playerPosition.x + randomX, playerPosition.y + randomY, 0);
					Quaternion spawnRotation = Quaternion.identity;
					Network.Instantiate (hazard, spawnPosition, spawnRotation, 0);
					yield return new WaitForSeconds (spawnWait);
				}
			}
			// Increase the number of asteroids for next time.
			//hazardCount += 1;

			// Wait a short time before spawning the next wave
			yield return new WaitForSeconds (waveWait);
		}
	}
}