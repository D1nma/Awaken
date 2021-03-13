using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

[System.Serializable]
public class GravityMovement : CharacterAbility
{   
    

    [Tooltip("It enables/disables gravity. The gravity value is calculated based on the jump apex height and duration")]
    public bool useGravity = true;

    


    


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
        // gravityMagnitude = positiveGravityMultiplier * ( ( 2 * jumpApexHeight ) / Mathf.Pow( jumpApexDuration , 2 ) );
        // jumpSpeed = gravityMagnitude * jumpApexDuration;
    }

    public float JumpSpeed 
    {
        get
        {
            return jumpSpeed;
        }
    }


    Vector3 gravityVelocity = default( Vector3 );

    public override void Update( float dt , ref Vector3 velocity , ref Vector3 size )
    {

        velocity += gravityVelocity;

    }

    
}

}
