using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

[System.Serializable]
public class PlanarMovement : CharacterAbility
{

    [Tooltip("Base speed used for the planar movement.")]
    [Range_NoSlider(true)]
    public float speed = 6f;        
    
    [Range_NoSlider( 1f , Mathf.Infinity )]
    public float boostMultiplier = 1.5f;

    [Tooltip("Amount of movement control while the character is not grounded. This is normally refer to as \"air control\".")]
    [Range( 0f , 1f )]
    public float notGroundedControl = 0.2f; 


    Vector3 planarVelocity = default( Vector3 );

    public override void Update( float dt , ref Vector3 velocity , ref Vector3 size )
    {
        SetPlanarVelocity( dt );

        velocity += planarVelocity;

    }

    void SetPlanarVelocity( float dt )
    {
        float speedMultiplier = characterStateController.CurrentVolumeSpeedMultiplier * characterStateController.CurrentSurfaceSpeedMultiplier;
        
        if( CharacterActor.IsGrounded )
        {
            if( CharacterActor.IsStable )
            {                
                if( CharacterActions.inputAxes.AxesDetected )
                {   
                    float targetSpeed = CharacterActions.run.isHeldDown ? 
                    boostMultiplier * speed * speedMultiplier : 
                    speed * speedMultiplier;

                    Vector3 targetGroundVelocity = characterStateController.InputMovementReference * targetSpeed;


                    planarVelocity = Vector3.MoveTowards( planarVelocity , targetGroundVelocity , characterStateController.CurrentSurfaceControl * dt );
                }
                else
                {
                    planarVelocity = Vector3.MoveTowards( planarVelocity , Vector3.zero , characterStateController.CurrentSurfaceControl * dt );
                }

            }
            else
            {                      

                Vector3 slidingDirection = Vector3.ProjectOnPlane( - CharacterActor.UpDirection , CharacterActor.GroundContactNormal ).normalized;
                
                planarVelocity = Vector3.ProjectOnPlane( planarVelocity , CharacterActor.GroundContactNormal );
                planarVelocity = Vector3.MoveTowards( planarVelocity , Vector3.zero , characterStateController.CurrentVolumeControl * dt );


            }
        }
        else
        {
            
            if( CharacterActions.inputAxes.AxesDetected )
            {
                float targetSpeed = speed * speedMultiplier;
                float influencedControl = characterStateController.CurrentVolumeControl + notGroundedControl * ( characterStateController.RemainingVolumeControl );

                Vector3 targetGroundVelocity = characterStateController.InputMovementReference * targetSpeed;

                planarVelocity = Vector3.MoveTowards( planarVelocity , targetGroundVelocity , influencedControl * dt );
            }
            else
            {
                planarVelocity = Vector3.MoveTowards( planarVelocity , Vector3.zero , characterStateController.CurrentVolumeControl * dt );
            }

        }
    }
}

}
