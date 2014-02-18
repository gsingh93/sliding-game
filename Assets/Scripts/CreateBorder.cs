using UnityEngine;
using System;
using System.Collections;

using Square = Grid.Square;
using SquareType = Grid.SquareType;

public class CreateBorder : MonoBehaviour {
	public GameObject blockPrefab;

	void Start() {
		Grid g = GetComponent<Grid>();
		int dim = Grid.Dim;

		// Bottom row
		CreateRow(dim, i => PlaceBlock(g, dim - 1, i));

		// Top row
		CreateRow(dim, i => PlaceBlock(g, 0, i));

		// Right column
		CreateRow(dim - 2, i => PlaceBlock(g, i + 1, 0));

		// Left column
		CreateRow(dim - 2, i => PlaceBlock(g, i + 1, dim - 1));

//		System.Random r = new System.Random();
//		for (int i = 0; i < 5; i++) {
//			Vector3 coord = PlaceBlock(g, r.Next(dim - 2) + 1, r.Next(dim - 2) + 1);
//			CreateBlock(coord);
//		}
	}

	private Vector3 PlaceBlock(Grid g, int row, int col) {
		g.SetSquare(row, col, new Square(SquareType.Block)); 
		return g.PosToCoord(row, col);
	}

	private void CreateRow(int length, Func<int, Vector3> placementFunc) {
		for (int i = 0; i < length; i++) {
			CreateBlock(placementFunc(i));
		}
	}

	private void CreateBlock(Vector3 position) {
		GameObject block = Instantiate(blockPrefab) as GameObject;
		block.transform.position = position;
		block.transform.parent = transform;
	}
}
