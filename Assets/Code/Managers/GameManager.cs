using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	
	StateType state;
	enum StateType {
		MainMenu,
		Starting,
		Playing,
		Paused,
		Ending,
		GameOver
	}
	
	public enum GameMode {
		Cooperative,
		Competitive
	}
	public GameMode mode;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
		state = StateType.MainMenu;
		mode = GameMode.Cooperative;
	}
	
	void Start() {
		EnemySpawner.Instance.gameObject.SetActiveRecursively(false);
		PickUpSpawner.Instance.gameObject.SetActiveRecursively(false);
		RuneManager.Instance.gameObject.SetActiveRecursively(false);
		PlayerManager.Instance.FrogHit += HandleFrogHit;
	}
	#endregion
	
	public bool IsCoopMode() {
		return mode == GameMode.Cooperative;	
	}
	
	public bool IsCompetitiveMode() {
		return mode == GameMode.Competitive;
	}	
	
	public bool IsMainMenu() {
		return state == StateType.MainMenu;
	}
	
	public bool IsStarting() {
		return state == StateType.Starting;
	}
	
	public bool IsPlaying() {
		return state == StateType.Playing;
	}
	
	public bool IsPaused() {
		return state == StateType.Paused;
	}
	
	public bool IsGameOver() {
		return state == StateType.GameOver;
	}
	
	//
	// Calls Play() as necessary to get the game started.
	//
	public void Play() {
		if (IsPlaying()) return;
		state = StateType.Starting;
		
		EnemySpawner.Play();
		PickUpSpawner.Play();
		PlayerManager.Play();
		RuneManager.Play();
		
		state = StateType.Playing;
	}
	
	void HandleFrogHit(Frog frog, Enemy enemy) {
		if (PlayerManager.Instance.Lives == 0) {
			GameOver();
		}
	}
	
	void GameOver() {
		state = StateType.Ending;
		
		EnemySpawner.Stop();
		PickUpSpawner.Stop();
		PlayerManager.Stop();
		RuneManager.Stop();
		
		state = StateType.GameOver;
	}
}
