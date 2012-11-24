using UnityEngine;
using System.Collections;

public class Fish : TweenedEnemy {
	public int pointCountMin;
	public int pointCountMax;
	public int pathLength;
	public float pointDeviation;
	
	private Vector3[] path;
	private Vector3 rootPosition;
	
	private int pointCount;
	private int PointCount {
		get {
			if (pointCount == 0) {
				pointCount = Random.Range (pointCountMin, pointCountMax);
			}
			return pointCount;
		}
	}
	
	protected override void Start () {
		base.Start();
		
		rootPosition = transform.position;
		
		GeneratePath();
		Move();
	}
	
	void GeneratePath() {
		path = new Vector3[PointCount + 2];
		float pointGap = pathLength / PointCount;
		
		path[0] = rootPosition;
		path[PointCount + 1] = new Vector3(rootPosition.x + (pathLength + pointGap), rootPosition.y, rootPosition.z);
		for (int i = 1; i < PointCount + 1; i++) {
			pointDeviation = -pointDeviation;
			float randomZ = rootPosition.z + pointDeviation;
			float newX = rootPosition.x + (pointGap*i);
			path[i] = new Vector3(newX, rootPosition.y, randomZ);
		}
	}
	
	void Move() {
		iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", 1/Speed, "easetype", iTween.EaseType.linear));
	}
}
