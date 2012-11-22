using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Frog : MonoBehaviour {
	public float speed;
	public int playerNumber;
	public int score;
	public int mistakes;
		
	public delegate void ScoreChangedHandler(Frog frog);
	public event ScoreChangedHandler ScoreChanged;
	
	public delegate void HitHandler(Frog frog, Enemy other);
	public event HitHandler Hit;
	
	
	private static bool surpassing;
	public static bool Surpassing {
		get {
			return surpassing;
		}
		set {
			surpassing = value;
		}
	}
	
	public static int TotalScore {
		get {
			int sum = 0;
			foreach (Frog f in Players) {
				sum += f.score;
			}
			return sum;
		}
	}
	
	public static int MinScore {
		get {
			int minScore = Frog.HighScore * 4;

	
			foreach (Frog f in Frog.Players) {
				if (f.score < minScore) {
					minScore = f.score;
				}
			}
			return minScore;	
		}
	}
	
	private static int highScore;
	public static int HighScore {
		get {
			return highScore;
		}
		set {
			highScore = value;
		}
	}
	
	public static Frog[] players;
	public static Frog[] Players{
		get {
			if (players == null) {
				players = (Frog[])GameObject.FindObjectsOfType(typeof(Frog));
			}	
			return players;
		}
	}
	
	private int charge;
	private bool wantsToBoost;
	
	private Vector3 startPosition;
	
	private Rect upperLeftBounds;
	private Rect lowerLeftBounds;
	private Rect upperRightBounds;
	private Rect lowerRightBounds;
	
	private IList<Rect> inputQuadrants;
	
	private GameObject pad;
	
	
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
				return 24;
			case MoveState.Floating:
				return 2;
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
		SetUpInputQuadrants();
		
		startPosition = transform.position;
		maxCharge = 50;
		score = 0;
	}

	void Start () {
		FrogBoundary.Instance.Hit += HandleFrogBoundaryHit;
		pad = transform.FindChild("pad").gameObject;
	}
	
	void Update () {
		MoveForward();
		HandleInput();
		HandleState();
		AnimatePad();
	}
	
	void OnTriggerEnter(Collider other) {
		if (IsEnemy(other.gameObject)) {
			FireHitNotification(other.gameObject);
		}
    }
	#endregion
	
	public void Die() {
		ResetPosition();
		ResetState();		
		DecreaseScore();
		FireScoreChangedNotification();
	}
	
	void SetUpInputQuadrants() {
		inputQuadrants = new List<Rect>();
		inputQuadrants.Add(new Rect(0, 0, Screen.width/2, Screen.height/2));
		inputQuadrants.Add(new Rect(0, Screen.height/2, Screen.width/2, Screen.height/2));
		inputQuadrants.Add(new Rect(Screen.width/2, 0, Screen.width/2, Screen.height/2));
		inputQuadrants.Add(new Rect(Screen.width/2, Screen.height/2, Screen.width/2, Screen.height/2));
	}
	
	bool IsEnemy(GameObject other) {
		return other.GetComponent<Enemy>() != null;
	}
	
	void DecreaseScore() {
		score-= mistakes;
		mistakes++;
		if (score <= 0) {
			score = 0;
			mistakes = 0;
		}
	}
	
	void FireHitNotification(GameObject other) {
		if (Hit != null) {
			Hit(this, other.GetComponent<Enemy>());
		}
	}
	
	void FireScoreChangedNotification() {
		if (ScoreChanged != null) {
			ScoreChanged(this);
		}
		if (Frog.TotalScore > Frog.HighScore) {
			Frog.HighScore = Frog.TotalScore;
			surpassing = true;
		} else {
			surpassing = false;
		}
	}
	
	void AnimatePad() {
		Vector3 scale = pad.transform.localScale;
		scale.x = 2.0f - (float)charge / (float)maxCharge;
		scale.z = 2.0f - (float)charge / (float)maxCharge;
		pad.transform.localScale = scale;
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
				boostLevel++;
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
	
	void MoveForward() {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Speed * Time.deltaTime);
	}
	
	void HandleInput() {
#if UNITY_EDITOR
		if (Input.GetButtonDown("Fire1")) {
	        if (inputQuadrants[playerNumber].Contains(Input.mousePosition)) {
				BeginCharging();
			}
		} else if (Input.GetButtonUp("Fire1")) {
			if (inputQuadrants[playerNumber].Contains(Input.mousePosition)) {
				BeginBoosting();
			}
		}
#elif UNITY_IPHONE
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began && inputQuadrants[playerNumber].Contains(touch.position)) {
				BeginCharging();
			} else if (touch.phase == TouchPhase.Ended && inputQuadrants[playerNumber].Contains(touch.position)) {
				BeginBoosting();
			}
		}
#endif
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
			FireScoreChangedNotification();
		}
	}
	
	void ChangePadColor(int boostLevel) {
		switch (boostLevel) {
		case 1:
			pad.renderer.material.color = Color.blue;
			break;
		case 2:
			pad.renderer.material.color = Color.magenta;
			break;
		case 3:
			pad.renderer.material.color = Color.yellow;
			break;
		case 4:
			pad.renderer.material.color = Color.red;
			break;
		case 5:
			pad.renderer.material.color = Color.black;
			break;
		default:
			pad.renderer.material.color = Color.clear;
			break;
		}
	}
	
	
}
