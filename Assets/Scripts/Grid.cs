using UnityEngine;
using System.Collections;
using System.Text;

public class Grid : MonoBehaviour {

	public const int Dim = 15;

	private const float magicConst = (Dim - 1) / 2;

	public enum SquareType {
		Empty, Block, Player1, Player2, Trail1, Trail2, Coin 
	}

	public class Square {
		public SquareType type;
		public int turnsRemaining;

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

	public Pair<int, int> FindEnemyTrail(Direction dir, int row, int col, SquareType enemyTrail, SquareType myTrail) {
		int stepC = directionOffsets[(int) dir].First;
		int stepR = directionOffsets[(int) dir].Second;
		
		bool found = false;
		while (true) {
			row += stepR;
			col += stepC;
			if (!InBounds(row, col)) {
				break;
			}
			SquareType squareType = grid[row, col].type;
			Debug.Log(squareType);
			if(squareType == enemyTrail) {
				found = true;
				break;
			}
			if (squareType != SquareType.Empty && squareType != myTrail) {
				break;
			}
		}

		if (!found) {
			return null;
		} else {
			return new Pair<int, int>(row, col);
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
		for (int i = Dim - 1; i >= 0; i--) {
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
				case SquareType.Trail1:
					sb.Append("T");
					break;
				case SquareType.Trail2:
					sb.Append("t");
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

	public Square GetSquare(int row, int col) {
		BoundsCheck(row, col);
		
		return grid[row, col];
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

	// TODO: Fix hack
	public bool IsEmpty(int row, int col) {
		BoundsCheck(row, col);

		SquareType t = grid[row, col].type;
		return t == SquareType.Empty || t == SquareType.Trail1 || t == SquareType.Trail2;
	}

	public Vector3 PosToCoord(int row, int col) {
		BoundsCheck(row, col);

		return new Vector3(-1 * magicConst + col, -1 * magicConst + row, -1);
	}

	public Pair<int, int> CoordToPos(Vector3 coord) {
		return CoordToPos(coord, true);
	}

	public Pair<int, int> CoordToPos(Vector3 coord, bool boundsCheck) {
		// The 0.5f is for rounding to the nearest int
		int row = (int) (coord.y + magicConst + 0.5f);
		int col = (int) (coord.x + magicConst + 0.5f);

		if (boundsCheck) BoundsCheck(row, col);

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
