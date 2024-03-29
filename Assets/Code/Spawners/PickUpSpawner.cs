using UnityEngine;
using System.Collections;

public class PickUpSpawner : MonoBehaviour {
	public static PickUpSpawner Instance;
	public static void Play() {
		Instance.gameObject.SetActiveRecursively(true);
	}
	public static void Stop() {
		Instance.gameObject.SetActiveRecursively(false);
		PickUp[] pickUps = (PickUp[])FindObjectsOfType(typeof(PickUp));
		for (int i = 0; i < pickUps.Length; i++) {
			Destroy(pickUps[i].gameObject);
		}
	}
	
	public int lowestSpawnTickAmount;
	public int highestSpawnTickAmount;
	public GameObject upgradeFloatingPrefab;
	public GameObject summonFishermanPrefab;
	public GameObject raiseDamPrefab;
	public GameObject coinPrefab;
	public int maximumZ;
	public int lowestCoinSpawnTickAmount;
	public int highestCoinSpawnTickAmount;
	
	private int spawnTimer;
	private int coinSpawnTimer;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
	}
	
	// Use this for initialization
	void Start () {
		foreach (Frog f in PlayerManager.Instance.Frogs) {
			f.RatingChanged += HandleFrogRatingChanged;
		}
		
		SetRandomSpawnTime();
		SetRandomCoinSpawnTime();
	}

	void Update() {
		spawnTimer--;
		if (spawnTimer <= 0) {
			SpawnPickup();
		}
		
		coinSpawnTimer--;
		if (coinSpawnTimer <= 0) {
			SpawnCoin();
		}
	}
	#endregion
	
	public void DestroyPickUp(GameObject go) {
		PickUp pickUp = go.GetComponent<PickUp>();
		if (pickUp != null) {
			DestroyPickUp(pickUp);
		}
	}
	
	public void DestroyPickUp(PickUp pickUp) {
		GameObject.Destroy(pickUp.gameObject);
	}
	
	void HandleFrogRatingChanged(Frog frog) {
		// TODO
	}
	
	Vector3 PickupSpawnPosition() {
		return new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(2, maximumZ));
	}
	
	void SpawnCoin() {
		GameObject coin = (GameObject)Instantiate(coinPrefab);
		coin.transform.position = PickupSpawnPosition();
		SetRandomCoinSpawnTime();
	}
	
	void SpawnPickup() {
		GameObject pickUpPrefab = DetermineRandomPrefab();
		GameObject pickUpObject = (GameObject)Instantiate(pickUpPrefab);
		pickUpObject.transform.position = PickupSpawnPosition();
		SetRandomSpawnTime();
	}
	
	void SetRandomCoinSpawnTime() {
		coinSpawnTimer = Random.Range (lowestCoinSpawnTickAmount, highestCoinSpawnTickAmount);
	}
	
	void SetRandomSpawnTime() {
		spawnTimer = Random.Range(lowestSpawnTickAmount, highestSpawnTickAmount);
	}
	
	GameObject DetermineRandomPrefab() {
		int num = Random.Range (1, 100);
		if (num <= 15) {
			return summonFishermanPrefab;
		} else if (num > 15 && num <= 40) {
			return raiseDamPrefab;
		}
		return upgradeFloatingPrefab;
	}
}
