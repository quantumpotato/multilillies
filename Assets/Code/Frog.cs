using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Frog : MonoBehaviour {
	public float speed;
	public int playerNumber;
	public int score;
	public int mistakes;
	public float floatExperience;
	public int floatLevel;
	public float floatLevelThreshhold;
	public float potentialFloatExperience;	
	
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
	
			foreach (Frog f in PlayerManager.Instance.Frogs) {
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
	
	private float ModifiedFloatSpeed {
		get {
			return baseFloatingSpeed + floatLevel * floatModifier;
		}
	}
	
	private float fullPadRadius = 2.0f;
	private float baseFloatingSpeed = 2.0f;
	private float baseBoostingSpeed = 30.0f;
	private float floatModifier = 1.5f;
	private float maxCharge = 70;
	private float charge;
	private float chargeReached;
	private float chargeIncreaseSpeed = 130;
	private float chargeDecreaseSpeed = 280;
	private float potentialFloatExperienceIncreaseSpeed = 200;
	private float potentialExperienceThreshold = 30;
	private bool wantsToBoost;
	
	private Vector3 startPosition;
	
	private Rect upperLeftBounds;
	private Rect lowerLeftBounds;
	private Rect upperRightBounds;
	private Rect lowerRightBounds;
	private Rect[] inputQuadrants;
	
	private GameObject character;
	private GameObject pad;
	private GameObject floatExperienceCircle;
	private GameObject fullFloatExperienceCircle;
	
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
				return baseBoostingSpeed;
			case MoveState.Floating:
				return baseFloatingSpeed;
			case MoveState.Charging:
				return 0;
			default:
				return 0;
			}
		}
	}
		
	#region MonoBehaviour
	void Awake() {
		SetUpInputQuadrants();
		SetUpFloating();
		startPosition = transform.position;
	}

	void Start () {
		FrogBoundary.Instance.Hit += HandleFrogBoundaryHit;
		
		character = transform.FindChild("character").gameObject;
		pad = transform.FindChild("pad").gameObject;
		floatExperienceCircle = transform.FindChild("floatExperienceCircle").gameObject;
		fullFloatExperienceCircle = transform.FindChild("fullFloatExperienceCircle").gameObject;
		
		SetFullFloatExperienceCircleScale();
		
		SetColor();
	}
	
	void Update () {
		MoveForward();
		HandleInput();
		HandleState();
		SetPadScale();
		SetFloatExperienceCircleScale();
		SetPadColor();
	}
	
	void OnTriggerEnter(Collider other) {
		if (IsEnemy(other.gameObject)) {
			FireHitNotification(other.gameObject);
		}
    }
	#endregion
	
	void SetUpFloating() {
		floatExperience = 0;
		potentialFloatExperience = 0;
		floatLevelThreshhold = 200;
	}
	
	public void Die() {
		ResetPosition();
		ResetState();	
		DecreaseScore();
		FireScoreChangedNotification();
		DownGradeFloating();
	}
	
	void SetUpInputQuadrants() {
		inputQuadrants = new Rect[4];
		inputQuadrants[0] = new Rect(0, 0, Screen.width/2, Screen.height/2);
		inputQuadrants[1] = new Rect(Screen.width/2, 0, Screen.width/2, Screen.height/2);
		inputQuadrants[2] = new Rect(0, Screen.height/2, Screen.width/2, Screen.height/2);
		inputQuadrants[3] = new Rect(Screen.width/2, Screen.height/2, Screen.width/2, Screen.height/2);
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
	
	void SetPadScale() {
		Vector3 scale = pad.transform.localScale;
		scale.x = PadRadius();
		scale.z = PadRadius();
		pad.transform.localScale = scale;
	}
	
	float PadRadius() {
		return fullPadRadius - ChargePercentage();
	}
	
	float ChargePercentage() {
		return (float)charge / (float)maxCharge;
	}
	
	void SetFloatExperienceCircleScale() {
		Vector3 scale = floatExperienceCircle.transform.localScale;
		scale.x = FloatExperienceRadius();
		scale.z = FloatExperienceRadius();
		floatExperienceCircle.transform.localScale = scale;
	}
	
	void SetFullFloatExperienceCircleScale() {
		Vector3 scale = fullFloatExperienceCircle.transform.localScale;
		scale.x = fullPadRadius + (fullPadRadius/2);
		scale.z = fullPadRadius + (fullPadRadius/2);
		fullFloatExperienceCircle.transform.localScale = scale;
	}
	
	float FloatExperienceRadius() {
		return fullPadRadius + FloatExperiencePercentage() * (fullPadRadius/2);
	}
	
	void BeginCharging() {
		if (moveState == MoveState.Floating) {
			moveState = MoveState.Charging;
			charge = 0;
			potentialFloatExperience = 0;				
			wantsToBoost = false;
		} else if (moveState == MoveState.Boosting) {
			wantsToBoost = true;	
		}
	}
	
	void BeginBoosting() {
		if (moveState == MoveState.Charging) {
			chargeReached = charge;
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
		floatLevel += 1;
	}
	
	void DownGradeFloating() {
		floatExperience = 0;
		if (floatLevel > 0) {
			floatLevelThreshhold /= 2;
			floatLevel--;
		}
	}
	
	void GainFloatExperience() {
		if (potentialFloatExperience >= potentialExperienceThreshold) {
			floatExperience += potentialFloatExperience - potentialExperienceThreshold;
			print ("gained " + floatExperience);
		} else {
			print("potential float exp " + potentialFloatExperience + " / " + potentialExperienceThreshold);
		}
		if (floatExperience > floatLevelThreshhold) {
			UpgradeFloating();
		}
	}
	
	void HandleState() {
		if (moveState == MoveState.Charging) {
			charge += (chargeIncreaseSpeed * Time.deltaTime);
			potentialFloatExperience += (potentialFloatExperienceIncreaseSpeed * Time.deltaTime);
			if (charge >= maxCharge) {
				charge = maxCharge;
				BeginBoosting();
			}
		} else if(moveState == MoveState.Boosting) {
			charge -= (chargeDecreaseSpeed * Time.deltaTime);
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
		float currentSpeed = Speed;
		if (moveState == MoveState.Floating) {
			currentSpeed = ModifiedFloatSpeed;
		} else if (moveState == MoveState.Boosting) {
//			if (ChargePercentage() < 0.5f) {
//				currentSpeed = ((1 - ((float)charge / (float)chargeReached)) * currentSpeed) * Time.deltaTime;
//			}
		}
		
		return currentSpeed * Time.deltaTime;
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
		potentialFloatExperience = 0;
	}
	
	void HandleFrogBoundaryHit (GameObject other) {
		if (other == gameObject) {
			ResetPosition();
			ResetState();
			score++;	
			FireScoreChangedNotification();
		}
	}
	
	void SetPadColor() {
		switch (floatLevel) {
		case 0:
			pad.renderer.material.color = Color.blue;
			break;
		case 1:
			pad.renderer.material.color = Color.magenta;
			break;
		case 2:
			pad.renderer.material.color = Color.yellow;
			break;
		case 3:
			pad.renderer.material.color = Color.red;
			break;
		case 4:
			pad.renderer.material.color = Color.black;
			break;
		default:
			pad.renderer.material.color = Color.black;
			break;
		}
	}
	
	void SetColor() {
		character.renderer.material.color = PlayerManager.Instance.GetPlayerColor(playerNumber);
	}
}
