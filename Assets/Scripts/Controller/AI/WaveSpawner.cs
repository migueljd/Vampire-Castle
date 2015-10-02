using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour {

	private Wave mainWave;
	private List<Wave> waveList;

	public int numberOfWaves;
	public int enemiesInWave;
	public float timeAtBegin;
	public float timeBetweenUnitSpawn;
	public float timeBetweenWaves;


	public Waypoint firstWaypoint;

	public string XmlFileName;

	public static int enemySpawned;


	private float firstSpawn;
	private Wave waveClone;

	// Use this for initialization
	void Start () {
		//For test purposes, create a list
		if (XmlFileName != null && XmlFileName != "") {
			WaveCreator wc = new WaveCreator ();
			waveList = wc.DeserializeXml ("Assets/Resources/" + XmlFileName + ".xml");
		}

		if (waveList != null && waveList.Count > 0) {
//			GameObject enemyPrefab = (GameObject)Resources.Load ("Prefabs/Enemy");
//			GameObject bigEnemy = (GameObject)Resources.Load ("Prefabs/SlowEnemy");
//			List<GameObject> enemies = new List<GameObject> ();
//
//			for (int a = 0; a < enemiesInWave; a++) { 
//				float random = Random.Range(0.0f, 1.0f);
//				if(random >= 1 - GameController.chanceForBigGuySpawn_)
//					enemies.Add(bigEnemy);
//				else
//					enemies.Add (enemyPrefab);
//			}


			mainWave = waveList[0];
			waveList.RemoveAt(0);
			firstSpawn = Time.time + timeAtBegin;

			numberOfWaves--;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (firstSpawn <= Time.time ) {

			if (mainWave != null && mainWave.GetEnemyCount () > 0) {
				GameObject nextEnemy = mainWave.TryGetNextEnemy ();
				if (nextEnemy != null)
					SpawnEnemy (nextEnemy);

			}
			else if(waveList != null && waveList.Count > 0){

//				List<GameObject> enemies = new List<GameObject> ();
//				GameObject enemyPrefab = (GameObject)Resources.Load ("Prefabs/Enemy");
//				GameObject bigEnemy = (GameObject)Resources.Load ("Prefabs/SlowEnemy");
//
//				
//				for (int a = 0; a < enemiesInWave; a++) { 
//					float random = Random.Range(0.0f, 1.0f);
//					if(random >= 1 - GameController.chanceForBigGuySpawn_)
//						enemies.Add(bigEnemy);
//					else
//						enemies.Add (enemyPrefab);
//				}

				mainWave = waveList[0];
				waveList.RemoveAt(0);

				numberOfWaves--;

				firstSpawn = Time.time + timeBetweenWaves;
			}
		}
	}


	public void SpawnEnemy(GameObject enemy){
		GameObject spawnedEnemy = (GameObject) Instantiate (enemy, this.transform.position, Quaternion.identity);
		spawnedEnemy.GetComponent<BaseAI> ().nextWaypoint = firstWaypoint;
		enemySpawned++;

	}

	public int GetEnemyToSpawn(){


		if (mainWave != null)
			return mainWave.GetEnemyCount ();
		else
			return 0;
	}
}


public class Wave{

	private List<GameObject> enemies;
	private float timer;
	private float nextSpawnTime;

	public Wave(List<GameObject> enemiesList, float timer){
		this.enemies = enemiesList;
		this.timer = timer;
		nextSpawnTime = 0;
	}

	public GameObject TryGetNextEnemy(){
		if (Time.time >= nextSpawnTime && enemies.Count > 0) {
			nextSpawnTime = Time.time + timer;
			GameObject next = enemies[0];
			enemies.RemoveAt(0);

			return next;
		}
		return null;
	}

	public int GetEnemyCount(){
		return enemies.Count;
	}
}
