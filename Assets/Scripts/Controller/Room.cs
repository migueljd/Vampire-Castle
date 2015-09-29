using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

	private List<BaseUnit> units;
	private Transform roomTransform;

	public int roomLimit;
	
	// Use this for initialization
	void Start () {
		units = new List<BaseUnit> ();
		this.roomTransform = roomTransform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool TryAddUnit(BaseUnit unit){
		if (units.Count < roomLimit) {
			units.Add (unit);
			return true;
		}

		return false;
	}

	public void RemoveUnit(BaseUnit unit){
		if(units.Contains(unit))
			units.Remove (unit);
	}
}
