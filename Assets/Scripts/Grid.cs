using UnityEngine;
using System.Collections;
using System.Text;

public class Grid : MonoBehaviour {

	public const int Dim = 12;

	public enum SquareType {
		Empty, Block, Player1, Player2, Coin
	}

	public class Square {
		public SquareType type;

		public Square(SquareType type) {
			this.type = type;
		}
	}

	public enum Direction {
		Up, Down, Left, Right
	}

	private Pair<int, int>[] directionOffsets = { 
		new Pair<int, int>(0, 1), new Pair<int, int>(0, -1), // Up, Down
		new Pair<int, int>(-1, 0), new Pair<int, int>(1, 0) }; // Left, Right
	
	private Square[,] grid;

	void Awake() {
		grid = new Square[Dim, Dim];
		for (int i = 0; i < Dim; i++) {
			for (int j = 0; j < Dim; j++) {
				SquareType type = SquareType.Empty;
				if (i == 0 || i == Dim - 1 || j == 0 || j == Dim - 1) {
					type = SquareType.Block;
				}
				grid[i, j] = new Square(type);
			}
		}
	}

	public Pair<int, int> FindOpenSquare(Direction dir, int row, int col) {
		int stepC = directionOffsets[(int) dir].First;
		int stepR = directionOffsets[(int) dir].Second;

		int i;
		for (i = 1; IsEmpty(row + stepR * i, col + stepC * i); i++) {}

		if (i == 1) {
			return null;
		} else {
			i--;
			return new Pair<int, int>(row + stepR * i, col + stepC * i);
		}
	}

	public void Print() {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < Dim; i++) {
			for (int j = 0; j < Dim; j++) {
				switch(grid[i, j].type) {
				case SquareType.Empty:
					sb.Append("E");
					break;
				case SquareType.Block:
					sb.Append("B");
					break;
				case SquareType.Player1:
					sb.Append("1");
					break;
				case SquareType.Player2:
					sb.Append("2");
					break;
				}
			}
			sb.AppendLine("");
		}

		Debug.Log(sb.ToString());
	}

	public void SetSquare(int row, int col, Square square) {
		BoundsCheck(row, col);

		grid[row, col] = square;
	}

	public Pair<SquareType, Pair<int, int>> SquareTypeAt(Direction dir, int row, int col) {
		if (!InBounds(row, col)) {
			return null;
		}

		int stepC = directionOffsets[(int) dir].First;
		int stepR = directionOffsets[(int) dir].Second;
		int r = row + stepR;
		int c = col + stepC;

		Pair<int, int> pos = new Pair<int, int>(r, c);
		return new Pair<SquareType, Pair<int, int>>(grid[r, c].type, pos);
	}

	public void Clear(Pair<int, int> pos) {
		grid[pos.First, pos.Second] = new Square(SquareType.Empty);
	}

	public bool IsEmpty(int row, int col) {
		BoundsCheck(row, col);

		return grid[row, col].type == SquareType.Empty;
	}

	public Vector3 PosToCoord(int row, int col) {
		BoundsCheck(row, col);

		return new Vector3(-5.5f + col, -5.5f + row, -1);
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
