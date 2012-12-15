using UnityEngine;
using System.Collections;

public class RaiseDam : PowerUp {
	public RaiseDam():
		base()
	{
		Name = "Dam";
	}
	
	public override void ApplyTo(Frog frog) {
		Dam.Instance.Raise();
	}
}
