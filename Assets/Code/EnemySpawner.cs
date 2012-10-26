using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public int desiredEnemyCount;
	public GameObject enemyPrefab;
	public int maximumZ;
	private int playerIndex;
	private int spawnTimer;
	
	private int actualEnemyCount;
	
	public static EnemySpawner Instance;
	
	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
				
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
		GameObject enemyObject = (GameObject)Instantiate(enemyPrefab);
		Enemy enemy = enemyObject.GetComponent<Enemy>();
		enemy.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(4, maximumZ));
		actualEnemyCount++;
		
		playerIndex++;
		if (playerIndex > 3) {
			playerIndex = 0;
		}
		
		Frog frog = Frog.Players[playerIndex];
		enemy.SetSpeedForFrog(frog);
		
		spawnTimer = Random.Range(20,70);
	}
}
