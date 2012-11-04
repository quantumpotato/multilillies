using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public static EnemySpawner Instance;
	
	public int desiredEnemyCount;
	public GameObject logPrefab;
	public int maximumZ;
	public int logWeight;
	public int fishWeight;
	
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
		int newScore = Frog.TotalScore;
		int fives = newScore / 5;
		int fifteens = newScore / 15;
		logWeight = 5;
		fishWeight = fives;
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
		int weightTotal = logWeight + fishWeight;
		int spawnPossibility = Random.Range(0, weightTotal);
		int logRequired = logWeight;
		int fishRequired = logWeight + fishWeight;
		
		GameObject enemyPrefab = logPrefab;
		if (spawnPossibility < logRequired) {
			enemyPrefab = logPrefab;
			print("chose log " + spawnPossibility + " which is under " + logRequired);
		} else if (spawnPossibility < fishRequired) {
			print("chose fish " + spawnPossibility + " which is under " + fishRequired);
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
		spawnTimer = Random.Range(5,70);
	}
}
