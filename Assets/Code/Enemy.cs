using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	private float _speed;
	private Vector3 _startPosition;
	
	#region MonoBehaviour
	void Awake() {
		_speed = Random.Range(1,5);
		Physics.IgnoreLayerCollision(8,8, true);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z);
	}
	#endregion
}
