using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// Interface used for objects that need to be updated in a frame by frame basis.
/// </summary>
public interface IUpdatable
{
	void PreUpdateBehaviour( float dt );
	void UpdateBehaviour( float dt );
	void PostUpdateBehaviour( float dt );
}

/// <summary>
/// This class handles all the involved states from the character, allowing an organized execution of events. It also contains extra information that may be required and shared between all the states.
/// </summary>
[AddComponentMenu("Character Controller Pro/Implementation/Character/Character State Controller")]
[RequireComponent( typeof( CharacterActor ) , typeof( CharacterBrain ))]
public sealed class CharacterStateController : CharacterActorBehaviour
{
	const float MaxControlValue = 100f;
	const string UntaggedTag = "Untagged";

	[SerializeField]
	CharacterState currentState = null;

	[CustomClassDrawer]
	[SerializeField]
    EnvironmentParameters environmentParameters = new EnvironmentParameters();

	[CustomClassDrawer]
	[SerializeField]
    MovementReferenceParameters movementReferenceParameters = new MovementReferenceParameters();


	CharacterBrain characterBrain = null;

	Dictionary< string , CharacterState > states = new Dictionary< string , CharacterState >();

	public EnvironmentParameters EnvironmentParameters
	{
		get
		{
			return environmentParameters;
		}
	}

	CharacterState previousState = null;

	/// <summary>
	/// Gets the brain component associated with the state controller.
	/// </summary>
	public CharacterBrain CharacterBrain
	{
		get
		{ 
			return characterBrain;
		} 
	} 

	/// <summary>
	/// This event is called when a state transition occurs. 
	/// 
	/// The "from" and "to" states are passed as arguments.
	/// </summary>
	public event System.Action< CharacterState , CharacterState > OnStateChange;

	/// <summary>
	/// This event is called when the character enters a volume. 
	/// 
	/// The volume is passed as an argument.
	/// </summary>
	public event System.Action< Volume > OnVolumeEnter;

	/// <summary>
	/// This event is called when the character exits a volume. 
	/// 
	/// The volume is passed as an argument.
	/// </summary>
	public event System.Action< Volume > OnVolumeExit;

	/// <summary>
	/// This event is called when the character step on a surface. 
	/// 
	/// The surface is passed as an argument.
	/// </summary>
	public event System.Action< Surface > OnSurfaceEnter;

	/// <summary>
	/// This event is called when the character step off a surface. 
	/// 
	/// The surface is passed as an argument.
	/// </summary>
	public event System.Action< Surface > OnSurfaceExit;

	
	// Movement State -------------------------------------------------------------------------------------------

	/// <summary>
	/// Gets the current state used by the state machine.
	/// </summary>
	public CharacterState CurrentState
	{
		get
		{ 
			return currentState;
		} 
	}  

	/// <summary>
	/// Gets the previous state used by the state machine.
	/// </summary>
	public CharacterState PreviousState
	{
		get
		{ 
			return previousState;
		} 
	} 

	/// <summary>
	/// Searches in the database for a particular state.
	/// </summary>
	public CharacterState GetState( string stateName )
	{
		CharacterState state = null;
		bool validState = states.TryGetValue( stateName , out state );

		return state;
	}

	
	public override void Initialize( CharacterActor characterActor )
    {
		base.Initialize( characterActor );

		characterBrain = GetComponent<CharacterBrain>();

		// Get and initialize all the states.   
		GetStates();

		// Reset the material data.
		SetCurrentSurface( environmentParameters.materials.DefaultSurface );
		SetCurrentVolume( environmentParameters.materials.DefaultVolume );
	

    }

	void GetStates()
	{
		
		CharacterState[] statesArray = gameObject.GetComponents<CharacterState>();
		for (int i = 0; i < statesArray.Length ; i++)
		{
			CharacterState state = statesArray[i];
			states.Add( state.Name ,  state );			
		}
	}
    
