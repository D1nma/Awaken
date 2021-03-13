using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{


/// <summary>
/// This component handles all the animation states.
/// </summary>
[AddComponentMenu("Character Controller Pro/Implementation/Character/Character Animation")]
public class CharacterAnimation : MonoBehaviour
{	
	public enum AnimatorPlayMode
	{
		Trigger ,
		PlayState
	}

	[Header("Animator")]

	[Tooltip("\"Trigger\" will consider the state parameters as trigger variables (using transitions). Make sure the triggers names in the animator controller match.\n\n" + 
	"\"PlayState\" will play directly the state (not using transitions). Again, the state names in the animator controller must match exactly.")]
	[SerializeField]
	AnimatorPlayMode animatorPlayMode = AnimatorPlayMode.Trigger;
	
	[Header("State parameters")]
	

	[SerializeField] 
	string groundedName = "Grounded";	

	[SerializeField] 
	string notGroundedName = "NotGrounded";

	[SerializeField] 
	string slideName = "Slide";

	[SerializeField] 
	string dashName = "Dash";

	[SerializeField] 
	string jetPackName = "JetPack";

	[Header("Blend tree parameters")]

	[SerializeField] 
	string notGroundedBlendName = "NotGroundedBlend";

	[SerializeField] 
	string groundBlendName = "GroundedBlend";


	[Range(0.1f , 1f)]
	[SerializeField] 
	float notGroundedBlendSensitivity = 0.4f;

	[Range_NoSlider(true)]
	[SerializeField] 
	float groundBlendLerpFactor = 1f;
	
    [Header("Inverse kinematics")]

	[SerializeField]
	bool ikFootPlacement = false;  

	[SerializeField]
	[Range_NoSlider( true )]
	float footRadius = 0.05f;

	[SerializeField]
	float ikExtraCastDistance = 0.8f;

	[SerializeField]
	string ikLeftFootWeightCurveName = "IK_LeftFoot";

	[SerializeField]
	string ikRightFootWeightCurveName = "IK_RightFoot";

	// ---------------------------------------------------------------------------------------------------  

	//hash	
	int slideHash = 0;
	int groundedHash = 0;
	int notGroundedHash = 0;
	int dashHash = 0;
	int jetPackHash = 0;
	
	float speedBlendValue = 0f;
	float verticalVelocityBlendValue = 0f;

	int currentStateHash = 0;

	CharacterStateController characterStateController = null;
    CharacterActor CharacterActor = null;
	CharacterBrain characterBrain = null;
	Animator animator = null;


	protected virtual void Awake()
	{		
		characterStateController = transform.root.GetComponent<CharacterStateController>();

        if( characterStateController == null )
        {
            Debug.Log("CharacterStateController component missing in root object \"" + transform.root.name + "\"");
            this.enabled = false;
        }

        CharacterActor = characterStateController.GetComponent<CharacterActor>();
		
		
		characterBrain = CharacterActor.GetComponent<CharacterBrain>();
		if( characterBrain == null )
			Debug.Log( "\"CharacterBrain\" component is missing, Does the parent contain this component?" );

		animator = GetComponent<Animator>();

		if( animator == null )
			Debug.Log("The Animator component is missing in object \"" + gameObject.name + "\"");
		else if( animator.runtimeAnimatorController == null )
			Debug.Log("The Runtime animator controller is empty!");

		
		slideHash = Animator.StringToHash( slideName );
		groundedHash = Animator.StringToHash( groundedName );
		notGroundedHash = Animator.StringToHash( notGroundedName );
		dashHash = Animator.StringToHash( dashName );
		jetPackHash = Animator.StringToHash( jetPackName );
	}



	void PlayFootstep(){}	//To avoid the PlayFootstep event error message
	
	void OnStateChange( CharacterState fromState , CharacterState toState )
	{		

	}

	void FixedUpdate()
	{	

		CharacterState currentState = characterStateController.CurrentState;
		
		
		Vector3 groundBlendVelocity = CharacterActor.InputVelocity;
		groundBlendVelocity = Vector3.ProjectOnPlane( groundBlendVelocity , CharacterActor.RigidbodyUp );
		speedBlendValue = groundBlendLerpFactor * groundBlendVelocity.magnitude;

		verticalVelocityBlendValue = notGroundedBlendSensitivity * CharacterActor.LocalInputVelocity.y;

		UpdateBlendTreeValues( 
			notGroundedBlendName , 
			groundBlendName , 
			verticalVelocityBlendValue , 
			speedBlendValue  
		);

		switch( currentState.Name )
		{
			case "NormalMovement":				
				
				if( CharacterActor.IsGrounded )
				{

					if( CharacterActor.IsStable )
					{	
						if( currentStateHash != groundedHash )
							PlayAnimation( groundedHash );			
								
					}
					else
					{	
						if( currentStateHash != slideHash )
							PlayAnimation( slideHash );
							
						
					}
					
					
				}
				else
				{
					if( currentStateHash != notGroundedHash )
						PlayAnimation( notGroundedHash );					
							
						
				}

				

				break;
			case "Dash":
				
				if( currentStateHash != dashHash )
					PlayAnimation( dashHash );
				

				break;

			case "JetPack":
				
				if( currentStateHash != notGroundedHash )
					PlayAnimation( notGroundedHash );
				

				break;
			default:
				break;
		}
	    
		
		
	}	

	// Virtual methods -----------------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Checks if the current animation state is equal to a given state.
	/// </summary>
	/// /// <param name="stateName">Name of the state.</param>
	protected virtual bool isCurrentlyOnState( string stateName )
	{
		return animator.GetCurrentAnimatorStateInfo(0).IsName( stateName );
	}

	/// <summary>
	/// Plays an animation state.
	/// </summary>
	/// <param name="stateName">Name of the state.</param>
	protected virtual void PlayAnimation( int stateHash )
	{
		if( animatorPlayMode == AnimatorPlayMode.PlayState )
			animator.Play( stateHash );
		else
			animator.SetTrigger( stateHash );

		currentStateHash = stateHash;
	}

	/// <summary>
	/// Sends the blend values to the blend tree.
	/// </summary>
	/// <param name="notGroundedBlendName">Name of the blend variable from the Air blend tree.</param>
	/// <param name="groundBlendName">Name of the blend variable from the Ground blend tree.</param>
	/// <param name="notGroundedBlendValue">Value of the blend variable from the Air blend tree.</param>
	/// <param name="groundBlendValue">Value of the blend variable from the Ground blend tree.</param>
	protected virtual void UpdateBlendTreeValues( string notGroundedBlendName , string groundBlendName , float notGroundedBlendValue , float groundBlendValue )
	{
		animator.SetFloat( notGroundedBlendName , notGroundedBlendValue );
		animator.SetFloat( groundBlendName , groundBlendValue );
	}


    void OnAnimatorIK( int layerIndex )
    {
        if( animator == null || !ikFootPlacement )
            return;      

        AlignFoot( AvatarIKGoal.LeftFoot , ikLeftFootWeightCurveName );
        AlignFoot( AvatarIKGoal.RightFoot , ikRightFootWeightCurveName );

    }

    void AlignFoot( AvatarIKGoal footAvatar , string ikVariableName = null )
    {
        Vector3 footPosition = animator.GetIKPosition( footAvatar );
		
		// Half the height
		float backstepCastDistance = 0.5f * CharacterActor.BodySize.y;

        Vector3 origin = footPosition + transform.up * backstepCastDistance;
	
		bool hit = false;
		float hitDistance = default( float );
		Vector3 hitPoint = default( Vector3 );
		Vector3 hitNormal = default( Vector3 );

		// Do a sphereCast
		if( CharacterActor.CharacterBody.Is2D )
		{
			RaycastHit2D hitInfo;		
			hit = PhysicsUtilities.SphereCast( 
				origin ,
				footRadius ,
				- transform.up * ( backstepCastDistance + ikExtraCastDistance ) ,
				CharacterActor.StaticObstaclesLayerMask ,
				out hitInfo 
			);

			
			if( hit )
			{
				hitPoint = hitInfo.point;
				hitNormal = hitInfo.normal;
				hitDistance = hitInfo.distance;
			}

			
		}
		else
		{
			RaycastHit hitInfo;		
			hit = PhysicsUtilities.SphereCast( 
				origin ,
				footRadius ,
				- transform.up * ( backstepCastDistance + ikExtraCastDistance ) ,
				CharacterActor.StaticObstaclesLayerMask ,
				out hitInfo 
			);

			if( hit )
			{
				hitPoint = hitInfo.point;
				hitNormal = hitInfo.normal;
				hitDistance = hitInfo.distance;
			}
		}

		// Set the foot position and rotation
		if( hit )
		{			
			Vector3 finalFootPosition = hitPoint;// + transform.up * footRadius;
			animator.SetIKPosition( footAvatar , finalFootPosition );

			Quaternion finalFootRotation = Quaternion.LookRotation( transform.forward , hitNormal );
			animator.SetIKRotation( footAvatar , finalFootRotation );

		}

		
		// Set the weights
		if( ikVariableName != null )
		{
			animator.SetIKPositionWeight( footAvatar , animator.GetFloat( ikVariableName ) );
			animator.SetIKRotationWeight( footAvatar , animator.GetFloat( ikVariableName ) );

		}
		
		
		

    }


}


}


