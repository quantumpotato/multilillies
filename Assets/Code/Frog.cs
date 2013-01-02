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
	public int score;
	public int scoreMultiplier;
	public int coins;
	public int floatDirection;
	
	public int[] powerupQuantities;
	
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
	private static PowerUp[] inventoryPowerups;
	public static PowerUp[] InventoryPowerups {
		get {
			if (inventoryPowerups != null) {
				return inventoryPowerups;
			}
			inventoryPowerups = new PowerUp[]{new UpgradeFloating(), new RaiseDam(), new SummonFisherman()};
			return inventoryPowerups;
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

	public static int TotalScore {
		get {
			int sum = 0;
			foreach (Frog frog in PlayerManager.Instance.Frogs) {
				if (frog.gameObject.active) {
					sum+= frog.score;
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
	
	private PowerUp[] inventory;
	public PowerUp[] Inventory {
		get {
			return inventory;
		}
		
		set {
			inventory = value;
		}
	}
	
	public static float MiddleOfTheStreamZ {
		get {
			return 17;
		}
	}
	
	private float ModifiedFloatSpeed {
		get {
			return baseFloatingSpeed + floatLevel * floatModifier;
		}
	}
	
	public static int TeamScoreMultiplier {
		get {
			int teamMultiplier = 0;
			
			foreach (Frog f in PlayerManager.Instance.Frogs) {
				if (f.gameObject.active) {
					teamMultiplier+= f.scoreMultiplier;
				}
			}
				
			return teamMultiplier;
		}
	}
	
	private static Color[] frogColors = new Color[]{Color.yellow, Color.green, Color.red, Color.white};
	public static Color[] FrogColors {
		get {
			return frogColors;
		}
	}
	
	private float fullPadRadius = 2.0f;
	private float baseFloatingSpeed = 2.0f;
	private float baseBoostingSpeed = 42.0f;
	private float floatModifier = 1.5f;
	private float maxCharge = 70;
	private float charge;
	private float chargeReached;
	private float chargeIncreaseSpeed = 130;
	private float chargeDecreaseSpeed = 420;
	private float potentialFloatExperienceIncreaseSpeed = 200;
	private float potentialExperienceThreshold = 30;
	private bool wantsToBoost;
	
	private Vector3 startPosition;
	
	private Rect upperLeftBounds;
	private Rect lowerLeftBounds;
	private Rect upperRightBounds;
	private Rect lowerRightBounds;
	private Rect[] inputQuadrants;
	private Rect[] inventoryRects;
	
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
		scoreMultiplier = 1;
		ResetFloatDirection();
		SetUpInputQuadrants();
		SetUpInventoryRects();
		SetUpFloating();
		startPosition = transform.position;
		Inventory = new PowerUp[3];
		powerupQuantities = new int[3];		
	}

	void Start () {
		FrogBoundary.NorthInstance.Hit += HandleFrogBoundaryHit;
		FrogBoundary.SouthInstance.Hit += HandleFrogBoundaryHit;		
		
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
	
	void OnGUI() {
		DrawInventory();
	}
	#endregion
	
	int FrogScoreMultiplier() {
		if (GameManager.Instance.IsCompetitiveMode()) {
			return scoreMultiplier;	
		}
		
		return Frog.TeamScoreMultiplier;
	}
	
	public void CollectCoin() {
		print ("collecting coin" + score +"" + FrogScoreMultiplier());
		coins++;
	}
	
	public void AddToInventory(PowerUp powerUp) {
	    int powerUpIndex = 0;
		for (int i = 0; i < InventoryPowerups.Length; i++) {
			if (powerUp.Name == InventoryPowerups[i].Name) {
				powerUpIndex = i;
				break;
			}
		}
		powerupQuantities[powerUpIndex]++;
		
		for (int i = 0; i < Inventory.Length; i++) {
			PowerUp item = Inventory[i];
			if (item == null) {
				Inventory[i] = powerUp;
				return;
			}
		}
	}
	
	void ResetScoreMultiplierToFloatLevel() {
		scoreMultiplier = floatLevel + 1;
	}
	
	void ResetFloatDirection() {
		floatDirection = 1;
	}
	
	public void Die() {
		coins = 0;	
		ResetFloatDirection();
		ResetPosition();
		ResetState();	
		DownGradeFloating();
		ResetScoreMultiplierToFloatLevel();
		DecreaseRating();
		FireRatingChangedNotification();

	}
	
	public void UpgradeFloating() {
		floatExperience = 0;
		if (floatLevel < (int)FloatLevels.BlackLevel) {
			floatLevel += 1;
		}
		scoreMultiplier++;
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
	
	void SetUpInventoryRects() {
		inventoryRects = new Rect[4];
		int width = 400;
		int height = 40;
		int leftStart = 10;
		int topStart = 100;
		
		inventoryRects[2] = new Rect(leftStart, topStart, width, height);
		
		if (playerNumber == 3) {
			// upper right
			leftStart = Screen.width - width - 5;
			topStart = 100;
			inventoryRects[3] = new Rect(leftStart, topStart, width, height);
		} else if (playerNumber == 0) {
			// lower left
			leftStart = 10;
			topStart = Screen.height - 100;
			inventoryRects[0] = new Rect(leftStart, topStart, width, height);
		} else if (playerNumber == 1) {
			// lower right
			leftStart = Screen.width - width - 5;
			topStart = Screen.height - 100;
			inventoryRects[1] = new Rect(leftStart, topStart, width, height);
		}
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
		}
		
		return currentSpeed * Time.deltaTime * floatDirection;
	}
	
	void MoveForward() {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + CurrentSpeed());
	}
	
	void HandleInput() {
#if UNITY_EDITOR
		if (Input.GetButtonDown("Fire1")) {
	        if (inputQuadrants[playerNumber].Contains(Input.mousePosition)
				&& !inventoryRects[playerNumber].Contains(Input.mousePosition)) {
				BeginCharging();
			}
		} else if (Input.GetButtonUp("Fire1")) {
			if (inputQuadrants[playerNumber].Contains(Input.mousePosition)
				&& !inventoryRects[playerNumber].Contains(Input.mousePosition)) {
				BeginBoosting();
			}
		}
#elif UNITY_IPHONE
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began
				&& inputQuadrants[playerNumber].Contains(touch.position)
				&& !inventoryRects[playerNumber].Contains(touch.position)) {
				BeginCharging();
			} else if (touch.phase == TouchPhase.Ended
				&& inputQuadrants[playerNumber].Contains(touch.position)
				&& !inventoryRects[playerNumber].Contains(touch.position)) {
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
	
	void ScoreFromCoinsDelivered() {
		score = score + (coins * FrogScoreMultiplier());
		print("coins: " + coins + "scoreMultiplier" + FrogScoreMultiplier());
		coins = 0;
	}
	
	void IncrementRating() {
		rating = rating + 1;
	}
	
	bool FloatingNorthwards() {
		return floatDirection == 1;
	}
	
	bool CloserToNorthShore() {
		return transform.position.z > Frog.MiddleOfTheStreamZ;
	}
	
	void HandleFrogBoundaryHit(GameObject other) {	
		if (other == gameObject) {
			if (FloatingNorthwards() && CloserToNorthShore()) {
				rating++;
				FireRatingChangedNotification();
				ResetState();
				ScoreFromCoinsDelivered();
				floatDirection = -floatDirection;
			} else if (!FloatingNorthwards() && !CloserToNorthShore()) {
				UpgradeFloating();
				floatDirection = -floatDirection;
			} else {
				print ("unknown case" + FloatingNorthwards() + "" + CloserToNorthShore());
			}
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
	
	void UsePowerup(int index) {
		if (powerupQuantities[index] > 0) {
			InventoryPowerups[index].ApplyTo(this);		
			powerupQuantities[index]--;
		}
	}
	
	void DrawInventory() {
		Rect inventoryRect = inventoryRects[playerNumber];
		GUI.Box(inventoryRect, "");
		
		int padding = 5;
		int buttonWidth = (int)inventoryRect.width / Inventory.Length - (Inventory.Length * padding);
		int buttonHeight = (int)inventoryRect.height - (padding * 2);
		
		for (int i = 0; i < InventoryPowerups.Length; i++) {
			string powerupDisplay = InventoryPowerups[i].Name + "x" + powerupQuantities[i];
			if (GUI.Button(new Rect(inventoryRect.x + padding + (buttonWidth * i) + (i * padding), inventoryRect.y + padding, buttonWidth, buttonHeight), powerupDisplay)) {
				UsePowerup(i);
			}
		}
	}
}
