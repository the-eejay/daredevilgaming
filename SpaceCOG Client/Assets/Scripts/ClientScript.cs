﻿using UnityEngine;
using System.Collections;

public class ClientScript : MonoBehaviour {
	// Serialized Variables
	public bool w = false;
	public bool a = false;
	public bool s = false;
	public bool d = false;
	public bool mb1 = false;
	public Vector3 cursor;
	
	// Mathematical Constructs
	private static Plane targettingPlane;
	
	
	void Start() {
		cursor = new Vector3();
		targettingPlane = new Plane (Vector3.forward, Vector3.zero);
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		UpdateSerializedVars();
		stream.Serialize(ref w);
		stream.Serialize(ref a);
		stream.Serialize(ref s);
		stream.Serialize(ref d);
		stream.Serialize(ref mb1);
		stream.Serialize(ref cursor);
	}
	
	void UpdateSerializedVars() {
		w = Input.GetKey("w");
		a = Input.GetKey("a");
		s = Input.GetKey ("s");
		d = Input.GetKey ("d");

		if (w)
						Debug.Log ("w");
				else
						Debug.Log ("not w");
		
		// Cursor
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist = 2000f;
		if (targettingPlane.Raycast(ray, out dist)) {
			cursor = ray.GetPoint(dist);
		}
	}
}