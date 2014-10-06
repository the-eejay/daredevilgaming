using UnityEngine;
using System.Collections;

public class SpawnAsteroid : MonoBehaviour {
	
	public GameObject ObjectToSpawn; 
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 100; i++) {
				Vector3 rndPosWithin;
				rndPosWithin = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f));
				rndPosWithin = transform.TransformPoint (rndPosWithin * .5f);
				Instantiate (ObjectToSpawn, rndPosWithin, Random.rotation);  
				Debug.Log ("spawn asteroids");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
