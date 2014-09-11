using UnityEngine;
using System.Collections;

/*	GameScript dictates all of the major game functionality. */
public class GameScript : MonoBehaviour {

	// GameObject Prefabs for cloning purposes.
	private static GameObject shipPrefab;
	private static GameObject bullet;
	
	// A specific reference to the player's ship.
	private GameObject ship;
	
	// Misc mathematical constructs
	private static Plane targettingPlane; // Used for projecting cursor coordinates to 3D space.
	
	// Ship stats & attributes
	private const float thrust = 10000f; // Thrust applied to ship moving along axis.
	private const float angleThrust = 7070f; // Thrust applied to ship moving diagonally.
	
	// Weapon stats & attributed
	private const float bulletForce = 1000f; // Force applied to bullet when fired.

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
		
		// Spawn player ship
		Debug.Log ("Game Loaded.");
		ship = (GameObject) Network.Instantiate (shipPrefab, new Vector3 (-2f, 0f, 0f), Quaternion.identity, 0);
		
		// Launch master host script (if acting as host)
		if (Network.isServer) {
			gameObject.AddComponent ("HostScript");
		}
	}
	
	// Update is called once per frame
	void Update () {
		Shoot ();
	}
	
	// FixedUpdate is called at consistent time intervals.
	// It is useful for physics effecting functionality, since
	// the player's framerate won't impact the results.
	void FixedUpdate () {
		Move ();
		Turn ();
	}
	
	// SetInitialLocation allows the server to set the location of this player's ship.
	[RPC] // This function can be called remotely over the network.
	public void SetInitialLocation (float x, float y) {
		ship.transform.localPosition = new Vector3 (x, y, 0f);
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
			tmp.rigidbody.AddForce(ship.transform.forward * bulletForce);
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

		// Move the player's camera to keep the ship centred.
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

}
