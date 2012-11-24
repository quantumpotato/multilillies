using UnityEngine;
using System.Collections;

public class SinEnemy : ManuallyMovedEnemy {
	private float ySpeed;
	private float yDirection;
	private int stateDuration;
	private int stateDurationLeft;
	
	private enum SinState {
		Oscilatting,
		Resting
	}
	
	private SinState state;
	
	private void resetStateDurationLeft() {
		stateDurationLeft = stateDuration;
	}
	
	public override void SetSpeedForLowestAndTeamScores(int lowest, int total) {
		_speed = Random.Range (SpeedModForScore (lowest) / 2 + 2, SpeedModForScore(total) / 2 + 3);	
		yDirection = 1;
		ySpeed = Random.Range (1,6);
		stateDuration = Random.Range (20, 40+lowest);
		
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