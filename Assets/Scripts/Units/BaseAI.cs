﻿using UnityEngine;
using System.Collections;

public class BaseAI : BaseUnit {
	
	public Waypoint nextWaypoint;

	private static float distanceThreshold = 2;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		MoveTo (nextWaypoint.transform.position);
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		if (target == null && nextWaypoint != null) {
			MoveTo (nextWaypoint.transform.position);
		}

		if (nextWaypoint != null && Vector3.Distance (nextWaypoint.transform.position, transform.position) <= distanceThreshold && target == null) {
			nextWaypoint = nextWaypoint.next;
			if (nextWaypoint != null)
				MoveTo (nextWaypoint.transform.position);
		}

	}

//	protected override void OnTriggerEnter(Collider c){
//		base.OnTriggerEnter (c);
//	}

	protected override void OnTriggerStay(Collider c){
		base.OnTriggerStay (c);
	}
}
