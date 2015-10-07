using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseUnit : MonoBehaviour {
	

	private NavMeshAgent agent;
	private float nextAttackTime;

	public bool enemy;
	public float attackSpeed;

	public int Health;
	public int Damage;


	public BaseUnit target;

	public Room unitRoom;

	public Markup markup;

	[HideInInspector]
	public List<BaseUnit> beingTargetedList;



	// Use this for initialization
	protected virtual void Start () {
		agent = transform.GetComponent<NavMeshAgent> ();
		beingTargetedList = new List<BaseUnit> ();
	}
	
	// Update is called once per frame
	protected virtual void Update () {

		if (target != null && nextAttackTime <= Time.time) {
			nextAttackTime = Time.time + attackSpeed;
			Attack ();

		}

	}

	protected virtual void LateUpdate(){
		if (Health <= 0) {
			this.BeforeDestroy();
			Destroy(this.gameObject);
			return;
		}
	}


	public void MoveTo(Vector3 position){
		agent.SetDestination (position);
		agent.Resume ();
	}

	protected virtual void OnTriggerEnter(Collider other){
//		float dist = Vector3.Distance (this.transform.position, other.transform.position);
//		Collider thisCol = this.GetComponent<Collider> ();
//		Debug.Log ("Dist: " + dist);
//		if(thisCol is SphereCollider) Debug.Log ("Radius: " + ((SphereCollider) thisCol).radius);
//		if (target == null && 
//		    ((thisCol is BoxCollider && dist <= this.GetComponent<Collider>().bounds.size.x/2) || (thisCol is SphereCollider && dist <= ((SphereCollider) thisCol).radius))
//		    ) {
		BaseUnit unit = other.transform.parent != null? other.transform.parent.GetComponent<BaseUnit> () : other.GetComponent<BaseUnit>();
		if (target == null) {
			if (other.transform.parent == null)
				return;
			
			if (this.enemy) {
				if (unit != null && unit.enemy != this.enemy && (unit.target == null || unit.target == this || unit.name == "Dracula")) {
					EnterCombat (unit);
				}
			} else if (unit != null && unit.enemy != this.enemy) {
				EnterCombat (unit);
			}
		} 
		else if (!this.enemy && target.gameObject != other.gameObject && unit != null && target.beingTargetedList.Count > 1 && unit.beingTargetedList.Count == 0) {
			EnterCombat(unit);
		}
		else if (this.enemy && target.target != this) {
			target = null;
			agent.Resume();
		}
	}

	protected virtual void OnTriggerStay(Collider other){
//		float dist = Vector3.Distance (this.transform.position, other.transform.position);
//		Collider thisCol = this.GetComponent<Collider> ();
//		Debug.Log ("Dist: " + dist);
//		if(thisCol is SphereCollider) Debug.Log ("Radius: " + ((SphereCollider) thisCol).radius);
//		if (target == null && 
//		    ((thisCol is BoxCollider && dist <= this.GetComponent<Collider>().bounds.size.x/2) || (thisCol is SphereCollider && dist <= ((SphereCollider) thisCol).radius))
//		    ) {
		BaseUnit unit = other.transform.parent != null? other.transform.parent.GetComponent<BaseUnit> () : other.GetComponent<BaseUnit>();
		if (target == null) {
			if (other.transform.parent == null)
				return;

			if (this.enemy) {
				if (unit != null && unit.enemy != this.enemy && (unit.target == null || unit.target == this || unit.name == "Dracula")) {
					EnterCombat (unit);
				}
			} else if (unit != null && unit.enemy != this.enemy) {
				EnterCombat (unit);
			}
		} else if (!this.enemy && target.gameObject != other.gameObject && unit != null && target.beingTargetedList.Count > 1 && unit.beingTargetedList.Count == 0) {
			EnterCombat (unit);
		} else if (this.enemy && target.target != this) {
			target = null;
			agent.Resume();
		}
	}


	private void EnterCombat(BaseUnit unit){
		Debug.Log ("Combat started");
		if(this.enemy)
			agent.Stop ();

		List<BaseUnit> list;
		if (target != null) {
			list = target.beingTargetedList;
//			lock(list){
				list.Remove(this);
//			}
		}

		target = unit;


		list = unit.beingTargetedList;

//		lock(list){
			list.Add(this);
//		}



	}

	private void Attack(){
		if (target.TakeDamage (this.Damage)) {
			target = null;
		}

	}

	public bool TakeDamage(int damage){

		Debug.Log ("Unit " + this.name + " took " + damage + " damage");
		Health -= damage;
//		Debug.Log (string.Format ("Unit {0} remaining health is {1}", this.name, this.Health));

		if (this.name == "Dracula")
			GameController.UpdateDraculaHealthText ();

		if (Health <= 0)
			return true;
		return false;
	}

	protected virtual void BeforeDestroy(){
		if (this is BaseAI) {
			WaveSpawner.enemySpawned--;
			GameController.GiveBlood ();
		} else if (this.name == "Dracula")
			GameController.draculaAlive = false;
		else if(this is BaseUnit && !this is TowerUnit){
			unitRoom.RemoveUnit(this);
			markup.available = true;
		}
	}
}
