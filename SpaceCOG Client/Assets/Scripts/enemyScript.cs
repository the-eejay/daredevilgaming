using UnityEngine;
using System.Collections;

public class enemyScript : MonoBehaviour {

	public int bounty;
	public bool canShoot;
	public int speed;
	public GameObject bullet;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Move ();
		Shoot();
	}

	void Move() {
		GameObject target = GameObject.FindGameObjectWithTag ("Player");
		Vector3 targetPos = target.rigidbody.position;
		rigidbody.velocity = (targetPos - this.transform.position).normalized * speed;

		Vector3 diff = target.transform.position - this.transform.position;
		Vector3 dir = target.transform.position - this.transform.position;
		dir.Normalize ();
		
		float rot = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		rot -= 90f;
		this.transform.rotation = Quaternion.Euler (0f, 0f, rot);
	}

	void Shoot() {
		if (canShoot) {
			float rot = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			rot -= 90f;
			baddies[i].transform.rotation = Quaternion.Euler (0f, 0f, rot);
		}
	}


}
