using UnityEngine;
using System.Collections;

public abstract class Rune : MonoBehaviour {
	#region MonoBehaviour
	protected virtual void Awake() {
		Name = "Unknown Rune";
	}
	#endregion
	
	private string name;
	public string Name {
		get {
			return name;
		}
		set {
			name = value;
		}
	}
	
	private bool applied;
	public bool Applied {
		get {
			return applied;
		}
		set {
			applied = value;
		}
	}
	
	public virtual void Apply() {
		applied = true;
	}
	
	public virtual void Unapply() {
		applied = false;
	}
	
	public void ToggleApplied() {
		if (applied) {
			Unapply();
		} else {
			Apply();
		}
	}
}
