using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

[AddComponentMenu("Character Controller Pro/Implementation/Character/States/Normal movement")]
public class NormalMovement : CharacterState
{   
    
    [Space(10)]

    
    [CustomClassDrawer]
    [SerializeField]
    PlanarMovementParameters planarMovementParameters = new PlanarMovementParameters();

    [CustomClassDrawer]
    [SerializeField]
    VerticalMovementParameters verticalMovementParameters = new VerticalMovementParameters(); 

    [CustomClassDrawer]
    [SerializeField]
    ShrinkParameters shrinkParameters = new ShrinkParameters();

    [CustomClassDrawer]
    [SerializeField]
    RigidbodyResponseParameters rigidbodyResponseParameters = new RigidbodyResponseParameters();


    
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    
    #region Events	

    /// <summary>
    /// Event triggered when the character jumps.
    /// </summary>
	public event System.Action OnJumpPerformed;

    /// <summary>
    /// Event triggered when the character jumps from the ground.
    /// </summary>
	public event System.Action OnGroundedJumpPerformed;

    /// <summary>
    /// Event triggered when the character jumps while.
    /// </summary>
	public event System.Action<int> OnNotGroundedJumpPerformed;
	
	#endregion

    Vector3 planarVelocity = default( Vector3 );
    Vector3 verticalVelocity =  default( Vector3 );
    Vector3 externalVelocity =  default( Vector3 );
      
    
    public void ResetVelocities()
    {
        planarVelocity = Vector3.zero;
        verticalVelocity = Vector3.zero;
        externalVelocity = Vector3.zero;
        jumpVelocity = Vector3.zero;

        CharacterActor.SetInputVelocity( Vector3.zero );
    }

    int notGroundedJumpsLeft = 0;
	float jumpTimer = 0f;

    bool isJumping = false;

    
    public override string Name
    {
        get
        {
            return "NormalMovement";
        }
    }

    
    protected override void Awake()
    {
        base.Awake();       

        notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;
        
        
    }

    void Start()
    {
        targetHeight = CharacterActor.DefaultBodySize.y;

        float minShrinkHeightRatio = CharacterActor.BodySize.x / CharacterActor.BodySize.y;
        shrinkParameters.shrinkHeightRatio = Mathf.Max( minShrinkHeightRatio , shrinkParameters.shrinkHeightRatio );

    }

    void OnEnable()
    {
        CharacterActor.OnHeadHit += OnHeadHit;
        // CharacterActor.OnContactHit += OnContactHit;
        CharacterActor.OnTeleport += OnTeleport;

        CharacterStateController.OnVolumeEnter += OnEnterVolume;
        
    }

    void OnDisable()
    {
        CharacterActor.OnHeadHit -= OnHeadHit;
        // CharacterActor.OnContactHit -= OnContactHit;
        CharacterActor.OnTeleport -= OnTeleport;

        CharacterStateController.OnVolumeEnter -= OnEnterVolume;
    }

    public override string GetInfo()
    {
        return "This state serves as a multi purpose movement based state. It is responsible for handling gravity and jump, walk and run, shrink, " + 
        "react to the different material properties, react to external impulses, etc. Basically it covers all the common movements involved " + 
        "in a typical game, from a 3D platformer to a first person walking simulator.";
    }

    

    void OnHeadHit( CollisionInfo collisionInfo )
    {
        verticalVelocity = Vector3.zero;
    }


    // void OnContactHit( Vector3 contactVelocity )
    // {
    //     if( !rigidbodyResponseParameters.reactToRigidbodies )
    //         return;
        
    //     externalVelocity += contactVelocity * rigidbodyResponseParameters.responseMultiplier;
        
    // }

    void OnTeleport( Vector3 position , Quaternion rotation )
    {
        ResetVelocities();
    }

    void OnEnterVolume( Volume volume )
    {
        verticalVelocity *= volume.speedMultiplier;
    }

    /// <summary>
    /// Gets/Sets the useGravity toggle. Use this property to enable/disable the effect of gravity on the character.
    /// </summary>
    /// <value></value>
    public bool UseGravity
    {
        get
        {
            return verticalMovementParameters.useGravity;
        }
        set
        {
            verticalMovementParameters.useGravity = value;
        }
    }

