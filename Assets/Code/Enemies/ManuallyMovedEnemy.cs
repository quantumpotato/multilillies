using UnityEngine;
using System.Collections;

public class ManuallyMovedEnemy : Enemy {
	#region MonoBehaviour
	protected virtual void Update () {
		UpdatePosition();
	}
	#endregion
	
	public override void SetSpeedForLowestAndTeamRatings(int lowest, int total) {
		_speed = Random.Range (SpeedModForRating (lowest), SpeedModForRating (total));	
	}
	
	public override void SetSpeedForFrog(Frog frog) {
		int ratingMod = RatingMod (frog);
		int speedMod = Random.Range (ratingMod, ratingMod * 2);
		_speed = speedMod;
	}
	
	public override void GetCaughtBy(Fisherman fisherman) {
		_speed = 0;
		base.GetCaughtBy(fisherman);
	}
	
	protected virtual void UpdatePosition() {
		// go nowhere by default
	}
}
