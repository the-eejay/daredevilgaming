﻿using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

	float motionSpeed = 100.0f;
	float m45Speed = 70.7f;
	float bulletSpeed = 10000.0f;
	float force = 100.0f;
	float angleForce = 70.7f;

	GameObject bullet;
	Plane targettingPlane;

	// Use this for initialization
	void Start () {
		bullet = (GameObject)Resources.Load ("prefabBullet");
		targettingPlane = new Plane (new Vector3(0,0,1), new Vector3(0,0,0));
	}
	
	// Update is called once per frame
	void Update () {
		Shoot ();
	}

	void FixedUpdate() {
		Move ();
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
	}

	void Shoot() {
		if (Input.GetMouseButtonDown(0)) {

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float dist = 2000.0f;
			if (targettingPlane.Raycast(ray, out dist)) {
				Vector3 targetCoordinates = ray.GetPoint(dist);
				Vector3 vel = targetCoordinates - this.transform.position;
				vel.Normalize();
				vel = vel * bulletSpeed;
				GameObject tmp = (GameObject)Instantiate(bullet);
				Physics.IgnoreCollision(this.collider, tmp.collider, true);
				tmp.transform.position = this.transform.position;
				tmp.rigidbody.AddForce(vel);
			}
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

		float dt = Time.deltaTime;

		if (w && a) {
			this.rigidbody.AddForce(new Vector3(-angleForce, angleForce, 0));
		} else if (w && d) {
			this.rigidbody.AddForce(new Vector3(angleForce, angleForce, 0));
		} else if (w) {
			this.rigidbody.AddForce(new Vector3(0, force, 0));
		} else if (a && s) {
			this.rigidbody.AddForce(new Vector3(-angleForce, -angleForce, 0));
		} else if (a) {
			this.rigidbody.AddForce(new Vector3(-force, 0, 0));
		} else if (s && d) {
			this.rigidbody.AddForce(new Vector3(angleForce, -angleForce, 0));
		} else if (s) {
			this.rigidbody.AddForce(new Vector3(0, -force, 0));
		} else if (d) {
			this.rigidbody.AddForce(new Vector3(force, 0, 0));
		}

		Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -100);
	}

}