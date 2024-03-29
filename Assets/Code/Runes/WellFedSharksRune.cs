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
		base.Apply();
		RuneManager.Instance.WellFedSharks = true;
	}
	
	public override void Unapply() {
		base.Unapply();
		RuneManager.Instance.WellFedSharks = false;
	}
}
