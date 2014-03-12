using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	private void Awake() {
		if (GameObject.FindGameObjectsWithTag("Music").Length > 1) {
			Destroy(gameObject);
		} else {
			DontDestroyOnLoad(gameObject);
		}
	}
}
