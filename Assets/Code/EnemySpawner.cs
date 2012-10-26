using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public int desiredEnemyCount;
	public GameObject enemyPrefab;
	public int maximumZ;
	
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
			SpawnEnemy();
		}
	}
	
	public void DestroyEnemy(GameObject enemy) {
		GameObject.Destroy(enemy);
		actualEnemyCount--;
	}
	
	void SpawnEnemy() {
		GameObject enemy = (GameObject)Instantiate(enemyPrefab);
		int xMod = Random.Range(10,100);
		enemy.transform.position = new Vector3(transform.position.x - xMod, transform.position.y, transform.position.z + Random.Range(1, maximumZ));
		actualEnemyCount++;
	}
}
