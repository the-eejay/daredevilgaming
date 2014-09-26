using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatusScript : MonoBehaviour {

	// Serialized variables? Status Variables
	public float health = 100;

	void Start() {

	}

	void Update () {

	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			UpdateHealth(health);
		}
		stream.Serialize (ref health);
	}

	public void UpdateHealth(float h) {
		health = h;
	}
}
