using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	void OnGUI () {
		if (GameManager.Instance.IsPlaying()) {
			DrawScore();
		}
	}
	
	void DrawScore() {
		int width = 80;
		int height = 23;
//		GUI.Box(new Rect (Screen.width/8, 0, Screen.height / 4, height), "MIN:" + Frog.MinRating);
//		GUI.Box(new Rect (Screen.width/4,10,600/4,height), "enemies" + EnemySpawner.Instance.actualEnemyCount + "/" + EnemySpawner.Instance.desiredEnemyCount);
		if (Frog.Surpassing) {
			GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + Frog.TotalRating + "!");
		} else {
			GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + Frog.TotalRating + " / " + Frog.HighRating);
		}
	}
}
