using UnityEngine;
using System.Collections;

public class login : MonoBehaviour {
	//public variables
	public string CurrentMenu = "Login";

	//Static variables
	public static string username = "";
	public static string password = "";

	//private variables
	//private string CreateAccountLink = "";
	private string LoginLink = "http://localhost/unity/LoginAccount.php";

	//GUI variables
	public float X;
	public float Y;
	public float Width;
	public float Height;

	// Use this for initialization
	void Start () {
	
	}
	
	// Main GUI Function
	void OnGUI(){

		if (CurrentMenu == "Login") 
		{
			LoginGUI();
		}
		else if (CurrentMenu == "CreateAccount")
		{
			CreateAccountGUI();
		}

	}

	// GUI for creating the account.
	void CreateAccountGUI() {
		GUI.Box(new Rect(374, 47, 730, 526), "Create Account");
		
		if (GUI.Button (new Rect (504, 503, 160, 27), "Done")) {
			
		}
		if (GUI.Button (new Rect (777,503,160,27), "Cancel")) {
			CurrentMenu = "Login";
		}

	}

	// Main Login GUI
	void LoginGUI() {
		GUI.Box(new Rect(374, 47, 730, 526), "Login");

		if (GUI.Button (new Rect (504, 503, 160, 27), "Create Account")) {
			//CurrentMenu = "CreateAccount";
			//DO Nothing for now
		}
		if (GUI.Button (new Rect (777,503,160,27), "Log In")) {
			StartCoroutine(LoginAccount());
		}
		GUI.Label (new Rect(513, 173, 80, 20), "Username:");
		username = GUI.TextField(new Rect(513, 207, 440, 23), username);

		GUI.Label (new Rect(513, 255, 80, 20), "Password:");
		password = GUI.PasswordField(new Rect(513, 287, 440, 23), password, '*');

	}

	IEnumerator LoginAccount(){
		WWWForm Form = new WWWForm ();
		Form.AddField ("username", username);
		Form.AddField ("password", password);
		WWW LoginAccountWWW = new WWW (LoginLink, Form);
		yield return LoginAccountWWW;

		if (LoginAccountWWW.error == null) {
			if(LoginAccountWWW.text == "Success")
			{
				PlayerPrefs.SetString("username", username);
				Debug.Log("Yes");
				Application.LoadLevel ("MainMenu");
			}
		} else {
			Debug.Log("Error: " + LoginAccountWWW.ToString() + "(Unable to connect)");
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
