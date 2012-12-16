using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	void OnGUI () {
		if (GameManager.Instance.Playing) {
			DrawScore();
		}
	}
	
	void DrawScore() {
		int width = 80;
		int height = 23;
			GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + Frog.TotalScore + " / " + Frog.HighRating);
	}
}
