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
	
	public void Play() {
		foreach (Frog frog in PlayerManager.Instance.Frogs) {
			frog.Hit += HandleFrogHit;
			frog.PickUpHit += HandleFrogPickUpHit;
		}
	}

	void HandleFrogPickUpHit(Frog frog, PickUp pickUp) {
		pickUp.ApplyTo(frog);
		PickUpSpawner.Instance.DestroyPickUp(pickUp);
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