using UnityEngine;
using System.Collections;

public class TweenedEnemy : Enemy {
	public override void SetSpeedForFrog(Frog frog) {
		int ratingMod = RatingMod (frog);
		int speedMod = Random.Range(ratingMod, ratingMod * 2);
		_speed = speedMod * 0.007f;
	}
}
