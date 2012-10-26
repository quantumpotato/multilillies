using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public int minSpeed;
	public int maxSpeed;
	
	private float _speed;
	private Vector3 _startPosition;
	
	#region MonoBehaviour
	void Awake() {
		int speedMod = Random.Range(minSpeed,maxSpeed);
		_speed = speedMod * .05f;
		_speed+= 3;
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
