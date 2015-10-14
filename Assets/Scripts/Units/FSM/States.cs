using UnityEngine;
using System.Collections;


public class IdleState : FSMState{

	public IdleState(){
		this.stateID = StateID.Idle;
	}

	//If an enemy has been detected or if the user has a destination that wasn't reached, it must change state accordingly
	public override void Reason (GameObject npc)
	{
		BaseUnit unit = npc.GetComponent<BaseUnit> ();

		if (unit.adversariesInRange.Count > 0) {

			unit.SetTransition (Transition.AdversaryDetected);
		} else if (!unit.DestinationReached ()) {
			unit.SetTransition(Transition.DestinationNotReached);
		}
	}

	//For now, the unit should do anything when idle
	public override void Act (GameObject npc)
	{
	}


}

public class MovingState : FSMState{

	public MovingState(){
		this.stateID = StateID.Moving;
	}

	//Just keep moving until you've reached your destination
	public override void Act (GameObject npc)
	{
		BaseUnit unit = npc.GetComponent<BaseUnit> ();

		unit.MoveToDestination ();
	}

	//If there are fightable enemies in range, the unit should change state
	//If the unit has reached it's destination, it should stop moving
	public override void Reason(GameObject npc){
		BaseUnit unit = npc.GetComponent<BaseUnit> ();

		if (unit.adversariesInRange.Count > 0) {
			unit.SetTransition(Transition.AdversaryDetected);
		}
		else if (unit.DestinationReached ()) {
			unit.SetTransition(Transition.DestinationReached);
		}

	}
}


public abstract class ACombatState : FSMState{

	private float attackCooldown;

	private float lastAttackTime;

	public ACombatState(float attackSpeed){
		this.stateID = StateID.Combat;

		this.attackCooldown = 1/attackSpeed;

		lastAttackTime = -this.attackCooldown;
	}

	//If there are no adversaries that are "fightable" the unit shouldn't stay in this state
	public override void Reason (GameObject npc)
	{
		BaseUnit unit = npc.GetComponent<BaseUnit> ();

		if (unit.adversariesInRange.Count == 0) {
			unit.Target(null);
			unit.SetTransition (Transition.AdversaryOutOfRange); 
		} 
	}

	//Once in combat, the unit must try to aquire a target and attack if possible
	public override void Act (GameObject npc)
	{
		BaseUnit unit = npc.GetComponent<BaseUnit>();
		unit.Stop ();

		if (unit.target == null) {
			unit.Target(unit.adversariesInRange[0]);
		}

		if (Time.time >= lastAttackTime + this.attackCooldown) {

			unit.Attack();
			lastAttackTime = Time.time;

		}

	}
	
}

public class AICombatState : ACombatState{

	public AICombatState (float attackSpeed) : base(attackSpeed)
	{
		stateID = StateID.AICombat;
	}

	//The AI should only fight units that are targeting them, in case there is no one else to check, he should get out of combat state
	public override void Reason (GameObject npc)
	{
		base.Reason (npc);
		BaseUnit unit = npc.GetComponent<BaseAI> ();

		if (unit.target != null && unit.target.target != unit && unit.adversariesInRange.Count == 1) {
			unit.Target(null);
			unit.SetTransition (Transition.AdversaryOutOfRange);
		}
	}

}



public class AllyCombatState : ACombatState{
	
	public AllyCombatState (float attackSpeed) : base(attackSpeed)
	{
		stateID = StateID.AllyCombat;
	}


	//The ally must also try to target different units that are not being targeted in case the one he is already attacking
	//already has someone else blocking
	public override void Act (GameObject npc)
	{
		BaseUnit unit = npc.GetComponent<BaseUnit> ();

		//Note that once the unit has found a valid target, he should not keep looking for another one
		if (unit.target != null && unit.target.beingTargetedList.Count > 1 && unit.adversariesInRange.Count > 1) {
			foreach (BaseUnit ai in unit.adversariesInRange) {
				if (ai.beingTargetedList.Count == 0) {
					unit.Target (ai);
					break;
				}
			}
		}
		base.Act (npc);
	}
}