    public override CharacterState CheckExitTransition()
    {
        
        CharacterState state = null;

        if( CharacterActions.jetPack.isHeldDown )
        {
            state = CharacterStateController.GetState( "JetPack" );
        }
        else if( CharacterActions.dash.isPressed )
        {
            state = CharacterStateController.GetState( "Dash" );            
        }
        else if( CharacterActions.interact.isPressed )   
        {
            if( CharacterActor.CurrentTrigger != null )  // <--- LadderClimb
            {                
                state = CharacterStateController.GetState( "LadderClimb" );
            }
        }
        else if( !CharacterActor.IsGrounded )
        {            
            state = CharacterStateController.GetState( "LedgeHanging" );
        }
        
        return state;
    }


    

    

    void HandleForwardDirection()
    {
        Vector3 forwardDirection;

        if( CharacterActor.IsGrounded )
        {
            if( CharacterActor.IsStable ) 
                forwardDirection = CharacterStateController.InputMovementReference;
            else
                forwardDirection = CharacterActor.InputVelocity;
        }
        else
        {
            forwardDirection = planarVelocity;
        }

        CharacterActor.SetForwardDirection( forwardDirection );

        
    }
    
    void ProcessPlanarMovement( float dt )
    {        
        float speedMultiplier = CharacterStateController.CurrentVolumeSpeedMultiplier * CharacterStateController.CurrentSurfaceSpeedMultiplier;
        
        if( CharacterActor.IsGrounded )
        {
            if( CharacterActor.IsStable )
            {                
                if( CharacterActions.inputAxes.AxesDetected )
                {   
                    float targetSpeed = CharacterActions.run.isHeldDown ? 
                    planarMovementParameters.boostMultiplier * planarMovementParameters.speed * speedMultiplier : 
                    planarMovementParameters.speed * speedMultiplier;

                    Vector3 targetGroundVelocity = CharacterStateController.InputMovementReference * targetSpeed;


                    planarVelocity = Vector3.MoveTowards( planarVelocity , targetGroundVelocity , CharacterStateController.CurrentSurfaceControl * dt );
                }
                else
                {
                    planarVelocity = Vector3.MoveTowards( planarVelocity , Vector3.zero , CharacterStateController.CurrentSurfaceControl * dt );
                }

            }
            else
            {                      

                Vector3 slidingDirection = Vector3.ProjectOnPlane( - transform.up , CharacterActor.GroundContactNormal ).normalized;
                
                planarVelocity = Vector3.ProjectOnPlane( planarVelocity , CharacterActor.GroundContactNormal );
                planarVelocity = Vector3.MoveTowards( planarVelocity , Vector3.zero , CharacterStateController.CurrentVolumeControl * dt );


            }
        }
        else
        {
            
            if( CharacterActions.inputAxes.AxesDetected )
            {
                float targetSpeed = planarMovementParameters.speed * speedMultiplier;
                float influencedControl = CharacterStateController.CurrentVolumeControl + planarMovementParameters.notGroundedControl * ( CharacterStateController.RemainingVolumeControl );

                Vector3 targetGroundVelocity = CharacterStateController.InputMovementReference * targetSpeed;

                planarVelocity = Vector3.MoveTowards( planarVelocity , targetGroundVelocity , influencedControl * dt );
            }
            else
            {
                planarVelocity = Vector3.MoveTowards( planarVelocity , Vector3.zero , CharacterStateController.CurrentVolumeControl * dt );
            }

        }

        
    }



    

