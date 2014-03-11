﻿using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {
	private static GameState instance = null;

	public bool enableBacking = false;

	private void Awake() {
		instance = this;
	}

	public static GameState Create() {
		return instance;
	}
}
