using UnityEngine;
using UnityEngine.UI;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This class is used for debug purposes, mainly to print information on screen about the collision flags, values and triggered events.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Debug")]
public class CharacterDebug : MonoBehaviour
{
	[SerializeField]
	Text text = null;

	[SerializeField]
    CharacterActor characterMotor = null;

	[SerializeField]
    bool debugCollisionFlags = true;

	[SerializeField]
    bool debugEvents = true;

	float time = 0f;

    void Awake()
    {
		if( debugCollisionFlags )
		{
			if( text == null || characterMotor == null )
					this.enabled = false;				
		}	
        
    }

	void Update()
	{
		if( debugCollisionFlags )
		{
			if( time > 0.2f )
			{
				text.text = characterMotor.ToString();
				
				time = 0f;
			}
			else
			{
				time += Time.deltaTime;
			}

		}
			
	}

	void OnEnable()
    {
		if( !debugEvents )
			return;
		
        characterMotor.OnWallHit += OnWallHit;
		characterMotor.OnGroundedStateEnter += OnEnterGroundedState;
		characterMotor.OnGroundedStateExit += OnExitGroundedState;
		characterMotor.OnHeadHit += OnHeadHit;
		characterMotor.OnStepUp += OnStepUp;
		characterMotor.OnTeleport += OnTeleportation;
    }

    void OnDisable()
    {
		if( !debugEvents )
			return;
		
		characterMotor.OnWallHit -= OnWallHit;
		characterMotor.OnGroundedStateEnter -= OnEnterGroundedState;
		characterMotor.OnGroundedStateExit -= OnExitGroundedState;
		characterMotor.OnHeadHit -= OnHeadHit;
		characterMotor.OnStepUp -= OnStepUp;
		characterMotor.OnTeleport -= OnTeleportation;
    }

    void OnWallHit( CollisionInfo collisionInfo )
    {
        Debug.Log( "OnWallHit" );
    }

	void OnEnterGroundedState( Vector3 localVelocity )
	{
		Debug.Log( "OnEnterGroundedState, localVelocity : " + localVelocity.ToString("F3") ); 
	}

	void OnExitGroundedState()
	{
		Debug.Log( "OnExitGroundedState" );
	}

	void OnHeadHit( CollisionInfo collisionInfo )
	{
		Debug.Log( "OnHeadHit" );
	}

	void OnStepUp( Vector3 position , float stepUpHeight )
	{
		Debug.Log( "OnStepUp , position : " + position.ToString("F3") + " Step up height : " + stepUpHeight.ToString("F2") );
	}

	void OnTeleportation( Vector3 position , Quaternion rotation )
	{
		Debug.Log( "OnTeleportation, position : " + position.ToString("F3") + " and rotation : " + rotation.ToString("F3") );
	}


}

}
