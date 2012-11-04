using UnityEngine;
using System.Collections;

public class Fish : Enemy {
	
	private Vector3[] path;
	
	protected override void Start () {
		base.Start();
		
		GeneratePath();
		Move();
	}
	
	protected override void Update() {
		base.Update();
		
		transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z);
	}
	
	void GeneratePath() {
		
	}
	
	void Move() {
		//iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", 3));
	}
}
