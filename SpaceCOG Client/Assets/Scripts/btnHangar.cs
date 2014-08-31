using UnityEngine;
using System.Collections;

public class btnHangar : MonoBehaviour {

	private float startTime = 0.0f;
	private float animDuration = 0.25f;
	private bool animated = false;
	private float startAngle = 0.0f;
	private float endAngle = -0.2f * Mathf.PI;
	private float Z = 0;
	private float M = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (animated) {
			float currentTime = Time.realtimeSinceStartup;
			if (currentTime - startTime > animDuration) {
				Camera.main.transform.position = new Vector3(M * Mathf.Cos(endAngle), M * Mathf.Sin(endAngle), Z);
				animated = false;
			}
			// Transform
			float dt = (currentTime - startTime) / animDuration;
			float theta = (endAngle - startAngle) * dt + startAngle;
			Camera.main.transform.position = new Vector3(M * Mathf.Cos(theta), M * Mathf.Sin(theta), Z);
		}
	}
	
	public void HangarShift () {
		startAngle = Mathf.Atan2(Camera.main.transform.position.y, Camera.main.transform.position.x);
		Z = Camera.main.transform.position.z;
		M = Mathf.Sqrt(Mathf.Pow(Camera.main.transform.position.x, 2) + Mathf.Pow(Camera.main.transform.position.y, 2));
		startTime = Time.realtimeSinceStartup;
		animated = true;
	}
}
