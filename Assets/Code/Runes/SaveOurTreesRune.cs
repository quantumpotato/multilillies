using UnityEngine;
using System.Collections;

public class SaveOurTreesRune : Rune {
	#region MonoBehaviour
	protected override void Awake() {
		base.Awake();
		Name = "Save Our Trees";
	}
	#endregion
	
	public override void Apply() {
		base.Apply();
		RuneManager.Instance.SaveOurTrees = true;
	}
	
	public override void Unapply() {
		base.Unapply();
		RuneManager.Instance.SaveOurTrees = false;
	}
}
