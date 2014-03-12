using UnityEngine;
using System.Collections;
using System.Text;

using PlayerEnum = Player.PlayerEnum;

public class CreateGUI : MonoBehaviour {

	public GUIStyle style = new GUIStyle();

	private void OnGUI() {
		Player player1 = GameObject.Find("Player1").GetComponent<Player>();
		Player player2 = GameObject.Find("Player2").GetComponent<Player>();
		DebugUtils.Assert(player1);
		DebugUtils.Assert(player2);

		displayInfo(player1, 60, 50);
		displayInfo(player2, 700, 50);
	}

	private void displayInfo(Player player, int x, int y) {
		string playerName;
		int piecesRemaining = player.pieces.Count;
		if (player.player == PlayerEnum.Player1) {
			playerName = "Player 1";
		} else {
			playerName = "Player 2";
		}
		GUI.Label(new Rect(x, y, 0, 0), playerName, style);
		
		StringBuilder sb = new StringBuilder();
		sb.Append("Pieces Remaining: ");
		sb.Append(piecesRemaining);
		style.fontSize -= 7;
		GUI.Label(new Rect(x - 40, y + 30, 0, 0), sb.ToString(), style);
		style.fontSize += 7;
	}
}
