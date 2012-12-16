using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager Instance;
	
	GameObject[] frogObjects;
	public GameObject[] FrogObjects {
		get {
			return frogObjects;
		}
	}
	
	Frog[] frogs;
	public Frog[] Frogs {
		get {
			return frogs;
		}
	}
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
		frogObjects = new GameObject[4];
		frogs = new Frog[4];
	}
	
	void Start() {
		frogObjects[0] = GameObject.Find("P1");
		frogObjects[1] = GameObject.Find("P2");
		frogObjects[2] = GameObject.Find("P3");
		frogObjects[3] = GameObject.Find("P4");
		
		for (int i = 0; i < 4; i++) {
			GameObject frogObject = FrogObjects[i];
			frogObject.SetActiveRecursively(false);
			frogs[i] = frogObject.GetComponent<Frog>();
		}
	}

	void OnGUI() {
		DrawPlayerAreas();
	}
	#endregion
	
	public Color GetPlayerColor(int playerNumber) {
		switch (playerNumber) {
		case 0:
			return Color.yellow;
		case 1:
			return Color.green;
		case 2:
			return Color.red;
		case 3:
			return Color.white;
		default:
			return Color.black;
		}
	}
	
	void DrawPlayerAreas() {
		float textureWidth = 50;
		float textureHeight = 50;
		float buttonWidth = 100;
		float buttonHeight = 50;
		Texture2D texture = (Texture2D)Resources.Load("WhiteTexture");
		float scoreWidth = 80;
		float scoreHeight = 23;
		
		// top left = red
		if (frogObjects[2].active) {
			GUI.color = Color.red;
			GUI.DrawTexture(new Rect(0,0,textureWidth,textureHeight), texture);
			GUI.color = Color.white;
			GUI.Box(new Rect (50, 0, scoreWidth, scoreHeight), "" + PlayerManager.Instance.Frogs[2].score + " x" + PlayerManager.Instance.Frogs[2].scoreMultiplier);
		} else {
			if(GUI.Button(new Rect(0, 0, buttonWidth, buttonHeight), "Play")) {
				StartPlayer(2);
			}
		}
		
		// bottom left = yellow
		if (frogObjects[0].active) {
			GUI.color = Color.yellow;
			GUI.DrawTexture(new Rect(0,Screen.height - textureHeight,textureWidth,textureHeight), texture);
			GUI.color = Color.white;
			GUI.Box(new Rect (50, Screen.height - textureHeight / 2, scoreWidth, scoreHeight), "" + PlayerManager.Instance.Frogs[0].score + " x" + PlayerManager.Instance.Frogs[0].scoreMultiplier);
		} else {
			if(GUI.Button(new Rect(0,Screen.height-buttonHeight,buttonWidth, buttonHeight), "Play")) {
				StartPlayer(0);
			}
		}
		
		// top right = white
		if (frogObjects[3].active) {
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(Screen.width - textureWidth,0,textureWidth,textureHeight), texture);
			GUI.color = Color.white;
			GUI.Box(new Rect (Screen.width - textureWidth - scoreWidth, 0, scoreWidth, scoreHeight), "" + PlayerManager.Instance.Frogs[3].score + " x" + PlayerManager.Instance.Frogs[3].scoreMultiplier);
		} else {
			if(GUI.Button(new Rect(Screen.width - buttonWidth,0,buttonWidth, buttonHeight), "Play")) {
				StartPlayer(3);
			}
		}
		
		// bottom right = green
		if (frogObjects[1].active) {
			GUI.color = Color.green;
			GUI.DrawTexture(new Rect(Screen.width - textureWidth,Screen.height - textureHeight,textureWidth,textureHeight), texture);
			GUI.color = Color.white;
			GUI.Box(new Rect (Screen.width - textureWidth - scoreWidth, Screen.height - textureHeight / 2, scoreWidth, scoreHeight), "" + PlayerManager.Instance.Frogs[1].score + " x" + PlayerManager.Instance.Frogs[1].scoreMultiplier);
		} else {
			if(GUI.Button(new Rect(Screen.width - buttonWidth,Screen.height - buttonHeight,buttonWidth, buttonHeight), "Play")) {
				StartPlayer(1);
			}
		}
	}
	
	void StartPlayer(int num) {
		frogObjects[num].SetActiveRecursively(true);
		GameManager.Instance.Play();
	}
}
