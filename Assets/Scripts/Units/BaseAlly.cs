using UnityEngine;
using System.Collections;

/// <summary>
/// The basic ally unit, which extends from BaseUnit
/// </summary>
public class BaseAlly : BaseUnit {

	/*ALLY FSM:
		Moving <-> Idle
 		  |        ^ |
		  v        | |
      AllyCombat---  |
			^--------
	 */
	protected override void MakeFSM(){
		base.MakeFSM ();

		MovingState movingS = new MovingState ();
		movingS.AddTransition (Transition.DestinationReached, StateID.Idle);
		movingS.AddTransition (Transition.AdversaryDetected, StateID.AllyCombat);
		
		IdleState idleS = new IdleState ();
		idleS.AddTransition (Transition.AdversaryDetected, StateID.AllyCombat);
		idleS.AddTransition (Transition.DestinationNotReached, StateID.Moving);
		
		AllyCombatState allyCS = new AllyCombatState (this.attackSpeed);
		allyCS.AddTransition (Transition.AdversaryOutOfRange, StateID.Idle);

		this.FSM.AddState (movingS);
		this.FSM.AddState (idleS);
		this.FSM.AddState (allyCS);


	}


}
