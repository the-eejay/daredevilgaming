using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthScript : MonoBehaviour {

	public Slider slider;

	private int hp = 100;

	void Update () {
		slider.value = hp;
	}

	public void damageHealth(int dmg) {
		hp -= dmg;
	}
}
