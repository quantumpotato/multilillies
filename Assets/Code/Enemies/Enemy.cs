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
	
	private int TopSpeed = 30;
	
	#region MonoBehaviour
	protected virtual void Awake() {
	}
	
	protected virtual void Start() {
		_core = transform.FindChild("core").gameObject;
		EnemySpawner.Instance.actualEnemyCount++;
	}
	
	void OnDestroy() {
		EnemySpawner.Instance.actualEnemyCount--;
	}
	#endregion
	
	public virtual void SetSpeedForLowestAndTeamRatings(int lowest, int total) {
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
	
	protected int SpeedModForRating(int rating) {
		int speed = Random.Range (9, 12);
		speed += (rating / 4);
		if (speed > TopSpeed) {
			speed = TopSpeed;
		}
		return speed;
	}
	
	protected int RatingMod(Frog frog) {
		return SpeedModForRating(frog.rating);
	}
	
	protected void OnCaught(Fisherman fisherman) {
		fisherman.FinishCatching(this);
	}
}
