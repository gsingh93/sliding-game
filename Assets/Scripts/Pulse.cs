using UnityEngine;
using System.Collections;

public class Pulse : MonoBehaviour {

	public float step = .1f;
	public float interval = 0.1f;
	public float min = 0;

	public static bool guard = false;

	private IEnumerator Start() {
		while (true) {
			while (guard) {
				yield return null;
			}
			float alpha = renderer.material.color.a;
			alpha += step;

			if (alpha >= 1) {
				alpha = 1;
				step *= -1;
			} else if (alpha <= min) {
				alpha = min;
				step *= -1;
			}

			Color c = renderer.material.color;
			renderer.material.color = new Color(c.r, c.g, c.b, alpha);
			yield return new WaitForSeconds(interval);
		}
	}
}
