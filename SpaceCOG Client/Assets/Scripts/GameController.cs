using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject hazard;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	private int score;

	void Start() {
		StartCoroutine(SpawnWaves());
		score = 0;
	}

	IEnumerator SpawnWaves() {

		yield return new WaitForSeconds(startWait);
		while (true) {
			for (int i = 0; i < hazardCount; i++) {
				Vector3 playerPosition = GameObject.Find ("PlayerSphere").transform.position;
				float randomX = Random.Range(-50, 50);
				float randomY = Random.Range(-50, 50);

				if (randomX < 0) {
					randomX = Mathf.Min(randomX, -10);
				} else {
					randomX = Mathf.Max(randomX, 10);
				}

				if (randomY < 0) {
					randomY = Mathf.Min(randomY, -10);
				} else {
					randomY = Mathf.Max(randomY, 10);
				}

				Vector3 spawnPosition = new Vector3(playerPosition.x + randomX, playerPosition.y + randomY, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds(waveWait);
		}
	}

	void onGUI() {
		GUI.Label (new Rect (10, 10, 100, 20), "Score: " + score);
	}

	public void AddScore(int scoreValue) {
		score += scoreValue;
	}

	public int GetScore() {
		return this.score;
	}
}
