using UnityEngine;
using System.Collections;

public class PlayerShipScript : MonoBehaviour {

	public int hp;
	public GameObject bullet;
	public float thrust;
	public float maxSpeed;
<<<<<<< HEAD
	public float currency;
=======
	public string name;
	public string description;
>>>>>>> origin/master

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//Display the money. It generates 1 money in second (In unity time)
		currency = PlayerPrefs.GetFloat("Money");
		currency += Time.deltaTime;
		PlayerPrefs.SetFloat("Money", currency);
		PlayerPrefs.Save();
	}
	
	void FixedUpdate () {
	
	}
	
}
