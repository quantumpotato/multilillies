using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager Instance;
	public static void Play() {
		foreach (Frog frog in Instance.Frogs) {
			frog.Hit += Instance.HandleFrogHit;
			frog.PickUpHit += Instance.HandleFrogPickUpHit;
		}
		Instance.lives = Instance.startLives;
	}
	public static void Stop() {
		foreach (Frog frog in Instance.Frogs) {
			frog.Hit -= Instance.HandleFrogHit;
			frog.PickUpHit -= Instance.HandleFrogPickUpHit;
			frog.rating = 0;
			frog.gameObject.SetActiveRecursively(false);
		}
		Frog.HighRating = 0;
	}
	
	public int lives;
	public int Lives {
		get {
			return lives;
		}
		
		set {
			lives = value;
			if (lives < 0) {
				lives = 0;
			}
		}
	}
	
	public delegate void LivesExpiredHandler();
	public event LivesExpiredHandler LivesExpired;
	
	public delegate void FrogHitHandler(Frog frog, Enemy enemy);
	public event FrogHitHandler FrogHit;
	
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
	
	int startLives;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
		frogObjects = new GameObject[4];
		frogs = new Frog[4];
		startLives = lives;
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
		DrawLives();
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
		
		// top left = red
		if (frogObjects[2].active) {
			GUI.color = Color.red;
			GUI.DrawTexture(new Rect(0,0,textureWidth,textureHeight), texture);
		} else {
			if(GUI.Button(new Rect(0, 0, buttonWidth, buttonHeight), "Play")) {
				StartPlayer(2);
			}
		}
		
		// bottom left = yellow
		if (frogObjects[0].active) {
			GUI.color = Color.yellow;
			GUI.DrawTexture(new Rect(0,Screen.height - textureHeight,textureWidth,textureHeight), texture);
		} else {
			if(GUI.Button(new Rect(0,Screen.height-buttonHeight,buttonWidth, buttonHeight), "Play")) {
				StartPlayer(0);
			}
		}
		
		// top right = white
		if (frogObjects[3].active) {
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(Screen.width - textureWidth,0,textureWidth,textureHeight), texture);
		} else {
			if(GUI.Button(new Rect(Screen.width - buttonWidth,0,buttonWidth, buttonHeight), "Play")) {
				StartPlayer(3);
			}
		}
		
		// bottom right = green
		if (frogObjects[1].active) {
			GUI.color = Color.green;
			GUI.DrawTexture(new Rect(Screen.width - textureWidth,Screen.height - textureHeight,textureWidth,textureHeight), texture);
		} else {
			if(GUI.Button(new Rect(Screen.width - buttonWidth,Screen.height - buttonHeight,buttonWidth, buttonHeight), "Play")) {
				StartPlayer(1);
			}
		}
	}
	
	void DrawLives() {
		if (GameManager.Instance.IsPlaying()) {
			GUI.Box(new Rect(120, 20, 100, 35), "Lives: " + Lives.ToString());
		}
	}
	
	void StartPlayer(int num) {
		frogObjects[num].SetActiveRecursively(true);
		GameManager.Instance.Play();
	}

	void HandleFrogPickUpHit(Frog frog, PickUp pickUp) {
		pickUp.ApplyTo(frog);
		PickUpSpawner.Instance.DestroyPickUp(pickUp);
	}

	void HandleFrogHit(Frog frog, Enemy enemy) {
		if (!Fisherman.Instance.CanCatchEnemies()) {
			frog.Die();
			lives--;
		}
		if (FrogHit != null) {
			FrogHit(frog, enemy);
		}
	}
}
