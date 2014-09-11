using UnityEngine;
using System.Collections;

/*	GameScript dictates all of the major game functionality. */
public class GameScript : MonoBehaviour {

	public GameObject hazard;
	public int spawnDist;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	// GameObject Prefabs for cloning purposes.
	private static GameObject shipPrefab;
	private static GameObject bullet;
	private static GameObject asteroid;
	
	// A specific reference to the player's ship.
	private GameObject ship;
	
	// Misc mathematical constructs
	private static Plane targettingPlane; // Used for projecting cursor coordinates to 3D space.
	
	// Ship stats & attributes
	private const float thrust = 10000f; // Thrust applied to ship moving along axis.
	private const float angleThrust = 7070f; // Thrust applied to ship moving diagonally.
	
	// Weapon stats & attributed
	private const float bulletForce = 5000f; // Force applied to bullet when fired.


	
	private int score;

	// Use this for initialization
	void Start () {
		// Simulate a network if this scene was loaded directly in the simulator.
		if (Network.connections.Length == 0) {
			Debug.Log ("Loading scene in singleplayer mode...");
			Network.InitializeServer (0, 0, false);
		}
		
		// Object instantiation
		targettingPlane = new Plane (Vector3.forward, Vector3.zero);
		
		// Load prefabs
		shipPrefab = (GameObject) Resources.Load ("Magpie");
		bullet = (GameObject) Resources.Load ("prefabBullet");
		asteroid = (GameObject)Resources.Load ("pentagon_asteroid");
		
		// Spawn player ship
		Debug.Log ("Game Loaded.");
		ship = (GameObject) Network.Instantiate (shipPrefab, new Vector3 (-5f, 0f, 0f), Quaternion.identity, 0);
		
		// Launch master host script (if acting as host)
		if (Network.isServer) {
			gameObject.AddComponent ("HostScript");
		}
		StartCoroutine (SpawnWaves());
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		try {
			Shoot ();
			CentreCamera ();
		} catch (MissingReferenceException e) {
			Debug.Log (e.ToString ());
			// Unity runs many scripts and function calls asynchronously. It's probable that this function is
			// executing at the time when the server tells the game to end. This causes nonfatal MissingReferenceExceptions.
			// This catch is designed to prevent these exceptions.
		}
	}
	
	// FixedUpdate is called at consistent time intervals.
	// It is useful for physics effecting functionality, since
	// the player's framerate won't impact the results.
	void FixedUpdate () {
		try {
			Move ();
			// Turn ();
		} catch (MissingReferenceException e) {
			Debug.Log (e.ToString ());
			// Unity runs many scripts and function calls asynchronously. It's probable that this function is
			// executing at the time when the server tells the game to end. This causes nonfatal MissingReferenceExceptions.
			// This catch is designed to prevent these exceptions.
		}
	}
	
	// SetInitialLocation allows the server to set the location of this player's ship.
	[RPC] // This function can be called remotely over the network.
	public void SetInitialLocation (float x, float y) {
		ship.transform.localPosition = new Vector3 (x, y, 0f);
	}

	[RPC]
	public void SpawnAsteroid () {
		Vector3 currentPos = ship.transform.localPosition;
		currentPos.x += 5;
		Network.Instantiate (asteroid, currentPos, Quaternion.identity, 0);
	}
	
