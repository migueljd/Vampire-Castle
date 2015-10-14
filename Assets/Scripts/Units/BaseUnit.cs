using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseUnit : MonoBehaviour {
	

	protected NavMeshAgent nvMsAgent;
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

	[HideInInspector]
	public List<BaseUnit> adversariesInRange;

	[HideInInspector]
	public bool inCombat = false;

	protected FSMSystem FSM;

	protected float distanceThreshold = .01f;



	//--------------------------------------------------------------------------------------------------------------------------------------------
	//Unity methods
	//--------------------------------------------------------------------------------------------------------------------------------------------


	// Use this for initialization
	protected virtual void Start () {

		nvMsAgent = transform.GetComponent<NavMeshAgent> ();


		beingTargetedList = new List<BaseUnit> ();
		adversariesInRange = new List<BaseUnit> ();

		MakeFSM ();

	}
	
	// Update is called once per frame
	protected virtual void Update () {
		FSM.CurrentState.Reason (this.gameObject);
		FSM.CurrentState.Act (this.gameObject);

	}

	protected virtual void LateUpdate(){
		if (Health <= 0) {
			this.BeforeDestroy();
			Destroy(this.gameObject);
			return;
		}
	}


	protected virtual void OnTriggerStay(Collider other){
		if (other.transform.parent != null) {
			BaseUnit unit = other.transform.parent.GetComponent<BaseUnit> ();
			
			if (unit != null && unit.enemy != this.enemy && !unit.inCombat) {
				adversariesInRange.Add (unit);
			}
		}
	}

	protected virtual void OnTriggerExit(Collider other){
		if (other.transform.parent != null) {
			BaseUnit unit = other.transform.parent.GetComponent<BaseUnit> ();
			
			if (unit != null && unit.enemy != this.enemy) {
				adversariesInRange.Remove(unit);
			}
		}
	}

	//--------------------------------------------------------------------------------------------------------------------------------------------


	//--------------------------------------------------------------------------------------------------------------------------------------------
	//FSM Related methods
	//--------------------------------------------------------------------------------------------------------------------------------------------

	public void SetTransition(Transition t){

		FSM.PerformTransition (t);

	}

	protected virtual void MakeFSM (){
		FSM = new FSMSystem ();
	}

	//--------------------------------------------------------------------------------------------------------------------------------------------


	//--------------------------------------------------------------------------------------------------------------------------------------------
	//NavMeshAgent Related methods
	//--------------------------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Moves to position.
	/// </summary>
	/// <param name="position">Position.</param>
	public void MoveTo(Vector3 position){
		nvMsAgent.SetDestination (position);
		nvMsAgent.Resume ();
	}

	/// <summary>
	/// Stops the unit's NavMeshAgent movement.
	/// </summary>
	public void Stop(){
		nvMsAgent.Stop ();
	}

	//TODO change this to abstract, as well as change the class to abstract
	/// <summary>
	/// Every unit should implement this method which would direct the unit to it's destination
	/// </summary>
	public virtual void MoveToDestination(){}

	/// <summary>
	/// Checks if the unit has reached it's destination.
	/// </summary>
	/// <returns><c>true</c>, if reached was destinationed, <c>false</c> otherwise.</returns>
	public virtual bool DestinationReached(){
		if (nvMsAgent.destination != Vector3.zero && Vector3.Distance (nvMsAgent.destination, this.transform.position) <= distanceThreshold) {
			nvMsAgent.SetDestination(this.transform.position);
			return true;
		}
		return false;
	}

	//--------------------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// This basically just makes the unit deal damage to another unit if it has a target
	/// </summary>
	public void Attack(){
		if (target != null && target.TakeDamage (this.Damage)) {
			this.Target(null);
		}

	}


	/// <summary>
	/// Takes the damage. The unit should take damage and inform to the caller if it died or not.
	/// Also if this is the Dracula, we should update the UI element that displays his HP
	/// </summary>
	/// <returns><c>true</c>, if damage was taken, <c>false</c> otherwise.</returns>
	/// <param name="damage">Damage.</param>
	public bool TakeDamage(int damage){

		Debug.Log ("Unit " + this.name + " took " + damage + " damage");
		Health -= damage;

		if (this.name == "Dracula")
			GameController.UpdateDraculaHealthText ();

		if (Health <= 0)
			return true;
		return false;
	}
	
	/// <summary>
	/// Target the specified newTarget. Changes the target to the new target, remove this from the beingTargetedList from the old target, 
		/// add this to the beingTargetedList from the @newTarget
	/// </summary>
	/// <param name="newTarget">New target.</param>
	public void Target(BaseUnit newTarget){

		if (target != null)
			target.beingTargetedList.Remove (this);

		target = newTarget;
		if (target != null) {
			target.beingTargetedList.Add (this);
			inCombat = true;
		} else {
			inCombat = false;
		}

	}

	/// <summary>
	/// Notify this unit that a given adversary has died
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void AdversaryDied(BaseUnit unit){
		if (unit == target)
			Target (null);
	
	}

	/// <summary>
	/// This should ALWAYS be called when a unit is about to die
	/// </summary>
	protected virtual void BeforeDestroy(){

		this.Target (null);

		if(beingTargetedList.Count > 0){
			foreach(BaseUnit unit in beingTargetedList){
				unit.AdversaryDied(this);

			}
		}
	}
	
}
