using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

public class DemoSceneManager : MonoBehaviour
{
    [Header("Character")]

    [SerializeField]
    CharacterActor playerCharacterActor = null;
    

    [Header("Scene references")]

    [SerializeField]
    CharacterReferenceObject[] references = null;

    [Header("UI")]

    [SerializeField]
    Canvas infoCanvas = null;

    [SerializeField]
    bool hideAndConfineCursor = true;

    [Header("Graphics")]

    [Tooltip("Whether or not to show the capsule shape or the animated model.")]
    [SerializeField]
    bool showCapsule = false;

    [SerializeField]
    GameObject capsuleObject = null;

    [SerializeField]
    GameObject graphicsObject = null;

    Renderer[] capsuleRenderers = null;
    Renderer[] graphicsRenderers = null;
    

    void Awake()
    {
        if( capsuleObject != null )
            capsuleRenderers = capsuleObject.GetComponentsInChildren<Renderer>();
                
        if( graphicsObject != null )
            graphicsRenderers = graphicsObject.GetComponentsInChildren<Renderer>();

        EnableRenderers( showCapsule );

        Cursor.visible = !hideAndConfineCursor;
        Cursor.lockState = hideAndConfineCursor ? CursorLockMode.Locked : CursorLockMode.None;
        
    }

    void Update()
    {
        int index = 0;

        for( index = 0 ; index < references.Length ; index++ )
        {
                        
            if( references[index] == null )
                break;
            
            if( Input.GetKeyDown( KeyCode.Alpha1 + index ) || Input.GetKeyDown( KeyCode.Keypad1 + index ) )
            {
                GoTo( references[index] );
                break;
            }
        }

        if( Input.GetKeyDown( KeyCode.Tab ) )
        {
            if( infoCanvas != null )
                infoCanvas.enabled = !infoCanvas.enabled;
        }


        if( Input.GetKeyDown( KeyCode.T ) )
        {
            showCapsule = !showCapsule;

            EnableRenderers( showCapsule );
        }
        
    }

    

    void EnableRenderers( bool showCapsule )
    {
        if( capsuleObject != null )
            for( int i = 0 ; i < capsuleRenderers.Length ; i++ )
                capsuleRenderers[i].enabled = showCapsule;
            
        if( graphicsObject != null )
            for( int i = 0 ; i < graphicsRenderers.Length ; i++ )
                graphicsRenderers[i].enabled = !showCapsule;
    }

    void GoTo( CharacterReferenceObject reference )
    {
        if( reference == null )
            return;        
        
        if( playerCharacterActor == null )
            return;
        
        playerCharacterActor.Teleport( reference.referenceTransform );

        playerCharacterActor.SetGravityMode( reference.gravityMode );
        
        if( reference.gravityMode == CharacterOrientationMode.FixedDirection )
        {
            playerCharacterActor.SetWorldGravityDirection( reference.useNegativeUpAsGravity ? - reference.referenceTransform.up : reference.referenceTransform.up );
            
        }
        else if( reference.gravityMode == CharacterOrientationMode.GravityCenter )
        {
                        
            playerCharacterActor.SetGravityCenter( reference.gravityCenter , reference.gravityCenterMode );
            
        }
    }
}

}
