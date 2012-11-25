using UnityEngine;
using System;
using System.Collections;

public class PickUp : MonoBehaviour {
	public float speed;
	public string powerUpClassName;

	#region MonoBehaviour
	void Update() {
		UpdatePosition();
	}
	#endregion
	
	public void ApplyTo(Frog frog) {
		Type type = Type.GetType(powerUpClassName);
		PowerUp powerUp = (PowerUp)Activator.CreateInstance(type);
		powerUp.ApplyTo(frog);
	}
	
	public void Die() {
		Destroy(gameObject);
	}
	
	void UpdatePosition() {
		transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
	}
}
