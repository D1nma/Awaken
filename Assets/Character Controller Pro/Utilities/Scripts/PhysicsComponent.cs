using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component is an encapsulation of the Physics and Physics2D classes, containing the most commonly used 
/// methods from these components. Also it holds the information from the collision and trigger messages received, such as 
/// contacts, trigger, etc.
/// </summary>
public abstract class PhysicsComponent : MonoBehaviour
{
	protected int hits = 0;
	public List< Contact > contactsList = new List< Contact >();

	protected List<GameObject> triggers = new List<GameObject>();
	

	public List<GameObject> Triggers
	{
		get
		{
			return triggers;
		}
	}


	public event System.Action<Contact> OnCollisionEnterEvent;	

	public event System.Action<GameObject> OnTriggerEnterEvent;

	public event System.Action<GameObject> OnTriggerExitEvent;

	protected virtual void Awake()
    {
        this.hideFlags = HideFlags.HideInInspector;
    }


	protected void OnCollisionEnterMethod( Contact contact )
	{
		if( OnCollisionEnterEvent != null )
			OnCollisionEnterEvent( contact );
	}

	protected void OnTriggerEnterMethod( GameObject trigger )
	{
		triggers.Add( trigger );

		if( OnTriggerEnterEvent != null )
			OnTriggerEnterEvent( trigger );
		
	}

	protected void OnTriggerExitMethod( GameObject trigger )
	{
		if( triggers.Contains( trigger ) )
			triggers.Remove( trigger );

		if( OnTriggerExitEvent != null )
			OnTriggerExitEvent( trigger );
	}

	
	public abstract void IgnoreLayerCollision( int layerA , int layerB , bool ignore );    
	
	// Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public void ClearContacts()
	{
		contactsList.Clear();
	}

	protected abstract void AddContacts( int bufferHits , bool firstContact );

	// Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public abstract int Raycast( out HitInfo hitInfo , Vector3 origin , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true );

	public abstract int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true );

	public abstract int CapsuleCast( out HitInfo hitInfo , Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true );
    

    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public abstract bool OverlapSphere( Vector3 center , float radius , LayerMask layerMask , bool ignoreTrigger = true );

    public abstract bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , bool ignoreTrigger = true );

	

}

}

