using UnityEngine;
using System;
using System.Collections;

public class CreateBorder : MonoBehaviour {
	public GameObject blockPrefab;

	void Awake() {
		Grid g = GetComponent<Grid>();
		int dim = Grid.Dim;

		// Bottom row
		CreateRow(dim, i => g.PosToCoord(dim - 1, i));

		// Top row
		CreateRow(dim, i => g.PosToCoord(0, i));

		// Right column
		CreateRow(dim - 2, i => g.PosToCoord(i + 1, 0));

		// Left column
		CreateRow(dim - 2, i => g.PosToCoord(i + 1, dim - 1));
	}

	private void CreateRow(int length, Func<int, Vector3> placementFunc) {
		for (int i = 0; i < length; i++) {
			GameObject block = Instantiate(blockPrefab) as GameObject;
			block.transform.position = placementFunc(i);
			block.transform.parent = transform;
		}
	}
}
