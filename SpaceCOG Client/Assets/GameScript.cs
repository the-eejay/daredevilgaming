using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	GameObject shipPrefab;
	GameObject ship;

	// Use this for initialization
	void Start () {
	
		Debug.Log ("Game Loaded.");
		shipPrefab = (GameObject) Resources.Load ("Magpie");
		ship = (GameObject) Network.Instantiate (shipPrefab, new Vector3 (0, 0, 0), new Quaternion (0, 0, 0, 0), 0);
		
		if (Network.isServer) {
			ship.transform.Translate (-5.0f, 0.0f, 0.0f);
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
