using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	public float playButtonWidth;
	public float playButtonHeight;
	
	void OnGUI() {
		if (GUI.Button(new Rect(Screen.width/2-playButtonWidth/2,
								Screen.height/2-playButtonHeight/2,
								playButtonWidth,
								playButtonHeight), "Play")) {
			Application.LoadLevel("main");
		}
	}
}
