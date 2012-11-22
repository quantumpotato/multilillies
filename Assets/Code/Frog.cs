using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Frog : MonoBehaviour {
	public float speed;
	public int playerNumber;
	public int score;
	public int mistakes;
	public float floatModifier;
	public int floatExperience;
	public int floatLevelThreshhold;
	public int potentialFloatExperience;
		
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
				return 30;
			case MoveState.Floating:
				return 4;
			case MoveState.Charging:
				return 0;
			default:
				return 0;
			}
		}
	}
	
	private int maxCharge;
	
	void SetupBoosting() {
		floatModifier = 1;
		floatExperience = 0;
		potentialFloatExperience = 0;
		floatLevelThreshhold = 100;
	}
		
	#region MonoBehaviour
	void Awake() {
		SetUpInputQuadrants();
		SetupBoosting();
		startPosition = transform.position;
		maxCharge = 70;
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
		DownGradeBoost();
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
	
	float FloatExperiencePercentage() {
	 	return (float)floatExperience / (float)floatLevelThreshhold;	
	}
		

	void BeginFloating() {
		moveState = MoveState.Floating;
		charge = 0;
	}
	
	void UpgradeFloating() {
		potentialFloatExperience = 0;
		floatExperience = 0;
		floatLevelThreshhold *= 2;	
		floatModifier *= 2;
	}
	
	void DownGradeFloating() {
		boostExperience = 0;
		if (floatModifier > 1) {
			boostLevelThreshhold /= 2;
			floatModifier /= 2;
		}
	}
		
	
	void GainFloatExperience() {
		floatExperience += potentialFloatExperience;
		if (floatExperience > floatLevelThreshhold) {
			UpgradeFloating();
		}
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
			potentialFloatExperience++;
			if (charge <= 0) {
				BeginFloating();
				GainFloatExperience();
			}
		} else if (moveState == MoveState.Floating) {
			if (wantsToBoost == true) {
				BeginCharging();	
			}
		}
	}
	
	float CurrentSpeed() {
		float currentSpeed = Speed * Time.deltaTime;
		if (moveState == MoveState.Floating) {
			currentSpeed *= floatModifier;
		}
		return currentSpeed;
	}
	
	void MoveForward() {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + CurrentSpeed());
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
		potentialBoostExperience = 0;
	}
	
	void HandleFrogBoundaryHit (GameObject other) {
		if (other == gameObject) {
			ResetPosition();
			ResetState();
			score++;	
			FireScoreChangedNotification();
		}
	}
}