	// Turn to be called during FixedUpdate in order to orient the player's ship towards
	// the player's cursor. Currently moves instantaneously, but the aim is to include
	// turn-rates and torque / angular momentum to limit the rapidness of a turn.
	void Turn () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist = 2000f;
		if (targettingPlane.Raycast(ray, out dist)) {
			Vector3 targetCoordinates = ray.GetPoint(dist);
			ship.transform.LookAt (targetCoordinates);
		}
	}
	
	// Shoot to be called during Update in order to spawn a bullet whenever the player
	// clicks the left mouse button. Eventually this function will need to change to
	// allow more advanced weapon types, and may need to be moved to FixedUpdate instead.
	void Shoot() {
		if (Input.GetMouseButtonDown(0)) {
			GameObject tmp = (GameObject) Network.Instantiate (bullet, ship.transform.position, Quaternion.identity, 0);
			tmp.collider.enabled = true;
			Physics.IgnoreCollision(ship.collider, tmp.collider, true);
			tmp.transform.position = ship.transform.position;
			tmp.transform.rigidbody.velocity = ship.transform.rigidbody.velocity;
			tmp.rigidbody.AddForce(ship.transform.up * bulletForce);
		}
	}
	
	// Move to be called during FixedUpdate in order to control the player's ship.
	// Uses force and momentum to move.
	void Move () {
		bool w = Input.GetKey ("w");
		bool a = Input.GetKey ("a");
		bool s = Input.GetKey ("s");
		bool d = Input.GetKey ("d");
		
		// Cancel out opposing directions
		if (w && s) {
			w = false;
			s = false;
		}
		if (a && d) {
			a = false;
			d = false;
		}
		
		// Apply thrust
		if (w && a)
			ship.rigidbody.AddForce(new Vector3(-angleThrust, angleThrust, 0));
		else if (w && d)
			ship.rigidbody.AddForce(new Vector3(angleThrust, angleThrust, 0));
		else if (w)
			ship.rigidbody.AddForce(thrust * Vector3.up);
		else if (a && s)
			ship.rigidbody.AddForce(new Vector3(-angleThrust, -angleThrust, 0));
		else if (a)
			ship.rigidbody.AddForce(thrust * Vector3.left);
		else if (s && d)
			ship.rigidbody.AddForce(new Vector3(angleThrust, -angleThrust, 0));
		else if (s)
			ship.rigidbody.AddForce(thrust * Vector3.down);
		else if (d)
			ship.rigidbody.AddForce(thrust * Vector3.right);
	}
	
	// CentreCamera to be called during Update in order to ensure that the player's
	// ship is always in the middle of the screen.
	private void CentreCamera () {
		Camera.main.transform.position = ship.transform.position - 10 * Vector3.forward;
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when this client is disconnected from the server.
	private void OnDisconnectedFromServer (NetworkDisconnection info) {
		Debug.Log ("Disconnected from the server: " + info.ToString ());
		EndGame ();
	}
	
	// Overriding MonoBehaviour method that is automatically called
	// when a client disconnects from this server.
	private void OnPlayerDisconnected (NetworkPlayer player) {
		Debug.Log ("Player disconnected.");
		EndGame ();
	}
	
	// EndGame quits back to the main menu. 
	private void EndGame () {
		Application.LoadLevel ("Menu");
	}

	IEnumerator SpawnWaves() {
		
		yield return new WaitForSeconds(startWait);
		while (true) {
			for (int i = 0; i < hazardCount; i++) {
				Vector3 playerPosition = ship.transform.position;

				Vector2 randomPointOnCircle = Random.insideUnitCircle;
				randomPointOnCircle.Normalize();
				randomPointOnCircle *= spawnDist;

				float randomX = randomPointOnCircle.x;
				float randomY = randomPointOnCircle.y;

				
				Vector3 spawnPosition = new Vector3(playerPosition.x + randomX, playerPosition.y + randomY, 0);
				Quaternion spawnRotation = Quaternion.identity;
				Network.Instantiate (hazard, spawnPosition, spawnRotation, 0);
				yield return new WaitForSeconds (spawnWait);
			}
			hazardCount += 1;
			yield return new WaitForSeconds(waveWait);
		}
	}
	
	void onGUI() {
		GUI.Label (new Rect (10, 10, 100, 20), "Score: " + score);
	}
	
	public void AddScore(int scoreValue) {
		score += scoreValue;
	}
	
	public int GetScore() {
		return this.score;
	}

	public void kill() {
		Network.Destroy (ship);
	}

	public Vector3 GetPos() {
		return ship.transform.position;
	}
	
	// ServerEndsGame is a remote procedure call that allows the server to tell the
	// client that the game has ended.
	[RPC]
	public void ServerEndsGame () {
		if (Network.connections.Length != 0) {
			Network.Disconnect ();
		}
		EndGame ();
	}
	
	// ReportDeath should be called when the player's ship is destroyed.
	public void ReportDeath () {
		if (Network.peerType != NetworkPeerType.Disconnected) {
			networkView.RPC ("KillPlayer", RPCMode.All, Network.player);
		}
	}
	

}
