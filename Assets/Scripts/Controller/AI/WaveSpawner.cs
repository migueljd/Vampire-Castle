using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour {

	private Wave mainWave;

	public Waypoint firstWaypoint;

	// Use this for initialization
	void Start () {
		//For test purposes, create a list
		GameObject enemyPrefab = (GameObject) Resources.Load ("Prefabs/Enemy");
		Debug.Log (enemyPrefab);
		List<GameObject> enemies = new List<GameObject> ();
		enemies.Add (enemyPrefab);
		enemies.Add (enemyPrefab);
		enemies.Add (enemyPrefab);
		enemies.Add (enemyPrefab);
		enemies.Add (enemyPrefab);

		mainWave = new Wave (enemies, 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (mainWave.GetEnemyCount () > 0) {
			Debug.Log ("Enemy Count: " + mainWave.GetEnemyCount ());
			GameObject nextEnemy = mainWave.TryGetNextEnemy();
			Debug.Log (nextEnemy);
			if(nextEnemy != null)
				SpawnEnemy(nextEnemy);

		}
	}


	public void SpawnEnemy(GameObject enemy){
		Debug.Log ("Spawning enemy");
		GameObject spawnedEnemy = (GameObject) Instantiate (enemy, this.transform.position, Quaternion.identity);
		spawnedEnemy.GetComponent<BaseAI> ().nextWaypoint = firstWaypoint;

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
			nextSpawnTime += Time.time + timer;
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
