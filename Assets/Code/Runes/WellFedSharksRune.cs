using UnityEngine;
using System.Collections;

public class WellFedSharksRune : Rune {
	#region MonoBehaviour
	protected override void Awake() {
		base.Awake();
		Name = "Well Fed Sharks";
	}
	#endregion
	
	public override void Apply() {
		Shark.WellFedSharks = true;
	}
	
	public override void Unapply() {
		Shark.WellFedSharks = false;
	}
}
