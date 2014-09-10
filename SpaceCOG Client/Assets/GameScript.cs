using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	GameObject shipPrefab;
	GameObject ship;
	GameObject bullet;
	
	float force = 10000.0f;
	float angleForce = 7070.0f;
	float bulletSpeed = 1000.0f;
	
	Plane targettingPlane;

	// Use this for initialization
	void Start () {
		targettingPlane = new Plane (new Vector3(0,0,1), new Vector3(0,0,0));
		bullet = (GameObject)Resources.Load ("prefabBullet");
		// Spawn player ship
		Debug.Log ("Game Loaded.");
		shipPrefab = (GameObject) Resources.Load ("Magpie");
		ship = (GameObject) Network.Instantiate (shipPrefab, new Vector3 (-2f, 0f, 0f), new Quaternion (0f, 0f, 0f, 0f), 0);
		
		// Launch master host script
		if (Network.isServer) {
			gameObject.AddComponent ("HostScript");
		}
	}
	
	// Update is called once per frame
	void Update () {
		Shoot ();
	}
	
	void FixedUpdate () {
		Move ();
		Turn ();
	}
	
	[RPC]
	public void SetInitialLocation (float x, float y) {
		Debug.Log ("RPC WORKING!!!");
		ship.transform.localPosition = new Vector3 (x, y, 0f);
	}
	
	void Turn () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist = 2000.0f;
		if (targettingPlane.Raycast(ray, out dist)) {
			Vector3 targetCoordinates = ray.GetPoint(dist);
			ship.transform.LookAt (targetCoordinates);
		}
	}
	
	void Shoot() {
		if (Input.GetMouseButtonDown(0)) {
			GameObject tmp = (GameObject) Network.Instantiate (bullet, ship.transform.position, Quaternion.identity, 0);
			Physics.IgnoreCollision(ship.collider, tmp.collider, true);
			tmp.collider.enabled = true;
			tmp.transform.position = ship.transform.position;
			tmp.transform.rigidbody.velocity = ship.transform.rigidbody.velocity;
			tmp.rigidbody.AddForce(ship.transform.forward * bulletSpeed);
		}
	}
	
	void Move () {
		
		bool w = false;
		bool a = false;
		bool s = false;
		bool d = false;
		
		if (Input.GetKey ("w")) {
			w = true;
		}
		if (Input.GetKey ("a")) {
			a = true;
		}
		if (Input.GetKey ("s")) {
			s = true;
		}
		if (Input.GetKey ("d")) {
			d = true;
		}
		
		if (w && s) {
			w = false;
			s = false;
		}
		
		if (a && d) {
			a = false;
			d = false;
		}
		
		if (w && a) {
			ship.rigidbody.AddForce(new Vector3(-angleForce, angleForce, 0));
		} else if (w && d) {
			ship.rigidbody.AddForce(new Vector3(angleForce, angleForce, 0));
		} else if (w) {
			ship.rigidbody.AddForce(new Vector3(0, force, 0));
		} else if (a && s) {
			ship.rigidbody.AddForce(new Vector3(-angleForce, -angleForce, 0));
		} else if (a) {
			ship.rigidbody.AddForce(new Vector3(-force, 0, 0));
		} else if (s && d) {
			ship.rigidbody.AddForce(new Vector3(angleForce, -angleForce, 0));
		} else if (s) {
			ship.rigidbody.AddForce(new Vector3(0, -force, 0));
		} else if (d) {
			ship.rigidbody.AddForce(new Vector3(force, 0, 0));
		}

		Camera.main.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, -10);
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
	
	private void EndGame () {
		Application.LoadLevel ("Menu");
	}

}