	public override void UpdateBehaviour( float dt )
	{ 
		
		if( !characterActor.enabled )
			return;
		
		GetSurfaceData();
        GetVolumeData();

		UpdateMovementReference();
		GetInputMovementReference();

		bool changeOfState = CheckForTransitions();

		if( changeOfState )
		{
			previousState.ExitBehaviour( dt );
			currentState.EnterBehaviour( dt );
		}
		
		
		currentState.PreUpdateBehaviour( dt );
		currentState.UpdateBehaviour( dt );
		currentState.PostUpdateBehaviour( dt );

		
		
		
	}
	

	bool CheckForTransitions()
	{
		CharacterState nextState = currentState.CheckExitTransition();

		if( nextState == null )
			return false;

		bool success = nextState.CheckEnterTransition( currentState );

		if( !success )
			return false;
		
		if( OnStateChange != null )
			OnStateChange( currentState , nextState );

		previousState = currentState;
		currentState = nextState;

		return true;
	}

	
	OrthonormalReference movementReference = new OrthonormalReference();

	/// <summary>
	/// Gets the current movement orthonormal reference used by the character.
	/// </summary>
	/// <value>The movement orthonormal reference.</value>
	public OrthonormalReference MovementOrthonormalReference
	{
		get
		{
			return movementReference;
		}
	}

	void UpdateMovementReference()
    {
		switch(  movementReferenceParameters.movementReferenceMode )
		{
			case MovementReferenceParameters.MovementReferenceMode.External:
				
				if( movementReferenceParameters.externalForwardReference == null )
					return;

				movementReference.Update( movementReferenceParameters.externalForwardReference , characterActor.transform.up );
				
				break;

			case MovementReferenceParameters.MovementReferenceMode.Character:

				movementReference.Update( this.transform );
				
				break;
			case MovementReferenceParameters.MovementReferenceMode.World:

				movementReference.Update( Vector3.right , Vector3.up , Vector3.forward );
				
				break;
		}
        

        
    }

	Vector3 inputMovementReference = default( Vector3 );
	
	/// <summary>
	/// Gets a vector that is the product of the input axes (taken from the character actions) and the movement reference. 
	/// The magnitude of this vector is always less than or equal to 1.
	/// </summary>
	/// <value></value>
	public Vector3 InputMovementReference
	{
		get 
		{			
			return inputMovementReference;
		}
	}


    void GetInputMovementReference()
    {
        if( characterActor.CharacterBody.Is2D )
		{
			inputMovementReference = characterBrain.CharacterActions.inputAxes.axesValue.x * movementReference.right;
		}
		else
		{
			inputMovementReference = characterBrain.CharacterActions.inputAxes.axesValue.x * movementReference.right +
				characterBrain.CharacterActions.inputAxes.axesValue.y * movementReference.forward;
			
			Vector3.ClampMagnitude( inputMovementReference , 1f );
		}
        
    }

    
	// Environment ------------------------------------------------------
	Volume currentVolume = null;
	Surface currentSurface = null;

	// Surface ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	/// <summary>
	/// Gets the surface the character is colliding with. If this returns null the surface will be considered as "default".
	/// </summary>
	public Surface CurrentSurface
	{
		get
		{
			return currentSurface;
		}
	}

	/// <summary>
	/// Gets the current surface speed mutliplier.
	/// </summary>
	public float CurrentSurfaceSpeedMultiplier
	{
		get
		{
			return currentSurface.speedMultiplier;
		}
	}
	

	/// <summary>
	/// Gets the full surface control value, that is, the control multiplier times the maximum control.
	/// </summary>
	public float CurrentSurfaceControl
	{
		get
		{
			return currentSurface.controlMultiplier * MaxControlValue;
		}
	}

	/// <summary>
	/// Gets the value needed to reach full surface control.
	/// </summary>
	public float RemainingSurfaceControl
	{
		get
		{
			return MaxControlValue - CurrentSurfaceControl;
		}
	}

	// Volume ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	/// <summary>
	/// Gets the volume the character is colliding with. If this returns null the volume will be considered as "default".
	/// </summary>
	public Volume CurrentVolume
	{
		get
		{
			return currentVolume;
		}
	}

	/// <summary>
	/// Gets the current volume speed mutliplier.
	/// </summary>
	public float CurrentVolumeSpeedMultiplier
	{
		get
		{
			return currentVolume.speedMultiplier;
		}
	}	


