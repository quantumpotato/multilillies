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
	
	public virtual void Apply() {
		
	}
	
	public virtual void Unapply() {
		
	}
}
