using UnityEngine;
using System.Collections;

public class TweenedEnemy : Enemy {
	public override void SetSpeedForFrog(Frog frog) {
		int scoreMod = ScoreMod (frog);
		int speedMod = Random.Range(scoreMod, scoreMod * 2);
		_speed = speedMod * 0.007f;
	}
}
