using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public static EnemySpawner Instance;
	
	public int desiredEnemyCount;
	public GameObject logPrefab;
	public GameObject fishPrefab;
	public int maximumZ;
	public int logWeight;
	public int fishWeight;
	
	private int playerIndex;
	private int spawnTimer;
	
	private int actualEnemyCount;
	
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
		int score = Frog.TotalScore;
		print("totalscore: " + score);
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
			enemyPrefab = fishPrefab;
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
