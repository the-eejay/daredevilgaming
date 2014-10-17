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
	
	public Vector3 cursor;
	private static Plane targettingPlane;


	// Use this for initialization
	void Start () {
		if(master == null) {
			master = GameObject.Find("WorldScriptObject");
		}
		targettingPlane = new Plane (Vector3.forward, Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
		//Display the money. It generates 1 money in second (In unity time)
		currency = PlayerPrefs.GetFloat("Money");
		currency += Time.deltaTime;
		PlayerPrefs.SetFloat("Money", currency);
		PlayerPrefs.Save();
		Move();
	}
	
	void FixedUpdate () {
	
	}
	
	void Move () {
		if (!Network.isServer && Network.isClient) {
			// Cursor
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float dist = 2000f;
			if (targettingPlane.Raycast(ray, out dist)) {
				cursor = ray.GetPoint(dist);
			}
		
			Vector3 diff = cursor - this.transform.position;
			diff.Normalize();
			float rot = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			rot -= 90f;
			this.transform.rotation = Quaternion.Euler (0f, 0f, rot);
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo SystemInfo) {
		stream.Serialize (ref hp);
	}
	
}
