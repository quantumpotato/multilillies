using UnityEngine;
using System.Collections;

public class EnemyBoundary : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		EnemySpawner.Instance.DestroyEnemy(other.gameObject);
	}
}
