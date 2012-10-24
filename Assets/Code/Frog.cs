using UnityEngine;
using System.Collections;

public class Frog : MonoBehaviour {
	
	public float _speed;
	private Vector3 startPosition;
	
	#region MonoBehaviour
	void Awake() {
		startPosition = transform.localPosition;
	}

	// Use this for initialization
	void Start () {
		FrogBoundary.Instance.Hit += HandleFrogBoundaryHit;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + _speed * Time.deltaTime);
	}
	
	void OnCollisionEnter(Collision collision) {
    	ResetPosition();    
    }
	#endregion
	
	
	void ResetPosition() {
		transform.position = startPosition;
	}
	
	void HandleFrogBoundaryHit (GameObject other) {
		if (other == gameObject) {
			ResetPosition();
		}
	}
}
