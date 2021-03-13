using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This class represents a root graphics object. This object will work independently from the character hierarchy, changing its transform properties based on the character movements and size.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Graphics")]
public class CharacterGraphics : MonoBehaviour
{       
    
    const float MaxRotationSlerpSpeed = 40f;

    /// <summary>
    /// The method used by the CharacterGraphics component to orient the graphics object towards the facing direction vector.
    /// </summary>
    public enum FacingDirectionMode
    {
        Rotation ,
        Scale
    }

    [Header("Graphics")]

    [Tooltip("Use \"Scale\" if you want to flip the sprite along the horizontal axis (the sprite must be assigned to the \"spriteTransform\" field)")]
    [SerializeField]
    FacingDirectionMode facingDirectionMode = FacingDirectionMode.Rotation;

    
    

    [Header("Rotation")]

    [Tooltip("This rotation vector will be \"added\" to the graphics object rotation. This vector should be zero if the character hierarchy + transform has been configured correctly.")]
    [SerializeField]
    Vector3 rotationOffset = Vector3.zero;

    [Tooltip("How smooth will be the rotation (slerp).")]
    [SerializeField]
    [Range( 0.1f , 1f)]
    float rotationSmoothness = 0.8f;

    [Header("Scale")]

    [Tooltip("Whether or not to affect the scale of this gameObject, based on the character actor body size property.")]
    [SerializeField]
    bool scaleAffectedByBodySize = true;
 

    

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    struct GraphicsChild
    {
        public Transform transform;
        public Vector3 initialScale;
    }

    Vector3 positionOffset = Vector3.zero;

    Vector3 initialScale = Vector3.zero;
    
    Transform characterTransform = null;
    CharacterActor characterActor = null;
    
    GraphicsChild[] childs = null;

    void Awake()
    {
        characterTransform = transform.parent;
        characterActor = characterTransform.GetComponent<CharacterActor>();
        positionOffset = transform.localPosition;

        childs = new GraphicsChild[ transform.childCount ];
        for( int i = 0 ; i < childs.Length ; i++ )
        {
            childs[i].transform = transform.GetChild( i );
            childs[i].initialScale = childs[i].transform.localScale;
        }
                
    }

    void Start()
    {
        transform.parent = null;

        initialScale = transform.localScale;
        
    }

    void OnEnable()
    {
        characterActor.OnTeleport += OnTeleportation;
    }

    void OnDisable()
    {
        characterActor.OnTeleport -= OnTeleportation;
    }

    void OnTeleportation( Vector3 position , Quaternion rotation )
    {
        transform.rotation = rotation;
    }

    void Update()
    {  
        if( characterActor == null )
        {
            Destroy( gameObject );
            return;
        }

        if( !characterActor.enabled )
            return;

        if( scaleAffectedByBodySize )
            ScaleByBodySize();
        
        float dt = Time.deltaTime;
        
        transform.position = characterTransform.position + characterTransform.TransformDirection( positionOffset );

        HandleRotation( dt );

    }

    void ScaleByBodySize()
    {
        Vector3 scale = new Vector3( 
            characterActor.BodySize.x / characterActor.DefaultBodySize.x , 
            characterActor.BodySize.y / characterActor.DefaultBodySize.y
        );

        scale.z = scale.x;

        transform.localScale = scale;
    }
    
     
    void HandleRotation( float dt )
    {       
        Vector3 forwardDirection = facingDirectionMode == FacingDirectionMode.Rotation ? characterActor.ForwardDirection : characterActor.RigidbodyRight;

        Quaternion lookRotation = Quaternion.LookRotation( forwardDirection , characterActor.RigidbodyUp );
        lookRotation *= Quaternion.Euler( rotationOffset );        
        
        transform.rotation = Quaternion.Slerp( transform.rotation , lookRotation , MaxRotationSlerpSpeed * ( 1 - ( rotationSmoothness / 1.1f) ) * dt );

        if( facingDirectionMode == FacingDirectionMode.Scale )
        {            
            
            Vector3 projectedForward = Vector3.ProjectOnPlane( characterActor.ForwardDirection , Vector3.forward );

            float signedAngle = Vector3.SignedAngle( projectedForward , characterActor.RigidbodyUp , Vector3.forward );

            for( int i = 0 ; i < childs.Length ; i++ )
            {
                Vector3 spriteLocalScale = childs[i].transform.localScale;
                spriteLocalScale.x = signedAngle < 0 ? - childs[i].initialScale.x : childs[i].initialScale.x;
                childs[i].transform.localScale = spriteLocalScale;
            }

            
        }
    }


}

}
