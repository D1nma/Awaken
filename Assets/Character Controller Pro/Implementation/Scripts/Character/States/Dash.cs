using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

public enum DashMode
{
	FacingDirection , 
	InputDirection 
}

[AddComponentMenu("Character Controller Pro/Implementation/Character/States/Dash")]
public class Dash : CharacterState
{    

	#region Events

	/// <summary>
	/// This event is called when the dash state is entered.
	/// 
	/// The direction of the dash is passed as an argument.
	/// </summary>
	public event System.Action<Vector3> OnDashStart;	

	/// <summary>
	/// This event is called when the dash action has ended.
	/// 
	/// The direction of the dash is passed as an argument.
	/// </summary>
	public event System.Action<Vector3> OnDashEnd;
	
	#endregion
		

	[Range_NoSlider(true)] 
	[SerializeField] 
	float initialVelocity = 12f;

	[Range_NoSlider(true)] 
	[SerializeField] 
	float duration = 0.4f;

	[SerializeField] 
	AnimationCurve movementCurve = AnimationCurve.Linear( 0,1,1,0 );
	

	[Range_NoSlider(true)] 
	[SerializeField] int availableNotGroundedDashes = 1;

	[SerializeField]
	bool ignoreSpeedMultipliers = false;

	[SerializeField]
	bool forceNotGrounded = true;

	int airDashesLeft;
	float dashCursor = 0;

	Vector3 dashDirection = Vector2.right;

	bool isDone = true;

	public override string Name
    {
        get
        {
            return "Dash";
        }
    }

	void OnEnable()
	{
		CharacterActor.OnGroundedStateEnter += OnGroundedStateEnter;
	}

	void OnDisable()
	{
		CharacterActor.OnGroundedStateEnter -= OnGroundedStateEnter;
	}

	public override string GetInfo()
    {
        return "This state is entirely based on particular movement, the \"dash\". This movement is normally a fast impulse along " + 
		"the forward direction. In this case the movement can be defined by using an animation curve (time vs velocity)";
    }

	void OnGroundedStateEnter( Vector3 localVelocity )
	{
		airDashesLeft = availableNotGroundedDashes;
	}
	

	public override bool CheckEnterTransition( CharacterState fromState )
    {
        if( !CharacterActor.IsGrounded && airDashesLeft <= 0 )
			return false;

		return true;
    }

	public override CharacterState CheckExitTransition()
    {
        if( isDone || CharacterActor.CollisionResponseContacts.Count != 0 )
        {
			if( OnDashEnd != null )
				OnDashEnd( dashDirection );

            return CharacterStateController.GetState( "NormalMovement" );
        }

        return null;
    }

	
	public override void EnterBehaviour( float dt )
	{
		if( forceNotGrounded )
			CharacterActor.ForceNotGrounded();
		

		if( CharacterActor.IsGrounded )
		{			
			if( !ignoreSpeedMultipliers )
				currentSpeedMultiplier = CharacterStateController.CurrentSurfaceSpeedMultiplier * CharacterStateController.CurrentVolumeSpeedMultiplier;
			
		}
		else
		{			
			if( !ignoreSpeedMultipliers )
				currentSpeedMultiplier = CharacterStateController.CurrentVolumeSpeedMultiplier;

			airDashesLeft--;

			
		}

		//Set the dash direction
		dashDirection = CharacterActor.ForwardDirection;		

		ResetDash();

		//Execute the event
		if( OnDashStart != null )
			OnDashStart( dashDirection );

	}

	

	float currentSpeedMultiplier = 1f;
	
	public override void UpdateBehaviour( float dt )
	{		
		Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * movementCurve.Evaluate(dashCursor) * dashDirection;		

		CharacterActor.SetInputVelocity( dashVelocity );

		float animationDt = dt / duration;
		dashCursor += animationDt; 

		if( dashCursor >= 1 )
		{
			isDone = true;
			dashCursor = 0;
		}

	}

	
	void ResetDash()
	{		
		CharacterActor.SetInputVelocity( Vector3.zero );
		isDone = false;
		dashCursor = 0;
	}

		
	
}

}


