using UnityEngine;
using System.Collections;

public class enemyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up * Time.deltaTime * 500);
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
	}

	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.name == "prefabBullet(Clone)")
		{
			Destroy(col.gameObject);
			Destroy(gameObject);
			Destroy(this);
		}
	}
}
