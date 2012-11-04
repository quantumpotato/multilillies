using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public int minSpeed;
	public int maxSpeed;
	
	protected float _speed;
	protected Vector3 _startPosition;
	protected float _startTime;
	
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
	
	protected virtual void UpdatePosition ()
	{
		// go nowhere by default
	}
}
