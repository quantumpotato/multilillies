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
		PickUpSpawner.Instance.gameObject.SetActiveRecursively(false);
		RuneManager.Instance.gameObject.SetActiveRecursively(false);
	}
	#endregion
	
	//
	// Calls Play() as necessary to get the game started.
	//
	public void Play() {
		if (Playing) return;
		
		EnemySpawner.Play();
		PickUpSpawner.Play();
		FrogHitManager.Instance.Play();
		RuneManager.Play();
		
		Playing = true;
	}
}
