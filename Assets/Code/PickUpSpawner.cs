using UnityEngine;
using System.Collections;

public class PickUpSpawner : MonoBehaviour {
	public static PickUpSpawner Instance;
	public static void Play() {
		Instance.gameObject.SetActiveRecursively(true);
	}
	
	public int lowestSpawnTickAmount;
	public int highestSpawnTickAmount;
	public GameObject upgradeFloatingPrefab;
	public int maximumZ;
	
	private int spawnTimer;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
	}
	
	// Use this for initialization
	void Start () {
		foreach (Frog f in PlayerManager.Instance.Frogs) {
			f.ScoreChanged += HandleFrogScoreChanged;
		}
		
		SetRandomSpawnTime();
	}

	void Update() {
		spawnTimer--;
		if (spawnTimer <= 0) {
			SpawnPickup();
		}
	}
	#endregion
	
	void HandleFrogScoreChanged(Frog frog) {
		// TODO
	}
	
	void SpawnPickup() {
		GameObject pickUpPrefab = upgradeFloatingPrefab;
		GameObject pickUpObject = (GameObject)Instantiate(pickUpPrefab);
		pickUpObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(2, maximumZ));
		SetRandomSpawnTime();
	}
	
	void SetRandomSpawnTime() {
		spawnTimer = Random.Range(lowestSpawnTickAmount, highestSpawnTickAmount);
	}
}