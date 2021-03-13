using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

[RequireComponent( typeof( Rigidbody ) )]
[AddComponentMenu("Character Controller Pro/Implementation/Kinematic camera/Camera 3D")]
public class Camera3D : KinematicCamera
{
    [Header("Target")]

    [Tooltip("The character the camera is going to follow.")]
    [SerializeField]
    CharacterActor characterActor = null;

    [SerializeField]
    Vector3 offsetFromHead = Vector3.down * 0.3f;

    [SerializeField]
    bool interpolatePosition = true;
    
    [Range_NoSlider( true )]
    [SerializeField]
    float positionLerpUpSpeed = 8f;

    [Range_NoSlider( true )]
    [SerializeField]
    float positionLerpPlanarSpeed = 8f;
    

    [Header("Pitch")]
    

    [SerializeField]
    bool updatePitch = true;

    [SerializeField]
    [Range( -85f , 85f )]
    float initialPitch = 45f;

    [SerializeField]
    float pitchSpeed = 180f;      

    
    [Header("Yaw")]

    [SerializeField]
    bool updateYaw = true;

    [SerializeField]
    float yawSpeed = 180f;

    [Header("Zoom")]

    [SerializeField]
    bool updateZoom = true;

    [Range_NoSlider(true)]
    [SerializeField]
    float distanceToTarget = 5f;

    [Range_NoSlider(true)]
    [SerializeField]
    float zoomInOutSpeed = 2f;

    [Range_NoSlider(true)]
    [SerializeField]
    float zoomInOutLerpSpeed = 5f;

    [Range_NoSlider(true)]
    [SerializeField]
    float minZoom = 2f;

    [Range_NoSlider(true)]
    [SerializeField]
    float maxZoom = 12f;     
    


    [Header("Collision")]

    [SerializeField]
    bool collisionDetection = true;

    [SerializeField]
    float detectionRadius = 0.5f;
    

    [SerializeField]
    LayerMask layerMask = 0;


    CharacterBrain characterBrain = null;

    float pitch = 45f;

    float currentDistanceToTarget;

    float smoothedDistanceToTarget;    

    OrthonormalReference orthonormalReference = new OrthonormalReference();

    float deltaYaw = 0f;
    float deltaPitch = 0f;
    float deltaZoom = 0f;

    Vector3 previousTargetPosition;
    

    public OrthonormalReference OrthonormalReference
    {
        get
        {
            return orthonormalReference;
        }
    }


    

    
    protected override void Start()
    {
        base.Start();

        if( characterActor == null )
        {
            Debug.Log( "The camera " + gameObject.name + " doesn't have a target assigned." );
            this.enabled = false;
        }

        characterBrain = characterActor.GetComponent<CharacterBrain>();
        if( characterBrain == null )
        {
            Debug.Log( "There is no character brain associated with the character actor." );
            this.enabled = false;
        }


        characterPosition = characterActor.Position;

        pitch = initialPitch;
        
        currentDistanceToTarget = distanceToTarget;
        smoothedDistanceToTarget = currentDistanceToTarget;

        GameObject referenceObject = new GameObject("Camera " + gameObject.name + " reference");
        

        orthonormalReference.Update( characterActor.transform );        

        previousTargetPosition = characterActor.Position + characterActor.UpDirection * characterActor.BodySize.y + 
            characterActor.transform.TransformDirection( offsetFromHead );
    }
    
   
    void Update()
    {
        GetInputs();        
    }
    
    void GetInputs()
    {

        if( updatePitch )        
            deltaPitch = - characterBrain.CharacterActions.cameraAxes.axesValue.y;
    
        if( updateYaw )        
            deltaYaw = characterBrain.CharacterActions.cameraAxes.axesValue.x;
    
        if( updateZoom )
            deltaZoom = - characterBrain.CharacterActions.zoomAxis.axisValue;
    }

    Vector3 characterPosition = default(Vector3);    

    public override void UpdateKinematicActor( float dt )
    {
        GetInputs();
        
        characterPosition = characterActor.TargetPosition;


        Vector3 targetPosition = characterPosition + characterActor.UpDirection * characterActor.BodySize.y + characterActor.transform.TransformDirection( offsetFromHead );

        // Set the target lookAt position       
        GetTargetPosition( ref targetPosition , dt );

        // Project and set the new Forward
        orthonormalReference.forward = Vector3.ProjectOnPlane( orthonormalReference.forward , characterActor.UpDirection ).normalized;

        // Yaw rotation -----------------------------------------------------------------------------------------
        Quaternion yawRotation = updateYaw ? Quaternion.AngleAxis( deltaYaw * yawSpeed * dt , characterActor.UpDirection ) : Quaternion.identity;
        orthonormalReference.forward = yawRotation * orthonormalReference.forward;

        // Pitch rotation -----------------------------------------------------------------------------------------    
        orthonormalReference.right = Vector3.Cross( characterActor.UpDirection , orthonormalReference.forward ).normalized;

        pitch += deltaPitch * pitchSpeed * dt;
        pitch = Mathf.Clamp( pitch , -85f , 85f );
       

        Quaternion pitchRotation = Quaternion.AngleAxis( pitch , orthonormalReference.right );
        
        Vector3 lookDirection = pitchRotation * orthonormalReference.forward;

        currentDistanceToTarget += deltaZoom * zoomInOutSpeed * dt;
        currentDistanceToTarget = Mathf.Clamp( currentDistanceToTarget , minZoom , maxZoom );
        
        smoothedDistanceToTarget = Mathf.Lerp( smoothedDistanceToTarget , currentDistanceToTarget , zoomInOutLerpSpeed * dt );
        Vector3 displacement = - lookDirection * smoothedDistanceToTarget;
        
        if( collisionDetection )
            DetectCollisions( ref displacement , targetPosition );

        Vector3 finalPosition = targetPosition + displacement;
        Quaternion finalRotation = Quaternion.LookRotation( lookDirection , Vector3.Cross( lookDirection , orthonormalReference.right ).normalized );

      
        RigidbodyComponent.Position = finalPosition; 
        RigidbodyComponent.Rotation = finalRotation; 

        previousTargetPosition = targetPosition;
        
    }

   
    
    /// <summary>
    /// Sets the camera target position based on the character target.
    /// </summary>
    void GetTargetPosition( ref Vector3 targetPosition , float dt )
    {
        if( !interpolatePosition )    
            return;                
        
        Vector3 deltaTargetPosition = targetPosition - previousTargetPosition;
        
        Vector3 deltaUp = Vector3.Project( deltaTargetPosition , characterActor.UpDirection );
        Vector3 deltaPlanar = Vector3.ProjectOnPlane( deltaTargetPosition , characterActor.UpDirection );

        deltaUp = Vector3.Lerp( Vector3.zero , deltaUp , positionLerpUpSpeed * dt );
        deltaPlanar = Vector3.Lerp( Vector3.zero , deltaPlanar , positionLerpPlanarSpeed * dt );       
        
        
        targetPosition = previousTargetPosition + deltaUp + deltaPlanar;
    }
    


    void DetectCollisions( ref Vector3 displacement , Vector3 lookAtPosition )
    {
        RaycastHit collisionInfo;
        bool hit = Physics.SphereCast(
            lookAtPosition , 
            detectionRadius ,
            displacement.normalized ,
            out collisionInfo ,
            currentDistanceToTarget ,
            layerMask ,
            QueryTriggerInteraction.Ignore
        );

        if( hit )
            displacement = displacement.normalized * collisionInfo.distance;
    }

    
}

}
