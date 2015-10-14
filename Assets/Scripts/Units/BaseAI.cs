using UnityEngine;
using System.Collections;

/// <summary>
/// The class for the basic AI units which extends from BaseUnit
/// </summary>
public class BaseAI : BaseUnit {


	public Waypoint nextWaypoint;

	//--------------------------------------------------------------------------------------------------------------------------------------------
	//Unity methods
	//--------------------------------------------------------------------------------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
		base.Start();
		MoveTo (nextWaypoint.transform.position);
		distanceThreshold = 1f;

	}


	//The triggering for AI should be a bit different, it should only add adversaries if they are one of the following:
	//not in combat,
	//the unit is targeting this AI,
	//the unit is Dracula, as dracula should be attacked by as many units as possible
	protected override void OnTriggerStay(Collider other){
		if (other.transform.parent != null) {
			BaseUnit unit = other.transform.parent.GetComponent<BaseUnit> ();
			
			if (unit != null && unit.enemy != this.enemy && (!unit.inCombat || unit.target == this || unit.name == "Dracula")) {
				adversariesInRange.Add (unit);
			}
		}
	}

	//--------------------------------------------------------------------------------------------------------------------------------------------

	//--------------------------------------------------------------------------------------------------------------------------------------------
	//NavMeshAgent Related methods
	//--------------------------------------------------------------------------------------------------------------------------------------------

	public override void MoveToDestination(){
		if(this.nvMsAgent.velocity.magnitude < .2f)
			this.MoveTo (nextWaypoint.transform.position);
	}

	/// <summary>
	/// If the AI reached it's waypoint, it should change the nextWaypoint variable
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	public override bool DestinationReached ()
	{

		if (nvMsAgent.destination != Vector3.zero && Vector3.Distance (nvMsAgent.destination, this.transform.position) <= distanceThreshold) {
			if (nextWaypoint.next != null){
				nextWaypoint = nextWaypoint.next;
				nvMsAgent.SetDestination (nextWaypoint.transform.position);
			}
			else
				nvMsAgent.SetDestination (this.transform.position);
			return true;
		} 
		return false;
	}

	//--------------------------------------------------------------------------------------------------------------------------------------------


	/*AI FSM:
		Moving <-> Idle
 		  |        ^ |
		  v        | |
        AICombat---  |
			^--------
	*/
	protected override void MakeFSM(){
		base.MakeFSM ();
		
		MovingState movingS = new MovingState ();
		movingS.AddTransition (Transition.DestinationReached, StateID.Idle);
		movingS.AddTransition (Transition.AdversaryDetected, StateID.AICombat);
		
		IdleState idleS = new IdleState ();
		idleS.AddTransition (Transition.AdversaryDetected, StateID.AICombat);
		idleS.AddTransition (Transition.DestinationNotReached, StateID.Moving);
		
		AICombatState aiCS = new AICombatState (this.attackSpeed);
		aiCS.AddTransition (Transition.AdversaryOutOfRange, StateID.Idle);
		
		this.FSM.AddState (movingS);
		this.FSM.AddState (idleS);
		this.FSM.AddState (aiCS);
		
		
	}



}
