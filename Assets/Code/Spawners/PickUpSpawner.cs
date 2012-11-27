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
	public GameObject summonFishermanPrefab;
	public GameObject raiseDamPrefab;
	public int maximumZ;
	
	private int spawnTimer;
	
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
	}

	void Update() {
		spawnTimer--;
		if (spawnTimer <= 0) {
			SpawnPickup();
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
	
	void SpawnPickup() {
		GameObject pickUpPrefab = DetermineRandomPrefab();
		GameObject pickUpObject = (GameObject)Instantiate(pickUpPrefab);
		pickUpObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(2, maximumZ));
		SetRandomSpawnTime();
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
