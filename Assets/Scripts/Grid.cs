using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public const int Dim = 12;

	public enum SquareType {
		Empty, Block, Player1, Player2, Coin
	}

	public enum Player {
		One, Two
	}

	private class Square {
		public SquareType type;
		public Player player;

		public Square() {
			type = SquareType.Empty;
		}
	}

	private Square[,] grid;

	void Awake() {
		grid = new Square[Dim, Dim];
		for (int i = 0; i < Dim; i++) {
			for (int j = 0; j < Dim; j++) {
				grid[i, j] = new Square();
			}
		}
	}

	public bool IsEmpty(int row, int col) {
		BoundsCheck(row, col);

		return grid[row, col].type == SquareType.Empty;
	}

	public Vector3 PosToCoord(int row, int col) {
		BoundsCheck(row, col);

		return new Vector3(-5.5f + col, 5.5f - row, -1);
	}

	public Pair<int, int> CoordToPos(Vector3 coord) {
		int row = (int) (coord.y + 5.5f);
		int col = (int) (coord.x + 5.5f);

		BoundsCheck(row, col);

		return new Pair<int, int>(row, col);
	}

	private void BoundsCheck(int row, int col) {
		if (!InBounds(row, col)) {
			throw new System.ArgumentOutOfRangeException("Out of bounds: (" + row + ", " + col + ")");
		}
	}

	private bool InBounds(int row, int col) {
		return row < Dim && col < Dim && row >= 0 && col >= 0;
	}
}
