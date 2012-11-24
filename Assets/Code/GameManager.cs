using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	
	private bool playing;
	public bool Playing {
		get {
			return playing;
		}
		
		private set {
			playing = value;
		}
	}
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
	}
	
	void Start() {
		EnemySpawner.Instance.gameObject.SetActiveRecursively(false);
	}
	#endregion
	
	//
	// Calls Play() as necessary to get the game started.
	//
	public void Play() {
		if (Playing) return;
		
		EnemySpawner.Play();
		
		Playing = true;
	}
}