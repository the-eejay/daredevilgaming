using UnityEngine;
using System.Collections;

public class BomberScript : enemyScript {

	public int collisionDamage;
	public int selfDamage = 5;
	
	void OnCollisionEnter (Collision col) {
		if(networkView.isMine && col.gameObject.tag == "Player") {
			// Damage player on collision
			((ServerGameScript) master.GetComponent("ServerGameScript")).Damage(col.gameObject, collisionDamage);
			((ServerGameScript) master.GetComponent("ServerGameScript")).Damage(this.gameObject, selfDamage);
		}
	}
	
}
