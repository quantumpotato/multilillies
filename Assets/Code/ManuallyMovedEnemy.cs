using UnityEngine;
using System.Collections;

public class ManuallyMovedEnemy : Enemy {
	#region MonoBehaviour
	protected virtual void Update () {
		UpdatePosition();
	}
	#endregion
	
	public override void SetSpeedForLowestAndTeamScores(int lowest, int total) {
		_speed = Random.Range (SpeedModForScore (lowest), SpeedModForScore (total));	
	}
	
	public override void SetSpeedForFrog(Frog frog) {
		int scoreMod = ScoreMod (frog);
		int speedMod = Random.Range (scoreMod, scoreMod * 2);
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
