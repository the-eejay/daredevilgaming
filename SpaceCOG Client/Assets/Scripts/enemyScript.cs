using UnityEngine;
using System.Collections;

public class enemyScript : MonoBehaviour {

	public int bounty;
	public bool canShoot;
	public int speed;
	public float hp;
	public GameObject bullet;
	public float secondsPerShot;
	public AudioClip explosion;

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

	public void OnDestroy() {
		// Play explosion audio on death
		transform.position = new Vector3 (0, 0, 0);
		AudioSource.PlayClipAtPoint(explosion, transform.position, 0.25f);
	}
}
