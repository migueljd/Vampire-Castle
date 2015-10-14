using UnityEngine;
using System.Collections;

public class TowerUnit : BaseAlly {

	[HideInInspector]
	public Tower spawner;
	[HideInInspector]
	public Markup destinationPosition;


	

	protected override void BeforeDestroy ()
	{
		spawner.MinionDestroyed (this);
		destinationPosition.available = true;

		base.BeforeDestroy ();
	}

	public override void MoveToDestination(){
		if(this.nvMsAgent.velocity.magnitude < .2f)
			this.MoveTo (destinationPosition.transform.position);
	}

	public override bool DestinationReached ()
	{
		return base.DestinationReached ();
	}


}
