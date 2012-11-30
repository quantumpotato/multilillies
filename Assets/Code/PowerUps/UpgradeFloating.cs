using UnityEngine;
using System.Collections;

public class UpgradeFloating : PowerUp {

	#region MonoBehaviour
	#endregion
	
	public override void ApplyTo(Frog frog) {
		frog.UpgradeFloating();
	}
}
