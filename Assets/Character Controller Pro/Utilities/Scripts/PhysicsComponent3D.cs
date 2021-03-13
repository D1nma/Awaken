using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a PhysicsComponent for 3D physics.
/// </summary>
public sealed class PhysicsComponent3D : PhysicsComponent
{
	RaycastHit[] raycastHits = new RaycastHit[10];
	Collider[] overlappedColliders = new Collider[10];

    ContactPoint[] contactsBuffer = new ContactPoint[10];

    void OnTriggerEnter( Collider other )
    {
        OnTriggerEnterMethod( other.gameObject );
    }
    
    void OnTriggerExit( Collider other )
    {
        
        OnTriggerExitMethod( other.gameObject );
    }

    
    void OnCollisionEnter( Collision collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        AddContacts( bufferHits , true );    
    }

    void OnCollisionStay( Collision collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        AddContacts( bufferHits , false );
    }

    public override void IgnoreLayerCollision( int layerA , int layerB , bool ignore )
    {
        Physics.IgnoreLayerCollision( layerA , layerB , ignore );
    }

    protected override void AddContacts( int bufferHits , bool firstContact )
    {      

        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint contact = contactsBuffer[i];

            Contact outputContact = new Contact();

            outputContact.firstContact = firstContact;
            outputContact.collider3D = contact.otherCollider;
            outputContact.point = contact.point;
            outputContact.normal = contact.normal;
            outputContact.gameObject = outputContact.collider3D.gameObject;

            Rigidbody contactRigidbody = outputContact.collider3D.attachedRigidbody;

            if( outputContact.isRigidbody = contactRigidbody != null )
            {
                outputContact.isKinematicRigidbody = contactRigidbody.isKinematic;
                outputContact.pointVelocity = contactRigidbody.GetPointVelocity( contact.point );     
            }       


            contactsList.Add( outputContact );
        }
    }



    // Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public override int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, LayerMask layerMask, bool ignoreTrigger = true)
    {
        hits = Physics.RaycastNonAlloc(
			origin ,
			castDisplacement.normalized ,            
            raycastHits ,
            castDisplacement.magnitude ,
			layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
		);

        GetClosestHit( out hitInfo , castDisplacement , layerMask );

        return hits;
    }


	public override int CapsuleCast( out HitInfo hitInfo , Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true )
    {        
        hits = Physics.CapsuleCastNonAlloc(
            bottom ,
            top ,  
            radius ,         
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        GetClosestHit( out hitInfo , castDisplacement , layerMask );

        return hits;
    }



    public override int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true )
    {
        hits = Physics.SphereCastNonAlloc(
            center ,
            radius ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        GetClosestHit( out hitInfo , castDisplacement , layerMask );

        return hits;
    }


    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public override bool OverlapSphere( Vector3 center , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {        
        
        int hits = Physics.OverlapSphereNonAlloc(
            center ,
            radius ,
            overlappedColliders ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );
        
        this.hits = hits;

        return hits != 0;
    }

    public override bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {  

        int hits = Physics.OverlapCapsuleNonAlloc(
            bottom ,
            top ,  
            radius ,
            overlappedColliders ,
            layerMask ,
            ignoreTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        this.hits = hits;

        return hits != 0;
    }

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    void GetHitInfo( ref HitInfo hitInfo , RaycastHit raycastHit  , Vector3 castDirection)
    {

        if( raycastHit.collider != null )
        {                    
            hitInfo.point = raycastHit.point;
            hitInfo.normal = raycastHit.normal;
            hitInfo.distance = raycastHit.distance;
            hitInfo.direction = castDirection;
            hitInfo.transform = raycastHit.transform;
            hitInfo.collider3D = raycastHit.collider;
            hitInfo.rigidbody3D = raycastHit.rigidbody;     
        }
    }

    void GetClosestHit( out HitInfo hitInfo , Vector3 castDisplacement , LayerMask layerMask )
    {
        RaycastHit closestRaycastHit = new RaycastHit();
        closestRaycastHit.distance = Mathf.Infinity;

        hitInfo = new HitInfo();
        hitInfo.hit = false;

        for( int i = 0 ; i < hits ; i++ )
        {
            RaycastHit raycastHit = raycastHits[i];             

            if( raycastHit.distance == 0 )
                continue;

            hitInfo.hit = true;

            if( raycastHit.distance < closestRaycastHit.distance )
                closestRaycastHit = raycastHit;

        }

        GetHitInfo( ref hitInfo , closestRaycastHit , castDisplacement.normalized );        

    }

    


}

}