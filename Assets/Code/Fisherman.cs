using UnityEngine;
using System.Collections;

public class Fisherman : MonoBehaviour {
	public GameObject activePosition;
	public int timesToSave;
	
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
	
	int timesSaved;
	Vector3 startPosition;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
		startPosition = transform.position;
	}
	
	void OnGUI() {
		DrawFishermanPickupButton();
	}
	#endregion
	
	public void MakeActive() {
		if (CurrentState == State.Inactive) {
			MoveIntoPosition();
		}
	}
	
	void DrawFishermanPickupButton() {
		int width = 200;
		int height = 50;
		if (GUI.Button (new Rect(470, 5, width, height), "Fisherman\n" + CurrentState)) {
			OnFishermanButtonClick();
		}
	}
	
	void OnFishermanButtonClick() {
		MakeActive();
	}
	
	void MoveBackToStartPosition() {
		ChangeState(State.MovingBackIntoStartPosition);
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", startPosition,
			"easetype", iTween.EaseType.linear,
			"time", 2
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
		
		// do casting stuff here...
		
		StartWaitingForCollision();
	}
	
	void StartWaitingForCollision() {
		ChangeState(State.WaitingForCollision);
		
		//TODO
		StartCoroutine(WaitThenMoveBackToStartPosition());
	}
	
	IEnumerator WaitThenMoveBackToStartPosition() {
		yield return new WaitForSeconds(3.0f);
		MoveBackToStartPosition();
	}
	
	void ChangeState(State newState) {
		State oldState = CurrentState;
		CurrentState = newState;
		if (StateChanged != null) {
			StateChanged(oldState, newState);
		}
	}
}
