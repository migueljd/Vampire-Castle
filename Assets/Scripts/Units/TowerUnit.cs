using UnityEngine;
using System.Collections;

public class TowerUnit : BaseUnit {

	[HideInInspector]
	public Tower spawner;
	[HideInInspector]
	public Markup destinationPosition;


	protected override void Start ()
	{
		base.Start ();
	}

	protected override void Update(){
		base.Update ();

		if (destinationPosition != null && Vector3.Distance(this.transform.position, destinationPosition.transform.position) > 1.0f)
			this.MoveTo (destinationPosition.transform.position);
	}

	protected override void OnTriggerStay(Collider other){
		base.OnTriggerStay (other);
	}

	protected override void OnTriggerEnter (Collider other)
	{
		base.OnTriggerEnter (other);
	}

	protected override void BeforeDestroy ()
	{
		spawner.MinionDestroyed (this);
		destinationPosition.available = true;
		if (this.target != null)
			target.beingTargetedList.Remove (this);
		base.BeforeDestroy ();
	}
}
