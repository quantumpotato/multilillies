using UnityEngine;
using System.Collections;

public class Dam : MonoBehaviour {
	public float moveDistanceY;
	
	Vector3 startPosition;
	Vector3 activatedPosition;
	bool up;
	
	#region MonoBehaviour
	void Awake() {
		startPosition = transform.position;
		activatedPosition = new Vector3(transform.position.x, transform.position.y + moveDistanceY, transform.position.z);
	}
	
	void OnGUI() {
		float width = 100;
		float height = 30;
		if (GUI.Button(new Rect(Screen.width - width - 5, 300, width, height), "Toggle Dam")) {
			Toggle();
		}
	}
	
	void OnTriggerEnter(Collider other) {
		Enemy enemy = other.gameObject.GetComponent<Enemy>();
		if (enemy != null) {
			EnemySpawner.Instance.DestroyEnemy(enemy.gameObject);
		}
	}
	#endregion
	
	void MoveUp() {
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", activatedPosition,
			"easetype", iTween.EaseType.easeOutQuad
		));
	}
	
	void MoveDown() {
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", startPosition,
			"easetype", iTween.EaseType.easeOutQuad
		));
	}
	
	void Toggle() {
		if (up) {
			MoveDown();
		} else {
			MoveUp();
		}
		up = !up;
	}
}
