using UnityEngine;
using System.Collections;

public class BomberScript : enemyScript {

	public int collisionDamage;
	
	void OnCollisionEnter (Collision col) {
		if(networkView.isMine && col.gameObject.tag == "Player") {

			((ServerGameScript) master.GetComponent("ServerGameScript")).Damage(col.gameObject, collisionDamage);
			((ServerGameScript) master.GetComponent("ServerGameScript")).Damage(this.gameObject, 1);
		}
	}
}
