using UnityEngine;
using System.Collections;

public class enemyScript : MonoBehaviour {

	public int bounty;
	public bool canShoot;
	public int speed;
	public float hp;
	public GameObject bullet;
	public float secondsPerShot;

	public static GameObject master;

	public GameObject target;
	private float lastShot = 0f;
	private const float bulletForce = 20000f;

	// Use this for initialization
	void Start () {

		if(master == null) {
			master = GameObject.Find("WorldScriptObject");
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		if (target) {
			//Move ();
			//Shoot ();
		}
	}
	/*
	void Move() {
		Vector3 targetPos = target.rigidbody.position;
		rigidbody.velocity = (targetPos - this.transform.position).normalized * speed;

		Vector3 dir = target.transform.position - this.transform.position;
		dir.Normalize ();
		
		float rot = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		rot -= 90f;
		this.transform.rotation = Quaternion.Euler (0f, 0f, rot);
	}

	void Shoot() {
		Vector3 diff = target.transform.position - this.transform.position;

		if (canShoot && diff.magnitude < 25f) {
			float tmpTime = Time.time;
			if (tmpTime - lastShot > secondsPerShot) {
				lastShot = tmpTime;
				Rigidbody ship = this.rigidbody;
				GameObject tmp = (GameObject) Network.Instantiate (bullet, ship.transform.position, Quaternion.identity, 0);
				tmp.collider.enabled = true;
				Physics.IgnoreCollision(ship.collider, tmp.collider, true);
				tmp.transform.position = ship.transform.position;
				tmp.transform.rotation = ship.transform.rotation;
				tmp.rigidbody.velocity = ship.transform.rigidbody.velocity;
				tmp.rigidbody.AddForce(ship.transform.up * bulletForce);
			}
		}
	}
	*/
	/*
	public void Damage(float damage) {
		hp -= (int) damage;
		Debug.Log ("Enemy lost" + damage + "hp.  Enemy now at " + hp + "hp");
		if (hp <= 0) {
			Debug.Log ("Enemy destroyed");
			Network.Destroy (gameObject);
		}
	}
	*/
	public void OnDestroy() {
		Debug.Log ("Enemy destroyed");
	}
}
