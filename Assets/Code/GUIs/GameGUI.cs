using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	int displayedScore;
	int updateScoreDisplayTimer;
	int scoreDisplayTimerReset;
	
	#region MonoBehavior
	
	void Awake() {
		scoreDisplayTimerReset = 2;
		ResetScoreDisplayTimer();
	}
	
	void OnGUI () {
		if (GameManager.Instance.IsPlaying()) {
			DrawScore();
		}
		if (GameManager.Instance.IsCoopMode()) {
			DrawLives();
		}
	}
	
	void Update() {
		updateScoreDisplayTimer--;
		if (updateScoreDisplayTimer <= 0) {
			int amplifiedFrogScore = Frog.TotalScore * 100;
			if (amplifiedFrogScore > displayedScore) {
				if (amplifiedFrogScore > displayedScore + 27) {
					displayedScore+= 9;	
				} else {
				    displayedScore++;	
				}
			} else if (amplifiedFrogScore < displayedScore) {
				if (amplifiedFrogScore < displayedScore - 27) {
					displayedScore-= 9;
				} else {
					displayedScore--;
				}
			}
			ResetScoreDisplayTimer();
		}
	}
	
	#endregion
	
	void ResetScoreDisplayTimer() {
		updateScoreDisplayTimer = scoreDisplayTimerReset;
	}
	
	void DrawScore() {
		int width = 130;
		int height = 23;
		GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + displayedScore + " / " + Frog.HighRating);
	}
	
	void DrawLives() {
		int width = 50;
		int height = 23;
		GUI.Box (new Rect (Screen.width/2-width/2,Screen.height-40,width,height), "" + PlayerManager.Instance.lives);	
	}
}
