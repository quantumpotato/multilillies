using UnityEngine;
using System.Collections;

public class FrogHitManager : MonoBehaviour {
	public delegate void FrogHitHandler(Frog frog, Enemy enemy);
	public event FrogHitHandler FrogHit;
	
	public static FrogHitManager Instance;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
	}
	
	void Start() {
		foreach (Frog frog in Frog.Players) {
			frog.Hit += HandleFrogHit;
		}
	}

	void HandleFrogHit(Frog frog, Enemy enemy) {
		if (!Fisherman.Instance.CanCatchEnemies()) {
			frog.Die();
		}
		if (FrogHit != null) {
			FrogHit(frog, enemy);
		}
	}
	#endregion
}
