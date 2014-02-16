using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	public GameObject upArrowPrefab;
	public GameObject downArrowPrefab;
	public GameObject leftArrowPrefab;
	public GameObject rightArrowPrefab;

	private int[] dr = {-1, 1, 0, 0};
	private int[] dc = {0, 0, 1, -1};

	private int row, col;

	private Grid g;

	void Start() {
		g = GameObject.Find("Board").GetComponent<Grid>();
		Pair<int, int> p = g.CoordToPos(transform.position);
		row = p.First;
		col = p.Second;
		CreateArrows();
	}

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

	private void CreateArrows() {
		for (int i = 0; i < dc.Length; i++) {
			if (g.IsEmpty(row + dr[i], col + dc[i])) {
				CreateArrow(i);
			}
		}
	}

	private void CreateArrow(int index) {
		GameObject arrow;
		if (dr[index] == 1) {         // Down
			arrow = Instantiate(downArrowPrefab) as GameObject;
		} else if (dr[index] == -1) { // Up
			arrow = Instantiate(upArrowPrefab) as GameObject;
		} else if (dc[index] == -1) { // Left
			arrow = Instantiate(leftArrowPrefab) as GameObject;
		} else if (dc[index] == 1) {  // Right
			arrow = Instantiate(rightArrowPrefab) as GameObject;
		} else {
			throw new System.ArgumentException();
		}

		arrow.transform.parent = transform;
		arrow.transform.position = transform.position + new Vector3(dc[index], -dr[index], 0);
	}
}