    void ProcessGravity( float dt )
    {
        verticalMovementParameters.UpdateParameters( CharacterStateController.CurrentGravityPositiveMultiplier );

        bool positiveGravity = CharacterActor.LocalInputVelocity.y >= 0;
        float gravityMultiplier = positiveGravity ? CharacterStateController.CurrentGravityPositiveMultiplier : CharacterStateController.CurrentGravityNegativeMultiplier;
        float gravity = gravityMultiplier * verticalMovementParameters.GravityMagnitude;

        if( CharacterActor.IsGrounded )
        {     
            if( !CharacterActor.IsStable )       
            {                                                
                //float verticalComponent = transform.InverseTransformDirection( verticalVelocity ).y;

                Vector3 newVerticalDirection = Vector3.ProjectOnPlane( transform.up , CharacterActor.GroundContactNormal ).normalized;
                Vector3 oldVerticalDirection = verticalVelocity.normalized;

                // if the new and old directions are not "equals" the vertical velocity vector needs to change its direction
                if( newVerticalDirection != oldVerticalDirection || !CharacterActor.WasGrounded )
                {
                    verticalVelocity = Vector3.ProjectOnPlane( verticalVelocity , CharacterActor.GroundContactNormal );
                }
                
                verticalVelocity -= newVerticalDirection * gravity * dt;
                
            }
            
                        
        }
        else
        {
            if( CharacterActor.WasGrounded )
            {
                verticalVelocity += Vector3.Project( CharacterActor.RigidbodyUp , planarVelocity );
            }

            verticalVelocity += - transform.up * ( gravity * dt );
            
        }

    }



    

    /// <summary>
    /// Apply the jump velocity to the vertical velocity vector, based on the current jump state.
    /// </summary>
    void ProcessJump( float dt )
    {      
        if( CharacterActor.IsGrounded )
            notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;
        

        if( isJumping )
        {
            verticalVelocity = jumpVelocity;
            jumpTimer += dt;

            if( jumpTimer > verticalMovementParameters.constantJumpDuration || CharacterActions.jump.isReleased )
            {
                isJumping = false;
                jumpVelocity = Vector3.zero;
                jumpTimer = 0f;
            }

            
        }
        else
        {      
            // Calculate the jump velocity vector ------------------------------------------------------
            if( CharacterActions.jump.isPressed )
            {
                
                if( CharacterActor.IsGrounded )
                {              
                    
                    SetJumpVelocity(); 
                    CharacterActor.ForceNotGrounded();  // Must be called AFTER setting the velocity ...

                    if( verticalMovementParameters.jumpReleaseAction == VerticalMovementParameters.JumpReleaseAction.StopJumping )
                    {
                        isJumping = true;
                        jumpTimer = 0f;
                    }

                    

                    verticalVelocity = jumpVelocity;

                }
                else
                {
                    if( notGroundedJumpsLeft != 0 )
                    {
                        
                        SetJumpVelocity();                        
                        
                        notGroundedJumpsLeft--;

                        if( verticalMovementParameters.jumpReleaseAction == VerticalMovementParameters.JumpReleaseAction.StopJumping )
                        {
                            isJumping = true;
                            jumpTimer = 0f;
                        }

                        verticalVelocity = jumpVelocity;
                    }
                    
                }

                
            }
        }
                
		
    }

    Vector3 jumpVelocity = default( Vector3 );
        

    void SetJumpVelocity()
	{		
        
        Vector3 verticalJumpComponent = Vector3.Project( verticalVelocity , transform.up );
        Vector3 verticalExtraComponent = verticalMovementParameters.jumpIntertiaMultiplier * ( verticalVelocity - verticalJumpComponent );


		if( CharacterActor.IsGrounded )
		{       
            if( CharacterActor.IsStable )
            {
                verticalJumpComponent = - CharacterActor.CurrentGravityDirection * verticalMovementParameters.JumpSpeed;  
                verticalExtraComponent = Vector3.zero;				 

                if( OnGroundedJumpPerformed != null )
                    OnGroundedJumpPerformed();	

            }
            else
            {
                switch( verticalMovementParameters.unstableJumpMode )
                {
                    case VerticalMovementParameters.UnstableJumpMode.Vertical:

                        verticalJumpComponent = - CharacterActor.CurrentGravityDirection * verticalMovementParameters.JumpSpeed;

                        break;
                    case VerticalMovementParameters.UnstableJumpMode.GroundNormal:

                        verticalJumpComponent = CharacterActor.GroundContactNormal * verticalMovementParameters.JumpSpeed;
                        
                        break;
                } 

            }
			
		}	
		else
		{
            verticalJumpComponent = - CharacterActor.CurrentGravityDirection * verticalMovementParameters.JumpSpeed;             

            if( OnNotGroundedJumpPerformed != null )
                OnNotGroundedJumpPerformed( notGroundedJumpsLeft );		
			
		}
        
        jumpVelocity = verticalJumpComponent + verticalExtraComponent;

		if( OnJumpPerformed != null )
			OnJumpPerformed();
		
	}
    
    
    void ProcessVerticalMovement( float dt )
    {       
        if( verticalMovementParameters.useGravity )
            ProcessGravity( dt );
        
        VerticalDrag( dt );

        ProcessJump( dt );
        
        
    }
    
    
    
