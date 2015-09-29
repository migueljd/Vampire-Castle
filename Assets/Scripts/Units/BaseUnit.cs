﻿	using UnityEngine;
using System.Collections;

public class BaseUnit : MonoBehaviour {
	

	private NavMeshAgent agent;
	private float nextAttackTime;

	public bool enemy;
	public float attackSpeed;

	public int Health;
	public int Damage;


	protected BaseUnit target;

	public Room unitRoom;


	// Use this for initialization
	protected virtual void Start () {
		agent = transform.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (Health <= 0) {
			if(this is BaseAI)
				WaveSpawner.enemySpawned--;
			else if(this.name == "Dracula")
				GameController.draculaAlive = false;
			else if(this is BaseUnit){
				Debug.Log ("Removing");
				unitRoom.RemoveUnit(this);
			}
			Destroy(this.gameObject);
			return;
		}
		if (target != null && nextAttackTime <= Time.time) {
			nextAttackTime = Time.time + attackSpeed;
			Attack ();

		}

	}


	public void MoveTo(Vector3 position){
		agent.SetDestination (position);
		agent.Resume ();
	}

	protected virtual void OnTriggerEnter(Collider other){
		if (target == null) {
			BaseUnit unit = other.GetComponent<BaseUnit> ();
			if (unit != null && unit.enemy != this.enemy && (unit.target == null || unit.target == this)) {

				EnterCombat (unit);
				agent.Stop ();
			}
		}
	}

	protected virtual void OnTriggerStay(Collider other){
		if (target == null) {
			BaseUnit unit = other.GetComponent<BaseUnit> ();

			if (unit != null && unit.enemy != this.enemy && unit.target == null) {
				if(unit.enemy == false) Debug.Log(unit.target);
				EnterCombat (unit);
				agent.Stop ();
			}
		}
	}


	private void EnterCombat(BaseUnit unit){
		target = unit;

	}

	private void Attack(){
		if (target.TakeDamage (this.Damage)) {
			target = null;
		}

	}

	public bool TakeDamage(int damage){

//		Debug.Log ("Unit " + this.name + " took " + damage + " damage");
		Health -= damage;
//		Debug.Log (string.Format ("Unit {0} remaining health is {1}", this.name, this.Health));
		if (Health <= 0)
			return true;
		return false;
	}
}
