using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

[System.Serializable]
public class JumpMovement : CharacterAbility
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


    Vector3 jumpVelocity = default( Vector3 );

    public override void Update( float dt , ref Vector3 velocity , ref Vector3 size )
    {
        velocity += jumpVelocity;

    }

    
}

}

