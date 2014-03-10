using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public HashSet<Piece> pieces;
	
	public Dictionary<Pair<int, int>, Piece> pieceMap = new Dictionary<Pair<int, int>, Piece>();
	void Start() {
		pieces = new HashSet<Piece>(transform.GetComponentsInChildren<Piece>());
	}
}
