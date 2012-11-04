using UnityEngine;
using System.Collections;

public class Shark : Enemy {
	
	public float attackRange;
	
	private bool attacking;
	
	#region monodevelop
	void Start () {
	
	}
	
	void UpdatePosition {
		
	}
	#endregion monodevelop
	
	void ScanForFrogs() {
		if (!attacking) {
			foreach (Frog f in Frog.Players) {
				float distance = Vector3.Distance(transform.position, f.transform.position);
				if (distance < attackRange) {
					
				}
			}
		}
	}
}

