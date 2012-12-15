using UnityEngine;
using System.Collections;

public abstract class PowerUp {
	protected string name;
	public string Name {
		get {
			return name;
		}
		
		protected set {
			name = value;
		}
	}
	
	public PowerUp() {
		Name = "Unknown";
	}
	
	public virtual void ApplyTo(Frog frog) {
		// sub-class implements
	}
}
