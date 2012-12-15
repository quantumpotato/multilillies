using UnityEngine;
using System.Collections;

public class Dam : MonoBehaviour {
	public static Dam Instance;
	
	public float moveDistanceY;
	
	Vector3 startPosition;
	Vector3 activatedPosition;
	
	enum State {
		Ready,
		Raising,
		Raised,
		Lowering
	}
	
	State currentState;
	State CurrentState {
		get {
			return currentState;
		}
		set {
			currentState = value;
		}
	}
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
		startPosition = transform.position;
		activatedPosition = new Vector3(transform.position.x, transform.position.y + moveDistanceY, transform.position.z);
	}
	
//	void OnGUI() {
//		float width = 100;
//		float height = 30;
//		if (GUI.Button(new Rect(Screen.width - width - 5, 300, width, height), "Toggle Dam")) {
//			Toggle();
//		}
//	}
	
	void OnTriggerEnter(Collider other) {
		if (Enemy.IsEnemy(other.gameObject)) {
			EnemySpawner.Instance.DestroyEnemy(other.gameObject);
		} else if (PickUp.IsPickUp(other.gameObject)) {
			PickUpSpawner.Instance.DestroyPickUp(other.gameObject);
		}
	}
	#endregion
	
	public void Raise() {
		if (CurrentState == State.Ready) {
			CurrentState = State.Raising;
			iTween.MoveTo(gameObject, iTween.Hash(
				"position", activatedPosition,
				"easetype", iTween.EaseType.easeOutQuad,
				"oncomplete", "RaiseFinished"
			));
			StartCoroutine(WaitThenLower());
		}
	}
	
	IEnumerator WaitThenLower() {
		yield return new WaitForSeconds(7);
		Lower();
	}
	
	void Lower() {
		if (CurrentState == State.Raised) {
			CurrentState = State.Lowering;
			iTween.MoveTo(gameObject, iTween.Hash(
				"position", startPosition,
				"easetype", iTween.EaseType.easeOutQuad,
				"oncomplete", "LowerFinished"
			));
		}
	}
	
	void RaiseFinished() {
		CurrentState = State.Raised;
	}
	
	void LowerFinished() {
		CurrentState = State.Ready;
	}
}
