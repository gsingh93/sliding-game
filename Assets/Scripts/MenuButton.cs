using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	public enum Button {
		Play, Help
	}
	public Button button;

	public void OnMouseEnter() {
		GetComponent<Animator>().Play("Grow");
		renderer.material.color = Color.red;
	}

	private void OnMouseExit() {
		GetComponent<Animator>().Play("Shrink");
		renderer.material.color = Color.white;
	}

	private void OnMouseDown() {
		switch(button) {
		case Button.Play:
			break;
		case Button.Help:
			break;
		}
	}
}
