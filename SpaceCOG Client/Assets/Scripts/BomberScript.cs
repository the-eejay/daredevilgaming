using UnityEngine;
using System.Collections;

public class BomberScript : enemyScript {

	public int collisionDamage;
	
	void OnCollisionEnter (Collision col) {
		if(networkView.isMine && col.gameObject.tag == "Player") {
			((PlayerShipScript) target.GetComponent ("PlayerShipScript")).Damage(collisionDamage);
			Network.Destroy(gameObject);
		}
	}
}
