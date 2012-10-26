using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Frog : MonoBehaviour {
	public float speed;
	public int playerNumber;
	private int charge;
	private bool wantsToBoost;
	
	private Vector3 startPosition;
	
	private Rect upperLeftBounds;
	private Rect lowerLeftBounds;
	private Rect upperRightBounds;
	private Rect lowerRightBounds;
	
	private IList<Rect> inputQuadrants;
	
	public int score;
	public int mistakes;
	
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
				return 20;
			case MoveState.Floating:
				return 5;
			case MoveState.Charging:
				return 0;
			default:
				return 0;
			}
		}
	}
	
	private int maxCharge;
	
	#region MonoBehaviour
	void Awake() {
		startPosition = transform.localPosition;
		
		inputQuadrants = new List<Rect>();
		inputQuadrants.Add(new Rect(0, 0, Screen.width/2, Screen.height/2));
		inputQuadrants.Add(new Rect(0, Screen.height/2, Screen.width/2, Screen.height/2));
		inputQuadrants.Add(new Rect(Screen.width/2, 0, Screen.width/2, Screen.height/2));
		inputQuadrants.Add(new Rect(Screen.width/2, Screen.height/2, Screen.width/2, Screen.height/2));
		
		maxCharge = 110;
		
		score = 0;
	}

	// Use this for initialization
	void Start () {
		FrogBoundary.Instance.Hit += HandleFrogBoundaryHit;
	}
	
	// Update is called once per frame
	void Update () {
		MoveForward();
		HandleInput();
		HandleState();
	}
		
	void BeginCharging() {
		if (moveState == MoveState.Floating) {
			moveState = MoveState.Charging;
			charge = 0;
			wantsToBoost = false;
		} else if (moveState == MoveState.Boosting) {
			wantsToBoost = true;	
		}
	}
	
	void BeginBoosting() {
		if (moveState == MoveState.Charging) {
			moveState = MoveState.Boosting;	
		}
	}

	void BeginFloating() {
		moveState = MoveState.Floating;
		charge = 0;
	}
	
	void HandleState() {
		if (moveState == MoveState.Charging) {
			charge+= 2;
			if (charge >= maxCharge) {
				charge = maxCharge;
				BeginBoosting();
			}
		} else if(moveState == MoveState.Boosting) {
			charge-= 5;
			if (charge <= 0) {
				BeginFloating();
			}
		} else if (moveState == MoveState.Floating) {
			if (wantsToBoost == true) {
				BeginCharging();	
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.GetComponent<Enemy>() != null) {
	    	ResetPosition();    
			score-= mistakes;
			mistakes++;
			if (score <= 0) {
				score = 0;
				mistakes = 0;
			}
		}
    }
	#endregion
	
	void MoveForward() {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Speed * Time.deltaTime);
	}
	
	void HandleInput() {
		if (Input.GetButtonDown("Fire1")) {
	        if (inputQuadrants[playerNumber].Contains(Input.mousePosition)) {
				BeginCharging();
			}
		} else if (Input.GetButtonUp("Fire1")) {
			if (inputQuadrants[playerNumber].Contains(Input.mousePosition)) {
				BeginBoosting();
			}
		}
	}
	
	void ResetPosition() {
		transform.position = startPosition;
	}
	
	void ResetState() {
		wantsToBoost = false;
		charge = 0;
		moveState = MoveState.Floating;
	}
	
	void HandleFrogBoundaryHit (GameObject other) {
		if (other == gameObject) {
			ResetPosition();
			ResetState();
			score++;	
		}
	}
}
