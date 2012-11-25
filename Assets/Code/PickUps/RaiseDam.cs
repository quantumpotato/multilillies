using UnityEngine;
using System.Collections;

public class RaiseDam : PowerUp {
	public override void ApplyTo(Frog frog) {
		Dam.Instance.Raise();
	}
}
