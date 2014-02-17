using UnityEngine;
using System;
using System.Collections;

public class CreateBorder : MonoBehaviour {
	public GameObject blockPrefab;

	public void DrawBorder(Grid g) {
		int dim = Grid.Dim;

		// Bottom row
		CreateRow(dim, i => PlaceBlock(g, dim - 1, i));

		// Top row
		CreateRow(dim, i => PlaceBlock(g, 0, i));

		// Right column
		CreateRow(dim - 2, i => PlaceBlock(g, i + 1, 0));

		// Left column
		CreateRow(dim - 2, i => PlaceBlock(g, i + 1, dim - 1));
	}

	private Vector3 PlaceBlock(Grid g, int row, int col) {
		g.SetSquare(row, col, Grid.SquareType.Block); 
		return g.PosToCoord(row, col);
	}

	private void CreateRow(int length, Func<int, Vector3> placementFunc) {
		for (int i = 0; i < length; i++) {
			GameObject block = Instantiate(blockPrefab) as GameObject;
			block.transform.position = placementFunc(i);
			block.transform.parent = transform;
		}
	}
}
