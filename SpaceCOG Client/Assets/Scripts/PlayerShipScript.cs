using UnityEngine;
using System.Collections;

public class PlayerShipScript : MonoBehaviour {

	public static GameObject master;

	public int hp;
	public GameObject bullet;
	public float thrust;
	public float maxSpeed;


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
		//Display the money. It generates 1 money in second (In unity time)
		currency = PlayerPrefs.GetFloat("Money");
		currency += Time.deltaTime;
		PlayerPrefs.SetFloat("Money", currency);
		PlayerPrefs.Save();
	}
	
	void FixedUpdate () {
	
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo SystemInfo) {
		stream.Serialize (ref hp);
	}
	
}
