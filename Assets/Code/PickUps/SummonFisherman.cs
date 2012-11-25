using UnityEngine;
using System.Collections;

public class SummonFisherman : PowerUp {
	public override void ApplyTo(Frog frog) {
		Fisherman.Instance.StartSequence();
	}
}
