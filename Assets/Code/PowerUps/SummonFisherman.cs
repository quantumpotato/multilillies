using UnityEngine;
using System.Collections;

public class SummonFisherman : PowerUp {
	public SummonFisherman():
		base()
	{
		Name = "Fisherman";
	}
	
	public override void ApplyTo(Frog frog) {
		Fisherman.Instance.StartSequence();
	}
}
