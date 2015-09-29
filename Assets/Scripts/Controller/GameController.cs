using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GameController : MonoBehaviour {

	public BaseUnit selectedUnit;
	public BaseUnit selectedEnemy;

	public BaseUnit toSpawn;

	public WaveSpawner waveSpawner;
	public static bool draculaAlive = true;


	public int availableUnitPool;

	public Text endGameText;
	public Text availableUnitPoolText;


	// Use this for initialization
	void Start () {
//		Debug.Log(~(1<< LayerMask.NameToLayer("Controller")));
//		Debug.Log (~(1 <<LayerMask.NameToLayer ("Default")));
		availableUnitPoolText.text = "Available units: " + availableUnitPool;
	}
	
	// Update is called once per frame
	void Update () {

		CheckEndGame ();


		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			
			Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
			
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 10);
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

						Vector3 destination = GetCorrectedDepthPoint(hit);
						selectedUnit.MoveTo(destination);
					}
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.I)) {
			RaycastHit hit;
			
			Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
			
			
			if(Physics.Raycast(ray,  out hit, 20, (1<< LayerMask.NameToLayer("Controller")))){
				Room roomHit = hit.collider.transform.GetComponent<Room>();

				if(toSpawn != null && availableUnitPool > 0 && roomHit != null && roomHit.TryAddUnit(toSpawn)){
					
					Vector3 spawnPoint = GetCorrectedDepthPoint(hit) + new Vector3(0, toSpawn.transform.GetComponent<Collider>().bounds.size.y/2, 0);
					Instantiate(toSpawn, spawnPoint, Quaternion.identity);
					availableUnitPool--;
					availableUnitPoolText.text = "Available units: " + availableUnitPool;
				}
			}
		}
	}

	private void CheckEndGame(){
//		Debug.Log (string.Format("There are {0} spawned and {1} to be spawned", WaveSpawner.enemySpawned, waveSpawner.GetEnemyToSpawn()));
		if (WaveSpawner.enemySpawned == 0 && waveSpawner.GetEnemyToSpawn () == 0) {
			endGameText.text = "Dracula is the ultimate overlord!";
			endGameText.enabled = true;
		} else if (!draculaAlive) {
			endGameText.text = "Punny Dracula >.>";
			endGameText.enabled = true;
		}
	}

	private Vector3 GetCorrectedDepthPoint(RaycastHit hit){
		float z = 0;
	
		for(int a =0; a < hit.collider.transform.childCount; a++){
			Transform child = hit.collider.transform.GetChild(a);
			if(child.name == "DepthMarkup") z = child.position.z;
		}

		return new Vector3(hit.point.x,hit.point.y ,z);
	}


	
}

