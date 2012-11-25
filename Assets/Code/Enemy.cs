using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public static bool IsEnemy(GameObject go) {
		return go.GetComponent<Enemy>() != null;
	}
	
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
	
	public virtual void SetSpeedForLowestAndTeamScores(int lowest, int total) {
		// subclass implements
	}
	
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
	
	protected int SpeedModForScore(int score) {
		int speed = Random.Range (9, 12);
		speed += (score / 3);
		if (speed > 50) {
			speed = 50;
		}
		return speed;
	}
	
	protected int ScoreMod(Frog frog) {
		return SpeedModForScore(frog.score);
	}
	
	protected void OnCaught(Fisherman fisherman) {
		fisherman.FinishCatching(this);
	}
}
