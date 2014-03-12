﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SquareType = Grid.SquareType;

public class Player : MonoBehaviour {
	public Piece piecePrefab;
	public Material mat1;
	public Material mat2;

	public PlayerEnum player;
	public PlayerEnum opponent;
	public enum PlayerEnum {
		Player1, Player2
	}

	public Keymap keymap;
	public Keymap opponentKeymap;
	public class Keymap {
		public KeyCode Left;
		public KeyCode Right;
		public KeyCode Up;
		public KeyCode Down;
		
		public Keymap(KeyCode l, KeyCode r, KeyCode u, KeyCode d) {
			Left = l;
			Right = r;
			Up = u;
			Down = d;
		}
	}

	public SquareType squareType;
	public SquareType opponentSquareType;

	public Dictionary<Pair<int, int>, Piece> pieceMap = new Dictionary<Pair<int, int>, Piece>();
	public HashSet<Piece> pieces;

	public static Keymap keymap1 = new Keymap(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
	public static Keymap keymap2 = new Keymap(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow);

	void Start() {
		Material material;
		if (player == PlayerEnum.Player1) {
			opponent = PlayerEnum.Player2;
			keymap = keymap1;
			opponentKeymap = keymap2;
			squareType = SquareType.Player1;
			opponentSquareType = SquareType.Player2;
			material = mat1;
		} else {
			opponent = PlayerEnum.Player1;
			keymap = keymap2;
			opponentKeymap = keymap1;
			squareType = SquareType.Player2;
			opponentSquareType = SquareType.Player1;
			material = mat2;
		}

		pieces = new HashSet<Piece>(GetComponentsInChildren<Piece>());
		foreach (Piece p in pieces) {
			p.renderer.material = material;
		}
	}
}
