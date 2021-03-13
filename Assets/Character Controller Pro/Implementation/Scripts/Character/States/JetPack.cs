using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{


[AddComponentMenu("Character Controller Pro/Implementation/Character/States/JetPack")]
public class JetPack : CharacterState
{
    [SerializeField]
    float targetSpeed = 5f;

    [SerializeField]
    float duration = 1f;

    Vector3 smoothDampVelocity = default( Vector3 );

    Vector3 jetPackVelocity = default( Vector3 );
    Vector3 planarVelocity = default( Vector3 );

    public override string Name
    {
        get
        {
            return "JetPack";
        }
    }
    
    public override string GetInfo()
    {
        return "This state allows the character to imitate a \"JetPack\" type of movement. Basically the character can ascend towards the up direction, " + 
        "but also move in the local XZ plane.";
    }

    public override void EnterBehaviour( float dt)
    {
        jetPackVelocity = Vector3.Project( CharacterActor.InputVelocity , transform.up );
        planarVelocity = Vector3.ProjectOnPlane( CharacterActor.InputVelocity , transform.up );

        smoothDampVelocity = jetPackVelocity;
        
        CharacterActor.ForceNotGrounded();
    }

    public override void UpdateBehaviour(float dt)
    {
        
        jetPackVelocity = Vector3.SmoothDamp( jetPackVelocity , targetSpeed * transform.up , ref smoothDampVelocity , duration );
		
        StableMovement( dt );
        
        CharacterActor.SetInputVelocity( jetPackVelocity + planarVelocity );
           
    }

    public override CharacterState CheckExitTransition()
    {
        if( !CharacterBrain.CharacterActions.jetPack.isHeldDown || CharacterActor.IsGrounded)
        {
            return CharacterStateController.GetState( "NormalMovement" );
        }
        
        return null;
    }


    void StableMovement( float dt )
    {        
        
        Vector3 inputMovementReference = 
            ( CharacterBrain.CharacterActions.inputAxes.axesValue.x * CharacterStateController.MovementOrthonormalReference.right +
            CharacterBrain.CharacterActions.inputAxes.axesValue.y * CharacterStateController.MovementOrthonormalReference.forward ).normalized;

        
        float targetSpeed = 5f;

        Vector3 targetGroundVelocity = inputMovementReference * targetSpeed;

        planarVelocity = Vector3.Lerp( planarVelocity , targetGroundVelocity , 7f * dt );
        
        
        CharacterActor.SetForwardDirection( planarVelocity );

    }       
}

}
