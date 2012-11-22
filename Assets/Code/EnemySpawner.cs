using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public static EnemySpawner Instance;
	
	public int desiredEnemyCount;
	public GameObject logPrefab;
	public GameObject fishPrefab;
	public GameObject sharkPrefab;
	
	public int maximumZ;
	public int logWeight;
	public int fishWeight;
	public int sharkWeight;
	
	private int playerIndex;
	private int spawnTimer;
	
	private int actualEnemyCount;
	
	private int score;
	
	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		foreach (Frog f in Frog.Players) {
			f.ScoreChanged += HandleFrogScoreChanged;
		}
	}

	void HandleFrogScoreChanged(Frog frog) {
		CalculateEnemyWeights();	
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
	
	public void DestroyEnemy(GameObject enemy) {
		GameObject.Destroy(enemy);
		actualEnemyCount--;
	}
	
	void SpawnEnemy() {
		CalculateEnemyWeights();
			
		int weightTotal = logWeight + fishWeight + sharkWeight;
		int spawnPossibility = Random.Range(0, weightTotal);
		int logRequired = logWeight;
		int sharkRequired = logRequired + sharkWeight;
		int fishRequired = sharkRequired + fishWeight;
		
		GameObject enemyPrefab = logPrefab;
		if (spawnPossibility <= logRequired) {
			enemyPrefab = logPrefab;
			print("chose log " + spawnPossibility + " which is under " + logRequired);
		} else if (spawnPossibility < sharkRequired) {
			enemyPrefab = sharkPrefab;
			print("chose shark " + spawnPossibility + " which is under " + sharkRequired);
		} else if (spawnPossibility < fishRequired) {
			enemyPrefab = fishPrefab;
			print("chose fish " + spawnPossibility + " which is under " + fishRequired);
		} else {
			print ("chose none " + spawnPossibility + " which is over" + fishRequired + "and over" + weightTotal);
		}
		
		GameObject enemyObject = (GameObject)Instantiate(enemyPrefab);
		
		Enemy enemy = enemyObject.GetComponent<Enemy>();
		enemy.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(2, maximumZ));
		actualEnemyCount++;
		
		playerIndex++;
		if (playerIndex > 3) {
			playerIndex = 0;
		}
		
		Frog frog = Frog.Players[playerIndex];
		enemy.SetSpeedForFrog(frog);		
		spawnTimer = Random.Range(5,20);
	}
	
	void CalculateEnemyWeights() {
		int newScore = Frog.TotalScore;
		int fives = newScore / 5;
		int tens = newScore / 10;
		
		logWeight = 5 + fives;
		sharkWeight = fives;
		fishWeight = tens;
	}
}
