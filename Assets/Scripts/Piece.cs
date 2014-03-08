using UnityEngine;
using System.Collections;

using Direction = Grid.Direction;

public class Piece : MonoBehaviour {
	
	public GameObject upArrowPrefab;
	public GameObject downArrowPrefab;
	public GameObject leftArrowPrefab;
	public GameObject rightArrowPrefab;

	private enum State {
		Sliding, Stationary
	}
	private State state = State.Stationary;

	// True if we need to draw arrows, false otherwise
	private bool drawArrows;

	private bool moving;

	private int[] dr = {-1, 1, 0, 0};
	private int[] dc = {0, 0, 1, -1};

	private int row, col;

	private Grid g;

	void Start() {
		g = GameObject.Find("Board").GetComponent<Grid>();
		UpdatePosition();
		CreateArrows();
	}

	void Update() {
		switch(state) {
		case State.Sliding:
			if (moving == false) {
				drawArrows = true;
				state = State.Stationary;
				Camera.main.audio.Stop();
			}
			break;
		case State.Stationary:
			if (drawArrows) {
				UpdatePosition();
				CreateArrows();
				drawArrows = false;
			}

			if (handleKeyboardInput()) {
				state = State.Sliding;
				DestroyArrows();
				Camera.main.audio.Play();
			}

			break;
		}
	}

	void UpdatePosition() {
		Pair<int, int> p = g.CoordToPos(transform.position);
		row = p.First;
		col = p.Second;
	}

	bool handleKeyboardInput() {
		Direction dir;
		if (Input.GetKeyDown(KeyCode.A)) {
			dir = Direction.Left;
		} else if (Input.GetKeyDown(KeyCode.D)) {
			dir = Direction.Right;
		} else if (Input.GetKeyDown(KeyCode.W)) {
			dir = Direction.Up;
		} else if (Input.GetKeyDown(KeyCode.S)) {
			dir = Direction.Down;
		} else {
			return false;
		}

		Pair<int, int> p = g.FindOpenSquare(dir, row, col);
		if (p == null) {
			return false;
		}
		row = p.First;
		col = p.Second;

		StartCoroutine(move(g.PosToCoord(row, col)));
		
		return true;
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
		if (dr[index] == -1) {        // Down
			arrow = Instantiate(downArrowPrefab) as GameObject;
		} else if (dr[index] == 1) {  // Up
			arrow = Instantiate(upArrowPrefab) as GameObject;
		} else if (dc[index] == -1) { // Left
			arrow = Instantiate(leftArrowPrefab) as GameObject;
		} else if (dc[index] == 1) {  // Right
			arrow = Instantiate(rightArrowPrefab) as GameObject;
		} else {
			throw new System.ArgumentException();
		}

		arrow.transform.parent = transform;
		arrow.transform.position = transform.position + new Vector3(dc[index], dr[index], 0);
	}

	private void DestroyArrows() {
		foreach (Transform t in transform) {
			Destroy(t.gameObject);
		}
	}

	private IEnumerator move(Vector3 to) {
		moving = true;

		Vector3 velocity = 0.1f * (to - transform.position).normalized;
		while (transform.position != to) {
			transform.position += velocity;
			yield return null;
		}
		transform.position = to;

		moving = false;
	}
}
