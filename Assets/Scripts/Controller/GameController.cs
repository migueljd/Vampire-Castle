﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public BaseUnit selectedUnit;
	public BaseUnit selectedEnemy;

	// Use this for initialization
	void Start () {
//		Debug.Log(~(1<< LayerMask.NameToLayer("Controller")));
//		Debug.Log (~(1 <<LayerMask.NameToLayer ("Default")));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			
			Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray,  out hit, 20, ~ (1 << LayerMask.NameToLayer("Default")))){

				BaseUnit unit = hit.collider.transform.GetComponent<BaseUnit>();
				if(unit != null){
					if(unit.enemy)
						selectedEnemy = unit;
					else
						selectedUnit = unit;
				}
					
			}
		} else if (Input.GetMouseButtonDown (1)) {


			if(selectedUnit != null){
				RaycastHit hit;

				Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);


				if(Physics.Raycast(ray,  out hit, 20, (1<< LayerMask.NameToLayer("Controller")))){
					if(selectedUnit != null){

						float z = 0;
						for(int a =0; a < hit.collider.transform.childCount; a++){
							Transform child = hit.collider.transform.GetChild(a);
							if(child.name == "DepthMarkup") z = child.position.z;
						}
						Debug.Log(hit.collider.transform);
						Vector3 destination = new Vector3(hit.point.x,hit.point.y ,z);
						selectedUnit.MoveTo(destination);
					}
				}
			}
		}
	}
	
}
