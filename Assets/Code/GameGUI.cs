using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	List<Frog> players;
	
	int TotalScore {
		get {
			int score = 0;
			players.ForEach(player => score += player.score);
			return score;
		}
	}

	void Start () {
		players = new List<Frog>();
		GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject go in playerGameObjects) {
			players.Add(go.GetComponent<Frog>());
		}
	}
	
	void OnGUI () {
		int width = 80;
		int height = 23;
		GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + TotalScore);
	}
}
