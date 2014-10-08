using UnityEngine;
using System.Collections;

public class EnemyShipType : MonoBehaviour {

	public class Ship {
		
		public int hpPerc;
		public int speedPerc;
		public int dmgPerc;
		public string shipName;
		
		// Constructs a ship with scaled stat values to the percentages given
		public Ship (string name, int health, int speed, int dmg) {
			shipName = name;
			hpPerc = health;
			speedPerc = speed;
			dmgPerc = dmg;
		}
	}
}
