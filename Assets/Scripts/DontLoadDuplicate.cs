using UnityEngine;
using System.Collections;

public class DontLoadDuplicate : MonoBehaviour {

	public string objectTag;

	private void Awake() {
		if (GameObject.FindGameObjectsWithTag(objectTag).Length > 1) {
			Destroy(gameObject);
		} else {
			DontDestroyOnLoad(gameObject);
		}
	}
}
