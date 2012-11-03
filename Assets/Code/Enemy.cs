using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public int minSpeed;
	public int maxSpeed;
	
	private float _speed;
	private Vector3 _startPosition;
	
	public float Speed {
		get {
			return _speed;
		}
		set {
			_speed = value;
		}
	}
	
	#region MonoBehaviour
	void Awake() {
		int speedMod = Random.Range(minSpeed,maxSpeed);
		_speed = speedMod * .05f;
		_speed+= 3;
	}

	// Use this for initialization
	void Start () {
		
	}
	
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
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z);
	}
	#endregion
}
