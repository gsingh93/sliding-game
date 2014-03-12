using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Direction = Grid.Direction;
using Square = Grid.Square;
using SquareType = Grid.SquareType;
using PlayerEnum = Player.PlayerEnum;
using Keymap = Player.Keymap;

public class Piece : MonoBehaviour {
	
	private static PlayerEnum turn = PlayerEnum.Player1;
	
	public GameObject upArrowPrefab;
	public GameObject downArrowPrefab;
	public GameObject leftArrowPrefab;
	public GameObject rightArrowPrefab;

	private float speed = 0.2f;

	private bool _isActive = false;
	private bool isActive {
		get { return _isActive; }
		set {
			if (value) {
				SetAllInactive();
				CreateArrows();
			} else {
				DestroyArrows();
			}
			_isActive = value;
		}
	}

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
	private Piece pieceToDestroy;
	private Player parent;
	private Player opponent;

	private static Keymap keymap;

	private GameState gameState;

	private void Start() {
		gameState = GameState.Create();
		g = GameObject.Find("Board").GetComponent<Grid>();
		parent = transform.parent.GetComponent<Player>();

		string opponentName = parent.opponent.ToString();
		opponent = GameObject.Find(opponentName).GetComponent<Player>();

		keymap = Player.keymap1;
		UpdatePosition();

		DebugUtils.Assert(parent);
		DebugUtils.Assert(opponent);
	}
	
	private void OnMouseDown() {
		if (turn == parent.player) {
			isActive = true;
		}
	}

	private void SetAllInactive() {
		foreach (Piece p in parent.pieces) {
			p.isActive = false;
		}
	}

	private void Update() {
		if (!isActive || turn != parent.player) {
			return;
		}

		switch(state) {
		case State.Sliding:
			if (moving == false) {
				drawArrows = true;
				state = State.Stationary;
				keymap = parent.opponentKeymap;
				turn = parent.opponent;
				if (pieceToDestroy != null) {
					pieceToDestroy.Destroy();
				}
				Camera.main.audio.Stop();
			}
			break;
		case State.Stationary:
			if (drawArrows) {
				UpdatePosition();
				CreateArrows();
				drawArrows = false;
			}

			if (HandleKeyboardInput()) {
				state = State.Sliding;
				DestroyArrows();
				Camera.main.audio.Play();
			}

			break;
		}
	}

	private void UpdatePosition() {
		Pair<int, int> p = g.CoordToPos(transform.position);
		ChangePosition(p.First, p.Second);
	}

	private void ChangePosition(int r, int c) {
		if (row != 0 && col != 0) { // Unset
			DebugUtils.Assert(parent.pieceMap.Remove(new Pair<int, int>(row, col)));
			g.SetSquare(row, col, new Square(SquareType.Empty));
		}
		row = r;
		col = c;
		SquareType type = parent.squareType;
		g.SetSquare(row, col, new Square(type));
		parent.pieceMap.Add(new Pair<int, int>(row, col), this);
	}

	private bool HandleKeyboardInput() {
		Direction dir;
		if (Input.GetKeyDown(keymap.Left)) {
			dir = Direction.Left;
		} else if (Input.GetKeyDown(keymap.Right)) {
			dir = Direction.Right;
		} else if (Input.GetKeyDown(keymap.Up)) {
			dir = Direction.Up;
		} else if (Input.GetKeyDown(keymap.Down)) {
			dir = Direction.Down;
		} else {
			return false;
		}

		Pair<int, int> p = g.FindOpenSquare(dir, row, col);
		if (p == null) {
			return false;
		}
		ChangePosition(p.First, p.Second);

		// Check if the square behind the spot moved to is an enemy
		Pair<SquareType, Pair<int, int>> type = g.SquareTypeAt(dir, row, col);
		if (type != null && type.First == parent.opponentSquareType) {
			Pair<int, int> enemyPos = type.Second;
			Pair<SquareType, Pair<int, int>> backingSquare = g.SquareTypeAt(dir, enemyPos.First, enemyPos.Second);
			// Check if the square behind the enemy isn't empty if backing is enabled
			if (gameState.enableBacking == false || (backingSquare != null && backingSquare.First != SquareType.Empty)) {
				Piece enemyPiece;
				opponent.pieceMap.TryGetValue(enemyPos, out enemyPiece);
				DebugUtils.Assert(enemyPiece != null);
				pieceToDestroy = enemyPiece;
				DebugUtils.Assert(opponent.pieceMap.Remove(enemyPos));
				g.Clear(enemyPos);
			}
			if (gameState.takeEnemySpot) {
				ChangePosition(enemyPos.First, enemyPos.Second);
			}
		}

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

		Vector3 velocity = speed * (to - transform.position).normalized;
		while (transform.position != to) {
			transform.position += velocity;
			yield return null;
		}
		transform.position = to;

		moving = false;
	}

	public void Destroy() {
		DebugUtils.Assert(parent.pieces.Remove(this));
		Destroy(gameObject);
	}
}