	/// <summary>
	/// Gets the full volume control value, that is, the control multiplier times the maximum control.
	/// </summary>
	public float CurrentVolumeControl
	{
		get
		{
			return currentVolume.controlMultiplier * MaxControlValue;
		}
	}

	/// <summary>
	/// Gets the value needed to reach full volume control.
	/// </summary>
	public float RemainingVolumeControl
	{
		get
		{
			return MaxControlValue - CurrentVolumeControl;
		}
	}

	/// <summary>
	/// Gets the current gravity positive multiplier.
	/// </summary>
	public float CurrentGravityPositiveMultiplier
	{
		get
		{
			return currentVolume.gravityPositiveMultiplier;
		}
	}

	/// <summary>
	/// Gets the current gravity negative multiplier.
	/// </summary>
	public float CurrentGravityNegativeMultiplier
	{
		get
		{
			return currentVolume.gravityNegativeMultiplier;
		}
	}

	        
    void GetSurfaceData()
    {
        
        if( !characterActor.IsGrounded )
        {
           SetCurrentSurface( environmentParameters.materials.DefaultSurface );           
        }
        else
        {
			Surface surface = null;

			bool validSurface = environmentParameters.materials.GetSurface( characterActor.GroundObject.tag , ref surface );

			if( validSurface )
			{
				SetCurrentSurface( surface );
			}
			else
			{
				// Untagged ground
				if( characterActor.GroundObject.tag == UntaggedTag )
				{
					SetCurrentSurface( environmentParameters.materials.DefaultSurface );
				}
			}

        }
    }

	void SetCurrentSurface( Surface surface )
	{
		if( surface != currentSurface )
		{							
			if( OnSurfaceExit != null )
				OnSurfaceExit( currentSurface );

			if( OnSurfaceEnter != null )
				OnSurfaceEnter( surface );
		}

		currentSurface = surface;
	}

	
    void GetVolumeData()
    {

        if( characterActor.CurrentTrigger == null )
        {	
			if( currentVolume != environmentParameters.materials.DefaultVolume )
			{
				if( OnVolumeExit != null )
					OnVolumeExit( currentVolume );	
					
				SetCurrentVolume( environmentParameters.materials.DefaultVolume );     

			}
        }
        else
        {
			Volume volume = null;

			bool validVolume = environmentParameters.materials.GetVolume( characterActor.CurrentTrigger.tag , ref volume );

			if( validVolume )
			{
				SetCurrentVolume( volume );
			}
			else
			{
				// If the current trigger is not a valid volume, then search for one that is.

				int currentTriggerIndex = characterActor.Triggers.Count - 1;

				for( int i = currentTriggerIndex ; i >= 0 ; i-- )
				{					
					validVolume = environmentParameters.materials.GetVolume( characterActor.Triggers[i].tag , ref volume );

					if( validVolume )
					{
						SetCurrentVolume( volume );
					}
				}

				if( !validVolume )
				{
					SetCurrentVolume( environmentParameters.materials.DefaultVolume );
				}
			}
            
           
        }
    }

	void SetCurrentVolume( Volume volume )
	{
		if( volume != currentVolume )
		{							
			if( OnVolumeExit != null )
				OnVolumeExit( currentVolume );

			if( OnVolumeEnter != null )
				OnVolumeEnter( volume );
		}

		currentVolume = volume;
	}


}

[System.Serializable]
public class EnvironmentParameters
{    
	/// <summary>
	/// Materials field (scriptable object).
	/// </summary>
    public MaterialsProperties materials = null;

}


[System.Serializable]
public class MovementReferenceParameters
{    
	/// <summary>
	/// Movement reference modes.
	/// </summary>
    public enum MovementReferenceMode
    {
        World ,
        External ,
		Character
    }

	[Tooltip("Select what type of movement reference the player should be using. Should the character use its own transform, the world coordinates, or an external transform?")]
    public MovementReferenceMode movementReferenceMode = MovementReferenceMode.World;

	[Tooltip("The external transform used by the \"External\" movement reference.")]
	/// <summary>
	/// The reference transform used as a reference when the selected mode is "MovementReferenceMode.External". 
	/// </summary>
    public Transform externalForwardReference = null;
}

}
