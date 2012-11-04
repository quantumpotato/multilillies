using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	void OnGUI () {
		int width = 80;
		int height = 23;
		GUI.Box(new Rect (Screen.width/2-width/2,10,width,height), "" + Frog.TotalScore);
	}
}
