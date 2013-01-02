using UnityEngine;
using System.Collections;

public class FrogBoundary : MonoBehaviour {
	public delegate void HitHandler(GameObject other);
	public event HitHandler Hit;
	
	public static FrogBoundary NorthInstance;
	public static FrogBoundary SouthInstance;
	
	#region MonoBehaviour
	void Awake () {
		if (transform.position.z > Frog.MiddleOfTheStreamZ) {
			NorthInstance = this;
		} else {
			SouthInstance = this;
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (Hit != null) {
			Hit(other.gameObject);
		}
	}
	#endregion
}
