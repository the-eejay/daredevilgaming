using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShipScript : MonoBehaviour {

	public static GameObject master;

	public int hp;
	public GameObject bullet;
	public float thrust;
	public float maxSpeed;

	public Text currencyText;
	public float currency;

	public string name;
	public string description;
	
	// Use this for initialization
	void Start () {
		if(master == null) {
			master = GameObject.Find("WorldScriptObject");
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void FixedUpdate () {
	
	}



}
