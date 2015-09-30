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
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool TryAddUnit(BaseUnit unit){
		Debug.Log ("Count " + units.Count + ". Limit " + roomLimit);
		if (units.Count < roomLimit) {
			units.Add (unit);
			unit.unitRoom = this;
			return true;
		}

		return false;
	}

	public void RemoveUnit(BaseUnit unit){
		Debug.Log (units[0].Health);
		Debug.Log (unit);
		if (units.Contains (unit)) {
			Debug.Log ("Removed");
			units.Remove (unit);
		}
	}
}
