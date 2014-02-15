using UnityEngine;
using System;
using System.Collections;

public class CreateBorder : MonoBehaviour {
	public GameObject blockPrefab;

	private const int Dim = 12;

	void Awake() {
		// Bottom row
		Vector3 start = new Vector3(-5.5f, -5.5f, 1);
		CreateRow(Dim, i => start + i * Vector3.right);

		// Top row
		start = new Vector3(-5.5f, 5.5f, 1);
		CreateRow(Dim, i => start + i * Vector3.right);

		// Right column
		start = new Vector3(5.5f, 4.5f, 1);
		CreateRow(Dim - 2, i => start + i * Vector3.down);

		// Left column
		start = new Vector3(-5.5f, 4.5f, 1);
		CreateRow(Dim - 2, i => start + i * Vector3.down);
	}

	private void CreateRow(int length, Func<int, Vector3> placementFunc) {
		for (int i = 0; i < length; i++) {
			GameObject block = Instantiate(blockPrefab) as GameObject;
			block.transform.position = placementFunc(i);
			block.transform.parent = gameObject.transform;
		}
	}
}
