using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public static EnemySpawner Instance;
	public static void Play() {
		Instance.gameObject.SetActiveRecursively(true);
	}
	
	public int desiredEnemyCount;
	public GameObject logPrefab;
	public GameObject fishPrefab;
	public GameObject bigFishPrefab;
	public GameObject sharkPrefab;
	public GameObject fatLogPrefab;
	
	public int maximumZ;
	public int logWeight;
	public int fishWeight;
	public int bigFishWeight;
	public int sharkWeight;
	public int fatLogWeight;
	
	private int playerIndex;
	private int spawnTimer;
	
	public int actualEnemyCount;
	private int lowestSpawnDelay;
	private int highestSpawnDelay;
	
	private int score;
	
	private float lastSpawnZ;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		foreach (Frog f in PlayerManager.Instance.Frogs) {
			f.RatingChanged += HandleFrogRatingChanged;
		}
	}
	
	// Update is called once per frame
	void Update () {	
		if (actualEnemyCount < desiredEnemyCount) {
			spawnTimer--;
			if (spawnTimer <= 0) {
				SpawnEnemy();
			}
		}
	}
	#endregion

	void HandleFrogRatingChanged(Frog frog) {
		CalculateEnemyWeights();	
	}
	
	public void DestroyEnemy(GameObject enemy) {
		GameObject.Destroy(enemy);
	}
	
	private void CyclePlayerIndex() {
		//will need to check which players are active and if not increment and call itself
		playerIndex++;
		if (playerIndex > 3) { 
			playerIndex = 0;
		}
	}
	
	float SpawnZ() {
		float distanceIncrement = Random.Range(1, maximumZ / 2);
		lastSpawnZ = lastSpawnZ - distanceIncrement;
		if (lastSpawnZ < 2) {
			lastSpawnZ += maximumZ;
		}
		
		return lastSpawnZ;
	}
		
	int LowestAllowedSpawnDelay() {
		if (HardMode()) {
		   return 1;	
		}
		
		return 5;
	}
	
	void SetLowestSpawnDelay() {
		lowestSpawnDelay = 15 - Frog.TotalRating;
		if (lowestSpawnDelay < LowestAllowedSpawnDelay()) {
			lowestSpawnDelay = LowestAllowedSpawnDelay();
		}
	}
	
	bool EasyMode() {
		return Frog.HighRating < 20 && Frog.MinRating < 11 - (Frog.NumberOfPlayers);
	}
	
	bool HardMode() {
		return Frog.TotalRating > 50;
	}
	
	void ModifySpawnDelayForEasyMode() {
		if (EasyMode()) {
			lowestSpawnDelay+= (150 - ((11 - Frog.NumberOfPlayers - Frog.MinRating) * 5));	
		}
	}
	
	void SetSpawnDelay() {
		SetLowestSpawnDelay();
		SetHighestSpawnDelay();
		ModifySpawnDelayForEasyMode();
	}
	
	void SetHighestSpawnDelay() {
		highestSpawnDelay = 10;
		if (EasyMode()) { 	
			highestSpawnDelay = 30;
		}
	}
	
	void SetDesiredEnemyCount() {
		desiredEnemyCount = Frog.TotalRating - 6;
		if (desiredEnemyCount < 8) {
			desiredEnemyCount = 8;
		}
		if (desiredEnemyCount > 30) {
			desiredEnemyCount = 30;
		}
	}
	
	void SetSpawnTimer() {
		spawnTimer = Random.Range(lowestSpawnDelay,highestSpawnDelay);
	}
	
	GameObject NewEnemy() {
		int weightTotal = logWeight + fishWeight + sharkWeight + bigFishWeight + fatLogWeight;
		int spawnPossibility = Random.Range(0, weightTotal);
		int logRequired = logWeight;
		int sharkRequired = logRequired + sharkWeight;
		int fishRequired = sharkRequired + fishWeight;
		int bigFishRequired = fishRequired + bigFishWeight;
		int fatLogRequired = bigFishRequired + fatLogWeight;	
		
		GameObject enemyPrefab = logPrefab;
		if (spawnPossibility <= logRequired) {
			enemyPrefab = logPrefab;
		} else if (spawnPossibility < sharkRequired) {
			enemyPrefab = sharkPrefab;
		} else if (spawnPossibility < fishRequired) {
			enemyPrefab = fishPrefab;
		} else if (spawnPossibility < bigFishRequired) {
			enemyPrefab = bigFishPrefab;
		} else if (spawnPossibility < fatLogRequired) {
			enemyPrefab = fatLogPrefab;
		} else {
			print ("chose none " + spawnPossibility + " which is over" + fishRequired + "and over" + weightTotal);
		}
		return enemyPrefab;	
	}
	
	void ConfigureEnemyPrefabForSpawningRules(GameObject enemyPrefab) {
		bool shouldSpawnLog = true;
		bool isLog = (enemyPrefab == logPrefab || enemyPrefab == fatLogPrefab);
		if (isLog && RuneManager.Instance.SaveOurTrees) {
			int rand = Random.Range(1,100);
			shouldSpawnLog = (rand > 95 ? false : true);
			print("spawn log: "  + shouldSpawnLog.ToString());
		}
		if ((isLog && shouldSpawnLog) || !isLog) {
			// spawn enemy
			GameObject enemyObject = (GameObject)Instantiate(enemyPrefab);
			Enemy enemy = enemyObject.GetComponent<Enemy>();
			enemy.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + SpawnZ());
			enemy.SetSpeedForLowestAndTeamRatings(Frog.MinRating, Frog.TotalRating);
		}
	}
	
	void SpawnEnemy() {
		CalculateEnemyWeights();
		GameObject enemyPrefab = NewEnemy();	
		ConfigureEnemyPrefabForSpawningRules(enemyPrefab);
		
		CyclePlayerIndex();
		SetSpawnDelay();
		SetDesiredEnemyCount();
		SetSpawnTimer();
	}
	
	void CalculateEnemyWeights() {
		int rating = PlayerManager.Instance.Frogs[playerIndex].rating;
		rating-= 10;
		if (rating < 1) {
			rating = 1;
		}
		int heavyWeight = rating / 2;
		int lightWeight = rating / 3;
		int featherWeight = rating / 10;
		fatLogWeight = rating / 11;
		
		logWeight = 3 + featherWeight;	
		sharkWeight = lightWeight;
		
		fishWeight = heavyWeight;
		bigFishWeight = fishWeight / 2;
	}
}
