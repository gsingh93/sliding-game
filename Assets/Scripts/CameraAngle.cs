using UnityEngine;
using System.Collections;

public class CameraAngle : MonoBehaviour {

	void Awake() {
		FlatView();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Z)) {
			IsometricView();
		} else if (Input.GetKeyDown(KeyCode.X)) {
			FlatView();
		}
	}

	void IsometricView() {
		camera.transform.position = new Vector3(10, 10, -10);
		StartCoroutine(rotate(new Vector3(40, 240, 250)));
		//camera.transform.localEulerAngles = new Vector3(40, 240, 250);
	}

	void FlatView() {
		camera.transform.position = new Vector3(0, 0, -10);
		StartCoroutine(rotate(new Vector3(0, 0, 0)));
		//camera.transform.localEulerAngles = new Vector3(0, 180, 0);
	}

	IEnumerator rotate(Vector3 to) {
		float seconds = 0.5f;
		int iterations = 25;
		float waitTime = seconds / iterations;

		Vector3 step = (to - camera.transform.localEulerAngles) / iterations;

		for (int i = 0; i < iterations; i++) {
			camera.transform.localEulerAngles += step;
			yield return new WaitForSeconds(waitTime);
		}

		camera.transform.localEulerAngles = to;
	}
}