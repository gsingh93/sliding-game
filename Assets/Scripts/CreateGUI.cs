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

		displayInfo(player1, 8, 7);
		displayInfo(player2, 84, 7);
	}

	private void displayInfo(Player player, int xPercent, int yPercent) {
		float x = xPercent * Screen.width / 100;
		float y = yPercent * Screen.height / 100;

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
