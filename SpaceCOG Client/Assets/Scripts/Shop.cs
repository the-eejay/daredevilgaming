using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	bool ShopButton = false;

	public int health_level = 1;
	public float damage_level = 1;
	public float speed_level = 1;

	public float Width; 
	public float Height; 
	public float X; 
	public float Y;

	public GameObject players;
	public PlayerShipScript pss;

	public float price1 = 1200;
	public float price2 = 500;
	public float min = 0;
	public float currency;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI () {
		if (GUI.Button (new Rect ((Screen.width - (Screen.width / 8)), 10, 100, 30), "Hangar")) {
			ShopButton = true;
		}

		if (ShopButton) {
			DisplayShop();
		}
	}

	void DisplayShop() {
		GUI.Box (new Rect(166, 117, 600, 380), "Hangar");

		//Health upgrade
		GUI.Label (new Rect(256, 191, 20, 20), "HP");
		GUI.Label (new Rect(320, 191, 32, 23), "Level");
		GUI.Label (new Rect(362, 191, 20, 20), health_level.ToString());

		if (GUI.Button (new Rect (455, 191, 80, 27), "Upgrade")) {
			DoMoney();
			//pss = (PlayerShipScript)players.GetComponent(typeof(PlayerShipScript));
			//Debug.Log(pss.hp);
		}

		GUI.Label (new Rect(547, 191, 150, 37), "Cost: 1200");

		//Damage
		GUI.Label (new Rect(256, 250, 60, 25), "Damage");
		GUI.Label (new Rect(320, 250, 32, 23), "Level");
		GUI.Label (new Rect(362, 250, 20, 20), damage_level.ToString("0"));

		if (GUI.Button (new Rect (455, 250, 80, 27), "Upgrade")) {
			DoMoney1();
		}

		GUI.Label (new Rect(547, 250, 150, 37), "Cost: 500");

		//Speed
		GUI.Label (new Rect(256, 311, 50, 20), "Speed");
		GUI.Label (new Rect(320, 311, 32, 23), "Level");
		GUI.Label (new Rect(362, 311, 20, 20), speed_level.ToString("0"));

		if (GUI.Button (new Rect (455, 311, 80, 27), "Upgrade")) {
			DoMoney1();
		}

		GUI.Label (new Rect (547, 311, 150, 37), "Cost: 500");

		if (GUI.Button (new Rect (678, 450, 78, 36), "Close")) {
			ShopButton = false;
		}
	}

	void DoMoney(){
		currency = PlayerPrefs.GetFloat("Money");
		if (currency < min) {
			currency = min;
		} else if (currency > price1) {
			currency -= price1;
		}
		PlayerPrefs.SetFloat("Money", currency);
		PlayerPrefs.Save();
	}

	void DoMoney1(){
		currency = PlayerPrefs.GetFloat("Money");
		if (currency < min) {
			currency = min;
		} else if (currency > price2) {
			currency -= price2;
		}
		PlayerPrefs.SetFloat("Money", currency);
		PlayerPrefs.Save();
	}
}