    void VerticalDrag( float dt )
    {
        if( CharacterActor.IsGrounded && CharacterActor.IsStable )
        {            
            Vector3 verticalComponent = Vector3.Project( verticalVelocity , CharacterActor.CurrentGravityDirection );
            Vector3 extraComponent = verticalVelocity - verticalComponent;

            extraComponent = Vector3.MoveTowards( extraComponent , Vector3.zero , CharacterStateController.CurrentSurfaceControl * dt );

            verticalVelocity = extraComponent;
        }
        else
        {
            Vector3 verticalComponent = Vector3.Project( verticalVelocity , CharacterActor.CurrentGravityDirection );
            Vector3 extraComponent = verticalVelocity - verticalComponent;

            extraComponent = Vector3.MoveTowards( extraComponent , Vector3.zero , CharacterStateController.CurrentVolumeControl * dt );

            verticalVelocity = verticalComponent + extraComponent;
        }

        
    }

    void ProcessExternalMovement( float dt )
    {

        for( int i = 0 ; i < CharacterActor.CollisionResponseContacts.Count ; i++ )
        {            
            Contact contact = CharacterActor.CollisionResponseContacts[i];

            Vector3 contactVelocity = Vector3.ClampMagnitude( contact.pointVelocity , rigidbodyResponseParameters.maxContactVelocity );
            externalVelocity += contactVelocity * rigidbodyResponseParameters.responseMultiplier;
              
            
        }
    }

    void ExternalDrag( float dt )
    {
        if( externalVelocity == Vector3.zero )
            return;
        
        float friction = CharacterActor.IsGrounded ? CharacterStateController.CurrentSurfaceControl : CharacterStateController.CurrentVolumeControl;
        externalVelocity = Vector3.MoveTowards( externalVelocity , Vector3.zero , friction * dt );
        
        
    }


    public override void EnterBehaviour( float dt )
    {
        planarVelocity = Vector3.ProjectOnPlane( CharacterActor.InputVelocity , transform.up );
        verticalVelocity = Vector3.Project( CharacterActor.InputVelocity , transform.up );
        jumpVelocity = Vector3.zero;        
        externalVelocity = Vector3.zero;
    }    

    

    public override void UpdateBehaviour( float dt )
    {        
        HandleSize( dt );
        HandleMovement( dt );

        HandleForwardDirection();
    }
    

    float targetHeight = 1f;
    bool wantToShrink = false;

    void HandleSize( float dt )
    {      
        

        if( shrinkParameters.shrinkMode == ShrinkParameters.ShrinkMode.Toggle )
        {
            if( CharacterActions.shrink.isPressed )
                wantToShrink = !wantToShrink;            
        } 
        else
        {
            wantToShrink = CharacterActions.shrink.isHeldDown;
            
        }

        targetHeight = wantToShrink ? CharacterActor.DefaultBodySize.y * shrinkParameters.shrinkHeightRatio : CharacterActor.DefaultBodySize.y;

        Vector3 targetSize = new Vector2( CharacterActor.DefaultBodySize.x , targetHeight );

        CharacterActor.SetTargetBodySize( targetSize );
       
    }


    void HandleMovement( float dt )
    {
        ExternalDrag( dt );
               
        ProcessVerticalMovement( dt );
        ProcessPlanarMovement( dt );   
        ProcessExternalMovement( dt );

        Vector3 velocity = planarVelocity + verticalVelocity + externalVelocity;
        CharacterActor.SetInputVelocity( velocity );
        
    }

    
    

}

