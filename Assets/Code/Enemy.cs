using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	private float _speed;
	private Vector3 _startPosition;
	
	#region MonoBehaviour
	void Awake() {
		int speedMod = Random.Range(1,100);
		_speed = speedMod * .05f;
		_speed+= 3;
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
