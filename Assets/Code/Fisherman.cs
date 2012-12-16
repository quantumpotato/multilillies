using UnityEngine;
using System.Collections;

public class Fisherman : MonoBehaviour {
	public GameObject activePosition;
	public int enemiesToCatch;
	
	public static Fisherman Instance;
	public enum State {
		Inactive = 0,
		MovingIntoPosition,
		Casting,
		WaitingForCollision,
		Catching,
		MovingBackIntoStartPosition
	}
	State currentState;
	public State CurrentState {
		get {
			return currentState;
		}
		set {
			currentState = value;
		}
	}
	public delegate void StateChangedHandler(State oldState, State newState);
	public event StateChangedHandler StateChanged;
	
	int enemiesCaught;
	Vector3 startPosition;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
		startPosition = transform.position;
	}
	
	void OnGUI() {
//		DrawFishermanPickupButton();
		if (IsActive()) {
			DrawEnemiesCaught();
		}
	}
	#endregion
	
	public void StartSequence() {
		if (CurrentState == State.Inactive) {
			MoveIntoPosition();
		}
	}
	
	public void EndSequence() {
		enemiesCaught = 0;
		MoveBackToStartPosition();
	}
	
	public void SetInactive() {
		ChangeState(State.Inactive);
	}
	
	public bool IsActive() {
		return CurrentState != State.Inactive;
	}
	
	public bool CanCatchEnemies() {
		return CurrentState == State.WaitingForCollision && enemiesCaught != enemiesToCatch;
	}
	
	public void FinishCatching(Enemy enemy) {
		EnemySpawner.Instance.DestroyEnemy(enemy.gameObject);
		CheckIfFinished();
	}
	
//	void DrawFishermanPickupButton() {
//		int width = 200;
//		int height = 50;
//		if (GUI.Button (new Rect(Screen.width - width - 5, 240, width, height), "Fisherman\n" + CurrentState)) {
//			OnFishermanButtonClick();
//		}
//	}
	
	void DrawEnemiesCaught() {
		GUI.Box(new Rect(Screen.width - 105, 60, 100, 23), enemiesCaught + " / " + enemiesToCatch);
	}
	
//	void OnFishermanButtonClick() {
//		StartSequence();
//	}
	
	void MoveBackToStartPosition() {
		ChangeState(State.MovingBackIntoStartPosition);
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", startPosition,
			"easetype", iTween.EaseType.linear,
			"time", 2,
			"oncomplete", "SetInactive"
		));
	}
	
	void MoveIntoPosition() {
		ChangeState(State.MovingIntoPosition);
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", activePosition.transform,
			"easetype", iTween.EaseType.linear,
			"time", 2,
			"oncomplete", "StartCasting"
		));
	}
	
	void StartCasting() {
		ChangeState(State.Casting);
		StartCoroutine(WaitThenCast());
	}
	
	IEnumerator WaitThenCast() {
		yield return new WaitForSeconds(1.0f);
		
		// TODO: do casting stuff here...
		
		StartWaitingForCollision();
	}
	
	void StartWaitingForCollision() {
		PlayerManager.Instance.FrogHit += CatchEnemy;
		ChangeState(State.WaitingForCollision);
	}
	
	void CatchEnemy(Frog frog, Enemy enemy) {
		if (enemiesCaught == enemiesToCatch) return;
		ChangeState(State.Catching);
		enemiesCaught++;
		PlayerManager.Instance.FrogHit -= CatchEnemy;
		enemy.GetCaughtBy(this);
	}
	
	void CheckIfFinished() {
		if (enemiesCaught == enemiesToCatch) {
			EndSequence();
		} else {
			StartCasting();
		}
	}
	
	void ChangeState(State newState) {
		State oldState = CurrentState;
		CurrentState = newState;
		if (StateChanged != null) {
			StateChanged(oldState, newState);
		}
	}
}
