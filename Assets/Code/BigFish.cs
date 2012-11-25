using UnityEngine;
using System.Collections;

public class BigFish : SinFish {	
	protected override void SetYSpeed (int lowest, int total) {
		ySpeed = Random.Range (5,10);
	}
	
	protected override void SetStateDuration(int lowest, int total) {
		stateDuration = Random.Range (80+lowest, 200+lowest);
	}
		
}
