using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Frog : MonoBehaviour {
	public float speed;
	public int playerNumber;
	public int rating;
	public int mistakes;
	public float floatExperience;
	public int floatLevel;
	public float floatLevelThreshhold;
	public float potentialFloatExperience;	
	
	public delegate void RatingChangedHandler(Frog frog);
	public event RatingChangedHandler RatingChanged;
	
	public delegate void HitHandler(Frog frog, Enemy enemy);
	public event HitHandler Hit;
	
	public delegate void PickUpHitHandler(Frog frog, PickUp pickUp);
	public event PickUpHitHandler PickUpHit;
	
	private static bool surpassing;
	public static bool Surpassing {
		get {
			return surpassing;
		}
		set {
			surpassing = value;
		}
	}
	
	public static int NumberOfPlayers {
		get {
			int sum = 0;
			foreach (Frog f in PlayerManager.Instance.Frogs) {
				if (f.gameObject.active) {
					sum++;
				}
			}
			return sum;
		}
	}
	
	public static int TotalRating {
		get {
			int sum = 0;
			foreach (Frog frog in PlayerManager.Instance.Frogs) {
				if (frog.gameObject.active) {
					sum += frog.rating;
				}
			}
			return sum;
		}
	}
	
	public static int MinRating {
		get {
			int minRating = Frog.HighRating * 4;
			
			foreach (Frog f in PlayerManager.Instance.Frogs) {
				if (f.gameObject.active) {
					if (f.rating < minRating) {
						minRating = f.rating;
					}
				}
			}
			
			return minRating;	
		}
	}
	
	private static int highRating;
	public static int HighRating {
		get {
			return highRating;
		}
		set {
			highRating = value;
		}
	}
	
	private float ModifiedFloatSpeed {
		get {
			return baseFloatingSpeed + floatLevel * floatModifier;
		}
	}
	
	private float fullPadRadius = 2.0f;
	private float baseFloatingSpeed = 2.0f;
	private float baseBoostingSpeed = 28.0f;
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
				return ModifiedFloatSpeed + baseBoostingSpeed;
			case MoveState.Floating:
				return baseFloatingSpeed;
			case MoveState.Charging:
				return 0;
			default:
				return 0;
			}
		}
	}
	
	private enum FloatLevels {
		BlueLevel,
		GreenLevel,
		YellowLevel,
		OrangeLevel,
		RedLevel,
		WhiteLevel,
		BlackLevel
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
		
		SetColor();
	}
	
	void Update () {
		MoveForward();
		HandleInput();
		HandleState();
		SetPadScale();
		SetPadColor();
	}
	
	void OnTriggerEnter(Collider other) {
		if (Enemy.IsEnemy(other.gameObject)) {
			FireHitNotification(other.gameObject);
		} else if (PickUp.IsPickUp(other.gameObject)) {
			FirePickUpHitNotification(other.gameObject);
		}
    }
	#endregion
	
	public void Die() {
		ResetPosition();
		ResetState();	
		DecreaseRating();
		FireRatingChangedNotification();
		DownGradeFloating();
	}
	
	public void UpgradeFloating() {
		floatExperience = 0;
		if (floatLevel < (int)FloatLevels.BlackLevel) {
			floatLevel += 1;
		}
	}
	
	void SetUpFloating() {
		floatExperience = 0;
		potentialFloatExperience = 0;
		floatLevelThreshhold = 200;
	}
	
	void SetUpInputQuadrants() {
		inputQuadrants = new Rect[4];
		inputQuadrants[0] = new Rect(0, 0, Screen.width/2, Screen.height/2);
		inputQuadrants[1] = new Rect(Screen.width/2, 0, Screen.width/2, Screen.height/2);
		inputQuadrants[2] = new Rect(0, Screen.height/2, Screen.width/2, Screen.height/2);
		inputQuadrants[3] = new Rect(Screen.width/2, Screen.height/2, Screen.width/2, Screen.height/2);
	}

	void DecreaseRating() {
		rating-= mistakes;
		mistakes++;
		if (rating <= 0) {
			rating = 0;
			mistakes = 0;
		}
	}
	
	void FireHitNotification(GameObject other) {
		if (Hit != null) {
			Hit(this, other.GetComponent<Enemy>());
		}
	}
	
	void FirePickUpHitNotification(GameObject other) {
		if (PickUpHit != null) {
			PickUpHit(this, other.GetComponent<PickUp>());
		}
	}
	
	void FireRatingChangedNotification() {
		if (RatingChanged != null) {
			RatingChanged(this);
		}
		if (Frog.TotalRating > Frog.HighRating) {
			Frog.HighRating = Frog.TotalRating;
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
	
	void DownGradeFloating() {
		floatExperience = 0;
		
		int levelsToDecrease = floatLevel > 0 ? 1 : 0;
		if (floatLevel > (int)FloatLevels.RedLevel) {
			levelsToDecrease = floatLevel - (int)FloatLevels.RedLevel;	
		}
		floatLevel -= levelsToDecrease;
		
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
	
	void HandleFrogBoundaryHit(GameObject other) {
		if (other == gameObject) {
			ResetPosition();
			ResetState();
			rating++;
			UpgradeFloating();
			FireRatingChangedNotification();
		}
	}
		
	void SetPadColor() {
		switch (floatLevel) {
		case (int)FloatLevels.BlueLevel:
			pad.renderer.material.color = Color.blue;
			break;
		case (int)FloatLevels.GreenLevel:
			pad.renderer.material.color = Color.green;
			break;
		case (int)FloatLevels.YellowLevel:
			pad.renderer.material.color = Color.yellow;
			break;
		case (int)FloatLevels.OrangeLevel:
			pad.renderer.material.color = new Color(255.0f/255.0f, 140.0f/255.0f, 0, 1);
			break;
		case (int)FloatLevels.RedLevel:
			pad.renderer.material.color = Color.red;
			break;
		case (int)FloatLevels.WhiteLevel:
			pad.renderer.material.color = Color.white;
			break;
		case (int)FloatLevels.BlackLevel:
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
