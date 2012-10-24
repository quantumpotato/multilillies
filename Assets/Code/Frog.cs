using UnityEngine;
using System.Collections;

public class Frog : MonoBehaviour {
	
	public float speed;
	
	private Vector3 startPosition;
	private float startSpeed;
	
	private Rect upperLeftBounds;
	private Rect lowerLeftBounds;
	private Rect upperRightBounds;
	private Rect lowerRightBounds;
	
	private enum MoveState {
		Floating,
		Charging,
		Boosting
	}
	private MoveState moveState;
	
	private float Speed {
		get {
			switch (moveState) {
			case MoveState.Boosting:
				return 15;
			case MoveState.Floating:
				return 5;
			case MoveState.Charging:
				return 0;
			default:
				return 0;
			}
		}
	}
	
	#region MonoBehaviour
	void Awake() {
		startPosition = transform.localPosition;
		startSpeed = speed;
		
		lowerLeftBounds = new Rect(0, 0, Screen.width/2, Screen.height/2);
		upperLeftBounds = new Rect(0, Screen.height/2, Screen.width/2, Screen.height/2);
		lowerRightBounds = new Rect(Screen.width/2, 0, Screen.width/2, Screen.height/2);
		upperRightBounds = new Rect(Screen.width/2, Screen.height/2, Screen.width/2, Screen.height/2);
	}

	// Use this for initialization
	void Start () {
		FrogBoundary.Instance.Hit += HandleFrogBoundaryHit;
	}
	
	// Update is called once per frame
	void Update () {
		MoveForward();
		HandleInput();
	}
	
	void OnCollisionEnter(Collision collision) {
    	ResetPosition();    
    }
	#endregion
	
	void MoveForward() {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Speed * Time.deltaTime);
	}
	
	void HandleInput() {
		if (Input.GetButtonDown("Fire1")) {
	        if (upperLeftBounds.Contains(Input.mousePosition)) {
//	            Debug.Log("upper Left!");
				
	        } else if (lowerLeftBounds.Contains(Input.mousePosition)) {
//				Debug.Log("lower Left!");
			} else if (upperRightBounds.Contains(Input.mousePosition)) {
//				Debug.Log("upper Right!");
			} else if (lowerRightBounds.Contains(Input.mousePosition)) {
//				Debug.Log("lower Right!");
			}
		}
	}
	
	void ResetPosition() {
		transform.position = startPosition;
	}
	
	void HandleFrogBoundaryHit (GameObject other) {
		if (other == gameObject) {
			ResetPosition();
		}
	}
}
