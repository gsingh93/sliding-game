using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {
	private static GameState instance = null;

	public bool enableBacking = false;
	public bool takeEnemySpot = false;
	public bool useAI = false;

	private void Awake() {
		instance = this;
	}

	public static GameState Create() {
		return instance;
	}
}
