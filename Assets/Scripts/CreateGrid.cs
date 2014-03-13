using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Square = Grid.Square;
using SquareType = Grid.SquareType;

public class CreateGrid : MonoBehaviour {
	public GameObject blockPrefab;
	public GameObject piecePrefab;
	private Material lineMaterial;

	private int dim = Grid.Dim;
	private GameObject board;
	private Grid g;

	private Pair<int, int>[] blocks = {
		new Pair<int, int>(4, 1),
		new Pair<int, int>(10, 1),
		new Pair<int, int>(6, 2),
		new Pair<int, int>(8, 3),
		new Pair<int, int>(3, 4),
		new Pair<int, int>(7, 5),
		new Pair<int, int>(5, 6),
		new Pair<int, int>(9, 7),
		new Pair<int, int>(4, 8),
		new Pair<int, int>(8, 9),
		new Pair<int, int>(11, 10),
		new Pair<int, int>(5, 11),
		new Pair<int, int>(7, 12),
		new Pair<int, int>(4, 13),
		new Pair<int, int>(10, 13),
	};

	void Start() {
		board = GameObject.Find("Board");
		DebugUtils.Assert(board);

		g = board.GetComponent<Grid>();
		DebugUtils.Assert(g);

		lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
		                                                      "SubShader { Pass { " +
		                                                      "    Blend SrcAlpha OneMinusSrcAlpha " +
		                                                      "    ZWrite Off Cull Off Fog { Mode Off } " +
		                                                      "    BindChannels {" +
		                                                      "      Bind \"vertex\", vertex Bind \"color\", color }" +
		                                                      "} } }" );
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

		CreateBorder();
		PlaceBlocks();
		PlacePieces();
	}

	private void CreateBorder() {
		// Bottom row
		CreateRow(dim, i => PlaceBlock(g, dim - 1, i));
		
		// Top row
		CreateRow(dim, i => PlaceBlock(g, 0, i));
		
		// Right column
		CreateRow(dim - 2, i => PlaceBlock(g, i + 1, 0));
		
		// Left column
		CreateRow(dim - 2, i => PlaceBlock(g, i + 1, dim - 1));
	}

	private void OnPostRender() {
		GL.PushMatrix();
		lineMaterial.SetPass(0);
		GL.Begin(GL.LINES);
		GL.Color(Color.grey);

		float dimF = dim;

		// Vertical lines
		for (int i = 1; i < dim; i++) {
			GL.Vertex3(i - dimF/2, -1 * dimF/2, -2);
			GL.Vertex3(i - dimF/2, dimF/2, -2);
		}

		// Horizontal lines
		for (int i = 1; i < dim; i++) {
			GL.Vertex3(-1 * dimF/2, i - dimF/2, -2);
			GL.Vertex3(dimF/2, i - dimF/2, -2);
		}

		GL.PopMatrix();
		GL.End();
	}

	private void PlaceBlocks() {
		foreach (Pair<int, int> p in blocks) {
			Vector3 coord = PlaceBlock(g, p.First, p.Second);
			CreateBlock(coord);
		}
		
		//		for (int i = 0; i < 5; i++) {
		//			Vector3 coord = PlaceBlock(g, 4, i * 2 + 1);
		//			CreateBlock(coord);
		//		}
		//
		//		for (int i = 0; i < 5; i++) {
		//			Vector3 coord = PlaceBlock(g, 7, i * 2 + 2);
		//			CreateBlock(coord);
		//		}
		
		//		System.Random r = new System.Random();
		//		for (int i = 0; i < 20; i++) {
		//			Vector3 coord = PlaceBlock(g, r.Next(dim - 3) + 2, r.Next(dim - 2) + 1);
		//			CreateBlock(coord);
		//		}
	}

	private void PlacePieces() {
		GameObject player1 = GameObject.Find("Player1");
		GameObject player2 = GameObject.Find("Player2");
		DebugUtils.Assert(player1);
		DebugUtils.Assert(player2);

		int numPieces = dim - 2;
		for (int i = 0; i < numPieces; i += 2) {
			Vector3 coord = Place(g, 1, i + 1, SquareType.Player1);
			CreatePiece(coord, player1);
		}
		
		for (int i = 0; i < numPieces; i += 2) {
			Vector3 coord = Place(g, dim - 2, i + 1, SquareType.Player2);
			CreatePiece(coord, player2);
		}
	}

	private Vector3 PlaceBlock(Grid g, int row, int col) {
		return Place(g, row, col, SquareType.Block);
	}

	private Vector3 Place(Grid g, int row, int col, SquareType type) {
		g.SetSquare(row, col, new Square(type)); 
		return g.PosToCoord(row, col);
	}

	private void CreateRow(int length, Func<int, Vector3> placementFunc) {
		for (int i = 0; i < length; i++) {
			CreateBlock(placementFunc(i));
		}
	}

	private void CreatePiece(Vector3 position, GameObject parent) {
		GameObject piece = Instantiate(piecePrefab) as GameObject;
		piece.transform.position = position;
		piece.transform.parent = parent.transform;
	}

	private void CreateBlock(Vector3 position) {
		GameObject block = Instantiate(blockPrefab) as GameObject;
		block.transform.position = position;
		block.transform.parent = board.transform;
	}
}
