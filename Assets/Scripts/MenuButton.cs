using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	public enum Button {
		PlayMulti, PlayComp, Options, Help
	}
	public Button button;

	public void OnMouseEnter() {
		GetComponent<Animator>().Play("Grow");
		renderer.material.color = Color.red;
	}

	// TODO: Not always called
	private void OnMouseExit() {
		GetComponent<Animator>().Play("Shrink");
		renderer.material.color = Color.white;
	}

	private void OnMouseDown() {
		audio.Play();
		switch(button) {
		case Button.PlayMulti:
			Application.LoadLevel("Board");
			break;
		case Button.PlayComp:
			GameObject.Find("GameState").GetComponent<GameState>().useAI = true;
			Application.LoadLevel("Board");
			break;
		case Button.Options:
		case Button.Help:
			break;
		}
	}
}
