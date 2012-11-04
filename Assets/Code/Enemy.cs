using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public int minSpeed;
	public int maxSpeed;
	
	protected float _speed;
	protected Vector3 _startPosition;
	protected float _startTime;
	protected GameObject _core;
	
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
		int speedMod = Random.Range(minSpeed,maxSpeed);
		_speed = speedMod * .05f;
		_speed+= 3;
		_startTime = Time.time;
	}
	
	protected virtual void Start() {
		_core = transform.FindChild("core").gameObject;
	}
	
	protected virtual void Update () {
		UpdatePosition ();
	}
	#endregion
	
	public void SetSpeedForFrog(Frog frog) {
		int scoreMod = frog.score + 10;
		if (scoreMod > 50) {
			scoreMod = 50;
		}
		
//		print ("scoreMod:" + scoreMod);
		int speedMod = Random.Range (scoreMod / 2, scoreMod);
		_speed = speedMod / 2;
		_speed+= 4;
		//print ("spawned with speed: " + _speed + "   with speedMod: " + speedMod);
	}
	
	public void GetCaughtBy(Fisherman fisherman) {
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
			"oncomplete", "Die"
		));
	}
	
	void Die() {
		Destroy(gameObject);
	}
	
	protected virtual void UpdatePosition ()
	{
		// go nowhere by default
	}
}
