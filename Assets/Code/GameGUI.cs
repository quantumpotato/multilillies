using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	void OnGUI () {
		DrawScore ();
		DrawPlayerBoxes();
	}
	
	void DrawScore() {
		int width = 80;
		int height = 23;
		if (Frog.Surpassing) {
			GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + Frog.TotalScore + "!");
		} else {
			GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + Frog.TotalScore + " / " + Frog.HighScore);
		}
	}
	
	void DrawPlayerBoxes() {
		float width = 50;
		float height = 50;
		Texture2D texture = (Texture2D)Resources.Load("WhiteTexture");
		
		// top left = red
		GUI.color = Color.red;
		GUI.DrawTexture(new Rect(0,0,width,height), texture);
		
		// bottom left = yellow
		GUI.color = Color.yellow;
		GUI.DrawTexture(new Rect(0,Screen.height - height,width,height), texture);
		
		// top right = white
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(Screen.width - width,0,width,height), texture);
		
		// bottom right = green
		GUI.color = Color.green;
		GUI.DrawTexture(new Rect(Screen.width - width,Screen.height - height,width,height), texture);
	}
}
