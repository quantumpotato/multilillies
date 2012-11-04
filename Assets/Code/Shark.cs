using UnityEngine;
using System.Collections;

public class Shark : Enemy {
	
	public float attackRange;
	
	private bool attacking;
	
	private int direction;
	
	#region monodevelop	
	#endregion monodevelop
	
	protected override void UpdatePosition() {
		if (!attacking) {
			ScanForFrogs();
		}
		
		float zDelta = attacking ? (direction / 2) * Time.deltaTime : 0;
		
		transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z + zDelta);
	}
	
	void ScanForFrogs() {
		foreach (Frog f in Frog.Players) {
			if (!attacking) {
				float distance = Vector3.Distance(transform.position, f.transform.position);
				if (distance < attackRange) {
					attacking = true;
					direction = Random.Range(4, 8);

					int Y = Random.Range(0, 4);
					if (Y <= 1) {
						direction = 0;
					}
					if (Y == 2) {
						direction = -direction;
					}
					
					_speed += 6;
				}
			}
		}
	}
}

