using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
		Vector3 targetPos = GameObject.Find("PlayerSphere").transform.position;
		rigidbody.velocity = (targetPos - this.transform.position).normalized * speed;
	}
}
