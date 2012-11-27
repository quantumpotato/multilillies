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
	
	private int actualEnemyCount;
	
	private int score;
	
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
		actualEnemyCount--;
	}
	
	private void CyclePlayerIndex() {
		//will need to check which players are active and if not increment and call itself
		playerIndex++;
		if (playerIndex > 3) { 
			playerIndex = 0;
		}
	}
	
	void SpawnEnemy() {
		CalculateEnemyWeights();
			
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
		
		GameObject enemyObject = (GameObject)Instantiate(enemyPrefab);
		
		Enemy enemy = enemyObject.GetComponent<Enemy>();
		enemy.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(2, maximumZ));
		actualEnemyCount++;
		
		CyclePlayerIndex();
		

		enemy.SetSpeedForLowestAndTeamRatings(Frog.MinRating, Frog.TotalRating);
		int lowestSpawnDelay = 15 - Frog.TotalRating;
		if (lowestSpawnDelay < 5) {
			lowestSpawnDelay = 5;
		}
		spawnTimer = Random.Range(lowestSpawnDelay,15);
	}
	
	void CalculateEnemyWeights() {
		int rating = PlayerManager.Instance.Frogs[playerIndex].rating;
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
