﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Direction = Grid.Direction;
using Square = Grid.Square;
using SquareType = Grid.SquareType;

public class Piece : MonoBehaviour {

	public PlayerEnum player;
	private static PlayerEnum turn = PlayerEnum.Player1;
	public enum PlayerEnum {
		Player1, Player2
	}
	
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

	private void Start() {
		parent = transform.parent.GetComponent<Player>();
		g = GameObject.Find("Board").GetComponent<Grid>();
		UpdatePosition();
	}
	
	private void OnMouseDown() {
		if (turn == player) {
			isActive = true;
		}
	}

	private void SetAllInactive() {
		foreach (Piece p in parent.pieces) {
			p.isActive = false;
		}
	}

	private void Update() {
		if (!isActive || turn != player) {
			return;
		}

		switch(state) {
		case State.Sliding:
			if (moving == false) {
				drawArrows = true;
				state = State.Stationary;
				turn = player == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1;
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
		string opponentName = player == PlayerEnum.Player1 ? "Player1" : "Player2";
		Player p = GameObject.Find(opponentName).GetComponent<Player>();
		p.pieceMap.Remove(new Pair<int, int>(row, col));
		
		g.SetSquare(row, col, new Square(SquareType.Empty));
		row = r;
		col = c;
		SquareType type = player == PlayerEnum.Player1 ? SquareType.Player1 : SquareType.Player2;
		g.SetSquare(row, col, new Square(type));
		p.pieceMap.Add(new Pair<int, int>(row, col), this);
	}

	private bool HandleKeyboardInput() {
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
		ChangePosition(p.First, p.Second);

		SquareType type = player == PlayerEnum.Player1 ? SquareType.Player2 : SquareType.Player1;
		Pair<int, int> enemyPos = g.EnemyPosition(dir, row, col, type);
		if (enemyPos != null) {
			string opponentName = player == PlayerEnum.Player1 ? "Player2" : "Player1";
			Player opponent = GameObject.Find(opponentName).GetComponent<Player>();

			Piece enemyPiece;
			opponent.pieceMap.TryGetValue(enemyPos, out enemyPiece);
			DebugUtils.Assert(enemyPiece != null);
			pieceToDestroy = enemyPiece;
			opponent.pieceMap.Remove(enemyPos);
			g.Clear(enemyPos);
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
		parent.pieces.Remove(this);
		Destroy(gameObject);
	}
}
