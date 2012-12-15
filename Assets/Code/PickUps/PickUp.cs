using UnityEngine;
using System;
using System.Collections;

public class PickUp : MonoBehaviour {
	public float speed;
	public string powerUpClassName;
	
	public static bool IsPickUp(GameObject go) {
		return go.GetComponent<PickUp>() != null;
	}

	#region MonoBehaviour
	void Update() {
		UpdatePosition();
	}
	#endregion
	
	public void ApplyTo(Frog frog) {
		Type type = Type.GetType(powerUpClassName);
		PowerUp powerUp = (PowerUp)Activator.CreateInstance(type);
		frog.AddToInventory(powerUp);
	}
	
	void UpdatePosition() {
		transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
	}
}
