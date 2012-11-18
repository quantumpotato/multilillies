using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	protected GameObject _core;
	
	protected float _speed;
	public float Speed {
		get {
			return _speed;
		}
		set {
			_speed = value;
		}
	}
	
	#region MonoBehaviour
	protected virtual void Awake() {
	}
	
	protected virtual void Start() {
		_core = transform.FindChild("core").gameObject;
	}
	#endregion
	
	public virtual void SetSpeedForFrog(Frog frog) {
		// up to sub-class
	}
	
	public virtual void GetCaughtBy(Fisherman fisherman) {
		float caughtHeight = 8.0f;
		float caughtTime = 2.0f;
		
		iTween.MoveBy(_core, iTween.Hash(
			"y", caughtHeight,
			"time", caughtTime / 2,
			"easeType", iTween.EaseType.easeOutQuad
		));
		iTween.MoveBy(_core, iTween.Hash(
			"y", -caughtHeight,
			"time", caughtTime / 2,
			"delay", caughtTime / 2,
			"easeType", iTween.EaseType.easeInCubic
		));     
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", fisherman.transform,
			"time", caughtTime,
			"easeType", iTween.EaseType.linear,
			"oncomplete", "OnCaught",
			"oncompleteparams", fisherman
		));
	}
	
	public void Die() {
		EnemySpawner.Instance.DestroyEnemy(gameObject);
	}
	
	protected int ScoreMod(Frog frog) {
		int scoreMod = frog.score + 10;
		if (scoreMod > 50) {
			scoreMod = 50;
		}
		return scoreMod;
	}
	
	protected void OnCaught(Fisherman fisherman) {
		fisherman.FinishCatching(this);
	}
}
