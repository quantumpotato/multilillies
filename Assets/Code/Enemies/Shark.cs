using UnityEngine;
using System.Collections;

public class Shark : ManuallyMovedEnemy {
	public float attackRange;
	
	private bool attacking;
	
	private int direction;
	
	private int stallBeforeBoosting;
	
	private int maxSpeed = 30;
	
	private enum SharkState {
		Swimming,
		Preparing,
		Charging
	}
	
	private SharkState state;
	
	#region monodevelop
	#endregion monodevelop
	
	override public void SetSpeedForLowestAndTeamRatings(int lowest, int total) {
		int defaultSpeedCap = (total /2) + 3;
		int speedCap = defaultSpeedCap > maxSpeed ? maxSpeed : defaultSpeedCap;
		_speed = Random.Range((lowest / 2) + 2, speedCap);
		
		int maxDelay = 60;
		stallBeforeBoosting = maxDelay - (lowest /4);
		if (stallBeforeBoosting < 5) {
			stallBeforeBoosting = 5;
		}
	}
	
	protected override void UpdatePosition() {
		if (!attacking) {
			ScanForFrogs();
		}
		
		float zDelta = attacking ? (direction / 2) * Time.deltaTime : 0;
		
		if (state == SharkState.Preparing) {
			stallBeforeBoosting--;
			if (stallBeforeBoosting <= 0) {
				state = SharkState.Charging;
				_speed *= 2;
			}
		} else {
			transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z + zDelta);
		}
			
	}
	
	void ScanForFrogs() {
		if (!RuneManager.Instance.WellFedSharks) {
			foreach (Frog f in PlayerManager.Instance.Frogs) {
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
							
						//stall state delay
						state = SharkState.Preparing;
					}
				}
			}
		}
	}
}

