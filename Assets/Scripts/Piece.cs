using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {
	void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			transform.rigidbody.velocity = Vector3.left * 10;
		} else if (Input.GetKeyDown(KeyCode.D)) {
			transform.rigidbody.velocity = Vector3.right * 10;
		} else if (Input.GetKeyDown(KeyCode.W)) {
			transform.rigidbody.velocity = Vector3.up * 10;
		} else if (Input.GetKeyDown(KeyCode.S)) {
			transform.rigidbody.velocity = Vector3.down * 10;
		}
	}

	void OnCollisionEnter(Collision collision) {
		//transform.rigidbody.velocity = Vector3.zero;
	}
}
