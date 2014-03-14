﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Direction = Grid.Direction;
using Square = Grid.Square;
using SquareType = Grid.SquareType;
using PlayerEnum = Player.PlayerEnum;
using Keymap = Player.Keymap;

public class Piece : MonoBehaviour {
	
	public static PlayerEnum turn = PlayerEnum.Player1;

	public GameObject trailBlockPrefab;

	public GameObject upArrowPrefab;
	public GameObject downArrowPrefab;
	public GameObject leftArrowPrefab;
	public GameObject rightArrowPrefab;

	private float speed = 0.2f;

	private bool _isActive = false;
	public bool isActive {
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
		DebugUtils.Assert(parent);

		string opponentName = parent.opponent.ToString();
		opponent = GameObject.Find(opponentName).GetComponent<Player>();
		DebugUtils.Assert(opponent);

		keymap = Player.keymap1;
		UpdatePosition();
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
				UpdateTrails();
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

	private void UpdateTrails() {
		for (int i = trails.Count - 1; i >= 0; i--) {
			trails[i].TurnPassed();
			if (trails[i].finished) {
				trails.RemoveAt(i);
			}
		}
	}

	private void UpdatePosition() {
		Pair<int, int> p = g.CoordToPos(transform.position);
		ChangePosition(p.First, p.Second);
	}

	// TODO: Overlapping trails
	class Trail {
		private Grid g;

		private List<Pair<int, int>> coords = new List<Pair<int, int>>();
		private List<Square> squares = new List<Square>();
		private List<GameObject> blocks = new List<GameObject>();

		public bool finished = false;

		public Trail(Grid g) {
			this.g = g;
		}

		public void Add(Pair<int, int> coord, Square square, GameObject block) {
			coords.Add(coord);
			squares.Add(square);
			blocks.Add(block);
		}

		public void TurnPassed() {
			for (int i = coords.Count - 1; i >= 0; i--) {
				squares[i].turnsRemaining--;
				if (squares[i].turnsRemaining == 0) {
					finished = true;
					GameObject b = blocks[i];
					blocks.RemoveAt(i);
					Destroy(b);
					g.SetSquare(coords[i].First, coords[i].Second, new Square(SquareType.Empty));
				}
			}
		}
	}

	static void Swap<T>(ref T lhs, ref T rhs) {
		T temp;
		temp = lhs;
		lhs = rhs;
		rhs = temp;
	}

	private static List<Trail> trails = new List<Trail>();

	GameObject PlaceTrailBlock(Vector3 position) {
		GameObject block = Instantiate(trailBlockPrefab) as GameObject;
		block.transform.position = position;
		block.renderer.material.color = parent.trailColor;
		return block;
		//block.transform.parent = t.transform;
	}

	private void ChangePosition(int r, int c) {
		if (row != 0 && col != 0) { // Unset
			DebugUtils.Assert(parent.pieceMap.Remove(new Pair<int, int>(row, col)));
			g.SetSquare(row, col, new Square(SquareType.Empty));
			//AddTrail(row, col, r, c);
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

				if (gameState.takeEnemySpot) {
					ChangePosition(enemyPos.First, enemyPos.Second);
				}
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

		Trail t = new Trail(g);

		Vector3 velocity = speed * (to - transform.position).normalized;
		Pair<int, int> lastPos = null;
		while (transform.position != to) {
			Pair<int, int> pos = g.CoordToPos(transform.position, false);
			if (lastPos != pos) {
				Square s = new Square(parent.trailType);
				s.turnsRemaining = 3;

				Vector3 coord = g.PosToCoord(pos.First, pos.Second);
				GameObject block = PlaceTrailBlock(coord);
				t.Add(pos, s, block);

				g.SetSquare(pos.First, pos.Second, s);

				lastPos = pos;
			}
			// TODO: Why does this have to come after?
			transform.position += velocity;

			yield return null;
		}
		transform.position = to;

		trails.Add(t);

		moving = false;
	}

	public void Destroy() {
		DebugUtils.Assert(parent.pieces.Remove(this));
		Destroy(gameObject);
	}
}
