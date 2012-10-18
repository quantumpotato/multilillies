using UnityEngine;
using System.Collections;

public class Frog : MonoBehaviour {
	
	public float _speed;
	
	void Awake() {
		
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + _speed * Time.deltaTime);
	}
}
