using UnityEngine;
using System.Collections;

public class Log : Enemy {
	protected override void UpdatePosition ()
	{
		transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z);
	}
}