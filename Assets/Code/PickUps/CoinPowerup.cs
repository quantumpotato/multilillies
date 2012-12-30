using UnityEngine;
using System.Collections;

public class CoinPowerup : PowerUp {
	public CoinPowerup():
		base()
	{
		Name = "Coin";
	}
	
	public override void ApplyTo(Frog frog) {
		frog.CollectCoin();
	}
}