[System.Serializable]
public class PlanarMovementParameters
{
    [Tooltip("Base speed used for the planar movement.")]
    [Range_NoSlider(true)]
    public float speed = 6f;    
    
    
    [Range_NoSlider( 1f , Mathf.Infinity )]
    public float boostMultiplier = 1.5f;

    [Tooltip("Amount of movement control while the character is not grounded. This is normally refer to as \"air control\".")]
    [Range( 0f , 1f )]
    public float notGroundedControl = 0.2f; 

}


[System.Serializable]
public class VerticalMovementParameters
{    

    public enum UnstableJumpMode
    {
        Vertical ,
        GroundNormal
    }

    public enum JumpReleaseAction
    {
        Disabled ,
        StopJumping
    }
    

    [Tooltip("It enables/disables gravity. The gravity value is calculated based on the jump apex height and duration")]
    public bool useGravity = true;

    [Tooltip("The height reached at the apex of the jump. The maximum height will depend on the \"jumpCancellationMode\".")]
    [Range_NoSlider(true)]
    public float jumpApexHeight = 1f;

    [Tooltip("The amount of time to reach the \"base height\" (apex).")]
    [Range_NoSlider(true)]
    public float jumpApexDuration = 0.3f;


    [Tooltip("Number of jumps available for the character in the air.")]
	public int availableNotGroundedJumps = 1;
    
    [Tooltip("How should the character jump on unstable ground?\n\nVertical = the jump is performed considering only the up direction.\nGroundNormal = the jump is performed considering the up direction and the ground normal.")]
    public UnstableJumpMode unstableJumpMode = UnstableJumpMode.GroundNormal;

    [Tooltip("How much of the current slide velocity (unstable movement) is converted into the jump velocity.")]
    [Range( 0f , 1f )]
    public float jumpIntertiaMultiplier = 0.5f;

    [Space] 

    [Tooltip("How the release of the jump input affects the jump.\n" +         
        "Disabled = no action at all.\n" + 
        "StopJumping = the vertical velocity will be maintained as \"jumpVelocity\" for a certain duration of time. This will produce a higher jump than the base one (\"KeepJumping\" jump height >= base jump height).\n"
    )]
    public JumpReleaseAction jumpReleaseAction = JumpReleaseAction.StopJumping;

    [Tooltip("[Only for the \"StopJumping\" action] It represents the amount of seconds in which the character will maintain the vertical velocity constant (as long as the jump input is being held down).")]
	[Range_NoSlider(true)] 
    public float constantJumpDuration = 0.2f;


    float gravityMagnitude = 10f;
    
    public float GravityMagnitude
    {
        get
        {
            return gravityMagnitude;
        }
    }

    float jumpSpeed = 10f;

    public void UpdateParameters( float positiveGravityMultiplier )
    { 
        gravityMagnitude = positiveGravityMultiplier * ( ( 2 * jumpApexHeight ) / Mathf.Pow( jumpApexDuration , 2 ) );
        jumpSpeed = gravityMagnitude * jumpApexDuration;
    }

    public float JumpSpeed 
    {
        get
        {
            return jumpSpeed;
        }
    }

}

[System.Serializable]
public class ShrinkParameters
{    
    [Tooltip("This multiplier represents how much of the height.")]
    [Range_NoSlider(true)]
    public float shrinkHeightRatio = 0.5f;

    
    public enum ShrinkMode
    {
        Toggle ,
        Hold
    }

    [Tooltip("\"Toggle\" will activate/deactivate the action when the input is \"pressed\". On the other hand, \"Hold\" will activate the action when the input is pressed, and deactivate it when the input is \"released\".")]
    public ShrinkMode shrinkMode = ShrinkMode.Hold;

}

[System.Serializable]
public class RigidbodyResponseParameters
{    
    public bool reactToRigidbodies = true;

    [Tooltip("Multiplier of the collision response.\n\nFor example:\n1 -> The original response.\n10 -> ten times the original response.")]
    [Range_NoSlider(true)]
    public float responseMultiplier = 1f;

    [Tooltip("The point velocity obtained from the contact will be clamped using this value.")]
    [Range_NoSlider(true)]
    public float maxContactVelocity = 10f;

}









}
