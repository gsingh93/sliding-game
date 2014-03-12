using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}
}
