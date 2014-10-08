using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	public class PowerUpType {
		
		public int hpPerc;
		public int speedPerc;
		public int dmgPerc;
		
		// Constructs a power up that can increase stats by given percentages
		public PowerUpType (int health, int speed, int dmg) {
			hpPerc = health;
			speedPerc = speed;
			dmgPerc = dmg;
		}
	}
}
