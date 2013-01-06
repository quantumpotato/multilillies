using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	int displayedScore;
	int[] individualDisplayedScores = new int[]{0,0,0,0};
	int updateScoreDisplayTimer;
	int scoreDisplayTimerReset;
	
	int leftmostScoreColumnX = 800;
	int scoreHorizontalMargin = 100;
	
	
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
			DrawTeamMultiplier();
		} else {
			DrawIndividualScores();
			DrawIndividualMultipliers();	
		}
	}
	
	void Update() {
		UpdateDisplayedScores();
	}
	
	#endregion
	
	void UpdateDisplayedScores() {
		updateScoreDisplayTimer--;
		if (updateScoreDisplayTimer <= 0) {
			if (GameManager.Instance.IsCoopMode()) {
				UpdateDisplayedCoopScore();
			} else {
				UpdateDisplayedCompetitiveScores();
			}
			ResetScoreDisplayTimer();
		}
		
		
	}
	
	void ResetScoreDisplayTimer() {
		updateScoreDisplayTimer = scoreDisplayTimerReset;
	}
	
	void DrawScore() {
		int width = 130;
		int height = 23;
		GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + displayedScore + " / " + Frog.HighRating);
	}
	
	#region coop
	
	void UpdateDisplayedCoopScore() {
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
	}
	
	void DrawLives() {
		int width = 80;
		int height = 23;
		GUI.Box (new Rect (Screen.width/2-width-50/2,Screen.height-40,width,height), "Frogs: " + PlayerManager.Instance.lives);	
	}
	
	void DrawTeamMultiplier() {
		int width = 50;
		int height = 23;
		GUI.Box (new Rect (Screen.width/2+width/2,Screen.height-40,width,height), Frog.TeamScoreMultiplier + "x");	
	}
	
	#endregion
	
	#region competitive
	
	void DrawIndividualScores() {
		int width = 70;
		int height = 30;
		int x = leftmostScoreColumnX;
		GUIStyle style = new GUIStyle();
		style.fontSize = 30;
		GUI.backgroundColor = Color.clear;
		for (int i = 0; i < 4; i++) {
			style.normal.textColor = Frog.FrogColors[i];
			Frog f = PlayerManager.Instance.Frogs[i];
			if (f.gameObject.active) {
				GUI.color = Frog.FrogColors[i];
				GUI.Box (new Rect(x,40,width, height), "" + individualDisplayedScores[i], style);
			}
			x+= scoreHorizontalMargin;
		}
	}
	
	void DrawIndividualMultipliers() {
		int width = 70;
		int height = 30;
		int x = leftmostScoreColumnX;
		GUIStyle style = new GUIStyle();
		style.fontSize = 30;
		GUI.backgroundColor = Color.clear;
		for (int i = 0; i < 4; i++) {
			style.normal.textColor = Frog.FrogColors[i];
			Frog f = PlayerManager.Instance.Frogs[i];
			if (f.gameObject.active) {
				GUI.color = Frog.FrogColors[i];
				GUI.Box (new Rect(x,Screen.height-40,width, height), "X" + f.scoreMultiplier, style);
			}
			x+= scoreHorizontalMargin;
		}
	}
	
	void UpdateDisplayedCompetitiveScores() {
		for (int i = 0; i < 4; i++) {
			Frog f = PlayerManager.Instance.Frogs[i];
			if (f.gameObject.active) {
				int amplifiedFrogScore = f.score * 100;
				if (amplifiedFrogScore > individualDisplayedScores[i]) {
					if (amplifiedFrogScore > individualDisplayedScores[i] + 27) {
						individualDisplayedScores[i]+= 27;	
					} else {
						individualDisplayedScores[i]++;
					}
				} else if (amplifiedFrogScore < individualDisplayedScores[i]) {
					individualDisplayedScores[i]--;	
				}
			}
		}
	}
	
	#endregion
}
