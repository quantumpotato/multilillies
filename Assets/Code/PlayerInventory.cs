using UnityEngine;
using System.Collections;

public class PlayerInventory : MonoBehaviour {
	public PickUp[] pickUps;
	
	#region MonoBehaviour
	void Awake() {
		pickUps = new PickUp[3];
	}
	
	void OnGUI() {
		
	}
	#endregion
	
	
}
