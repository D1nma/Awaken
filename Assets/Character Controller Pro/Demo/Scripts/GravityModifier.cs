using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

public abstract class GravityModifier : MonoBehaviour
{
    
    [SerializeField]
    CharacterReferenceObject reference = new CharacterReferenceObject();

    [Tooltip("The duration this modifier will be inactive once is activated. " + 
    "Use this to prevent the character from reactivating the trigger over and over again (the default value of 1 second should be more than fine.)")]
    [SerializeField]
    float waitTime = 1f;

    protected bool isReady = true;
    float time = 0f;

    protected Dictionary< Transform , CharacterActor > characters = new Dictionary<Transform, CharacterActor>();

    
    void Update()
    {
        if( isReady )
            return;

        time += Time.deltaTime;

        if( time >= waitTime )
        {
            time = 0f;
            isReady = true;
        }
    }

    protected void ChangeGravitySettings( CharacterActor characterActor )
    {
        if( reference == null )
            return;                
        
        
        characterActor.Teleport( reference.referenceTransform );

        characterActor.SetGravityMode( reference.gravityMode );
        
        if( reference.gravityMode == CharacterOrientationMode.FixedDirection )
        {
            characterActor.SetWorldGravityDirection( reference.useNegativeUpAsGravity ? - reference.referenceTransform.up : reference.referenceTransform.up );
            
        }
        else if( reference.gravityMode == CharacterOrientationMode.GravityCenter )
        {
                        
            characterActor.SetGravityCenter( reference.gravityCenter , reference.gravityCenterMode );
            
        }

        isReady = false;
    }
    
    protected CharacterActor GetCharacter( Transform objectTransform )
    {
        CharacterActor characterActor;
		bool found = characters.TryGetValue( objectTransform , out characterActor );

		if( !found )
        {		
			characterActor = objectTransform.GetComponent<CharacterActor>();

			if( characterActor != null )
				characters.Add( objectTransform , characterActor );			
			
		}

        return characterActor;
    }
}

}


