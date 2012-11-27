using UnityEngine;
using System.Collections;

public class SinEnemy : ManuallyMovedEnemy {
	protected float ySpeed;
	private float yDirection;
	protected int stateDuration;
	protected int stateDurationLeft;
	
	protected enum SinState {
		Oscilatting,
		Resting
	}
	
	private SinState state;
	
	private void resetStateDurationLeft() {
		stateDurationLeft = stateDuration;
	}
	
	protected virtual void SetXSpeed(int lowest, int total) {
		_speed = Random.Range (SpeedModForRating (lowest) / 2 + 2, SpeedModForRating(total) / 2 + 3);	
	}
	
	protected virtual void SetYSpeed(int lowest, int total) {
		ySpeed = Random.Range (1,6);
	}
	
	protected virtual void SetStateDuration(int lowest, int total) {
		stateDuration = Random.Range (20, 40+lowest);
	}
		
	public override void SetSpeedForLowestAndTeamRatings(int lowest, int total) {
	    SetXSpeed(lowest, total);
		yDirection = 1;
		SetYSpeed(lowest, total);
		SetStateDuration(lowest, total);
		
		
		resetStateDurationLeft();
	}
	
	protected override void UpdatePosition ()
	{
		if (state == SinState.Oscilatting) {
			float oscillationPercentage = ((float)stateDuration - (float)stateDurationLeft) / (float)stateDuration;
			oscillationPercentage = Mathf.Abs(oscillationPercentage - 0.5f);
			float sinSpeed = ySpeed - (oscillationPercentage * ySpeed);
			transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z + (sinSpeed * yDirection) * Time.deltaTime);
		} else {
			//stay still	
		}
		stateDurationLeft --;
		
		if (stateDurationLeft <= 0) {
			if (state == SinState.Oscilatting) {
				yDirection = -yDirection;
				stateDurationLeft = stateDuration;
				//state = SinState.Resting;
				resetStateDurationLeft();
			}
		}
	}
}