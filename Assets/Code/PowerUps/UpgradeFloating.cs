using UnityEngine;
using System.Collections;

public class UpgradeFloating : PowerUp {
	public UpgradeFloating():
		base()
	{
		Name = "Upgr. Float.";
	}
	
	public override void ApplyTo(Frog frog) {
		frog.UpgradeFloating();
	}
}
