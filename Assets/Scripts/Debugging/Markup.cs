﻿using UnityEngine;
using System.Collections;

public class Markup : MonoBehaviour {

	public Color colorMarkup;

	public bool available = true;

	public float radius = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos(){
		Gizmos.color = colorMarkup;
		Gizmos.DrawWireSphere (this.transform.position, radius);
	}
}
