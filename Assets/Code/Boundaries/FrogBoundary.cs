using UnityEngine;
using System.Collections;

public class FrogBoundary : MonoBehaviour {
	
	public delegate void HitHandler(GameObject other);
	public event HitHandler Hit;
	
	public static FrogBoundary Instance;
	
	void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		if (Hit != null) {
			Hit(other.gameObject);
		}
	}
}
