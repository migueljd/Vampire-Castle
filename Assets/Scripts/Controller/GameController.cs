using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GameController : MonoBehaviour {

//	public BaseUnit selectedUnit;
//	public BaseUnit selectedEnemy;

	private BaseUnit toSpawn;
	private BaseUnit rangedSpawn;
	private Tower towerPrefab;

	[Header("Dracula")]
	public BaseUnit dracula;
	public int unitHealthCost;
	public int enemyHealthYield;
	public int draculaMinBlood;


	[Header("Cooldown")]
	public float ECooldown;
	public float RCooldown;
	public float TCooldown;


	[Header("Spawnning")]
	public float chanceForBigGuySpawn;

	public List<WaveSpawner> waveSpawners;
	public static bool draculaAlive = true;

	public int maxDraculaBlood = 60;
//
//	public int availableUnitPool;

	[Header("UI")]
	public Text endGameText;
	public Text draculaHealthText;
	public Text ECooldownText;
	public Text RCooldownText;
	public Text TCooldownText;

	private float lastETime;
	private float lastRTime;
	private float lastTTime;


	private static GameController instance;

	public static float chanceForBigGuySpawn_;


	void Awake(){
		instance = this;
		chanceForBigGuySpawn_ = chanceForBigGuySpawn;
	}

	// Use this for initialization
	void Start () {
		draculaHealthText.text = "Blood: " + dracula.Health; 

		toSpawn = ((GameObject) Resources.Load ("Prefabs/Ally")).GetComponent<BaseUnit>();
		rangedSpawn = ((GameObject) Resources.Load ("Prefabs/RangedAlly")).GetComponent<BaseUnit>();
		towerPrefab = ((GameObject) Resources.Load ("Prefabs/Tower")).GetComponent<Tower> ();

		lastETime = -ECooldown;
		lastRTime = -RCooldown;
		lastTTime = -TCooldown;
	}
	
	// Update is called once per frame
	void Update () {

		CheckEndGame ();

		ECooldownText.text = "E Cooldown: " + (lastETime + ECooldown - Time.time < 0 ? 0 : lastETime + ECooldown - Time.time).ToString("0.00");
		RCooldownText.text = "R Cooldown: " + (lastRTime + RCooldown - Time.time < 0 ? 0 : lastRTime + RCooldown - Time.time).ToString("0.00");
		TCooldownText.text = "T Cooldown: " + (lastTTime + TCooldown - Time.time < 0 ? 0 : lastTTime + RCooldown - Time.time).ToString("0.00");


		//Spawn melee unit
		if (Input.GetKeyDown (KeyCode.E) && lastETime + ECooldown <= Time.time) {
			RaycastHit hit;
			
			Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
			
			
			if(Physics.Raycast(ray,  out hit, 20, (1<< LayerMask.NameToLayer("Controller")))){
				Room roomHit = hit.collider.transform.GetComponent<Room>();

				if(toSpawn != null && dracula.Health > unitHealthCost && (dracula.Health - unitHealthCost) >= draculaMinBlood  && roomHit != null){

					Markup m;

					Vector3 spawnPoint = GetCorrectedDepthPoint(hit, out m) + new Vector3(0, toSpawn.transform.GetComponent<Collider>().bounds.size.y/2, 0);

					BaseUnit go = (BaseUnit) Instantiate(toSpawn, spawnPoint, Quaternion.identity);
					go.markup = m;
					if( roomHit.TryAddUnit(go) && m != null){
						lastETime = Time.time;
						dracula.Health -= unitHealthCost;
						draculaHealthText.text = "Blood: " + dracula.Health;
					}
					else{
						Destroy(go.gameObject);
					}
				}
			}

		}
		//Puting Ranged unit
		if (Input.GetKeyDown (KeyCode.R) && lastRTime + RCooldown <= Time.time) {
			RaycastHit hit;
			
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			
			
			if (Physics.Raycast (ray, out hit, 20, (1 << LayerMask.NameToLayer ("Controller")))) {
				Room roomHit = hit.collider.transform.GetComponent<Room> ();
				
				if (rangedSpawn != null && dracula.Health > unitHealthCost && (dracula.Health - unitHealthCost) >= draculaMinBlood && roomHit != null) {
					
					Markup m;
					
					Vector3 spawnPoint = GetCorrectedDepthPoint (hit, out m) + new Vector3 (0, rangedSpawn.transform.GetComponent<Collider> ().bounds.size.y / 2, 0);
					
					BaseUnit go = (BaseUnit)Instantiate (rangedSpawn, spawnPoint, Quaternion.identity);
					go.markup = m;
					if (roomHit.TryAddUnit (go) && m != null) {
						lastRTime = Time.time;
						dracula.Health -= unitHealthCost;
						draculaHealthText.text = "Blood: " + dracula.Health;
					} else {
						Destroy (go.gameObject);
					}
				}
			}
		}
		//Putting in tower
		if (Input.GetKeyDown (KeyCode.T) && lastTTime + TCooldown <= Time.time) {
			RaycastHit hit;
			
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			
			
			if (Physics.Raycast (ray, out hit, 20, (1 << LayerMask.NameToLayer ("Controller")))) {
				Room roomHit = hit.collider.transform.GetComponent<Room> ();
				
				if (towerPrefab != null && dracula.Health > unitHealthCost && (dracula.Health - unitHealthCost) >= draculaMinBlood && roomHit != null) {
					
					Markup m;
					
					Markup towerMarkup = GetTowerPoint(hit, out m);
					
					Tower tower = (Tower)Instantiate (towerPrefab, towerMarkup.transform.position, Quaternion.identity);
					tower.MinionsMarkup = m;
					tower.TowerMarkup = towerMarkup;

					lastTTime = Time.time;
					dracula.Health -= unitHealthCost;
					draculaHealthText.text = "Blood: " + dracula.Health;
					
				}
			}
		}
	}

	private void CheckEndGame(){

		bool thereAreEnemiesToSpawn = false;

		foreach (WaveSpawner ws in waveSpawners) {
			if(ws.GetEnemyToSpawn() != 0)
				thereAreEnemiesToSpawn = true;
		}

		if (WaveSpawner.enemySpawned == 0 && !thereAreEnemiesToSpawn) {
			endGameText.text = "Dracula is the ultimate overlord!";
			endGameText.enabled = true;
		} else if (!draculaAlive) {
			endGameText.text = "Punny Dracula >.>";
			endGameText.enabled = true;
		}
	}

	private Vector3 GetCorrectedDepthPoint(RaycastHit hit, out Markup m){
		float z = 0;
		float x = 0;
		Debug.Log (hit.collider.name);
		m = null;
		for(int a =0; a < hit.collider.transform.childCount && m == null; a++){
			Transform child = hit.collider.transform.GetChild(a);
			if(child.name == "DepthMarkup"){
				m = child.GetComponent<Markup>();
				if(m.available){
					z = child.position.z;
					x = child.position.x;
					Debug.Log (x);
					m.available = false;
				}
				else{
					m = null;
				}
			}
		}

		return new Vector3(x,hit.point.y ,z);
	}

	private Markup GetTowerPoint(RaycastHit hit, out Markup m){
		Markup main;

		m = null;
		for(int a =0; a < hit.collider.transform.childCount; a++){
			Transform child = hit.collider.transform.GetChild(a);
			if(child.name == "TowerMarkup"){
				main = child.GetComponent<Markup>();
				if(main.available){
					return main;
				}
				else{
					main = null;
				}
			}
			else if(child.name == "DepthMarkup"){
				m = child.GetComponent<Markup>();
				if(m.available){
					m.available = false;
				}
				else{
					m = null;
				}
			}
		}
		return null;
	}

	public static void UpdateDraculaHealthText(){
		instance.draculaHealthText.text = "Blood: " + instance.dracula.Health; 
	}

	public static void GiveBlood(){
		if (instance.dracula.Health + instance.enemyHealthYield > instance.maxDraculaBlood)
			instance.dracula.Health = instance.maxDraculaBlood;
		else
			instance.dracula.Health += instance.enemyHealthYield;
		UpdateDraculaHealthText ();
	}
	
}

