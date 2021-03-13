using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

public class Ladder : MonoBehaviour
{
    enum FacingDirection
    {
        PositiveZ ,
        NegativeZ ,
        PositiveX ,
        NegativeX
    }

    [Header( "Debug" )]
    [SerializeField]
    bool showGizmos = true;


    [Header("Exit points")]
    [SerializeField]
    Transform topExit = null;

    [SerializeField]
    Transform bottomExit = null;

    [Header("Properties")]

    [Range_NoSlider( 3 , int.MaxValue )]
    [SerializeField]
    int stepsNumber = 10;

    [SerializeField]
    Vector3 topLocalPosition = Vector3.zero;

    [SerializeField]
    Vector3 bottomLocalPosition = Vector3.zero;

    [SerializeField]
    FacingDirection facingDirection = FacingDirection.PositiveZ;

    // ColliderComponent colliderComponent = null;

    Vector3 facingDirectionVector = Vector3.zero;

    List<Vector3> steps = new List<Vector3>();

    public List<Vector3> Steps
    {
        get
        {
            return steps;
        }
    }

    public Transform TopExit
    {
        get
        {            
            return topExit;
        }
    }

    public Transform BottomExit
    {
        get
        {
            return bottomExit;
        }
    }

    public Vector3 TopPosition
    {
        get
        {
            return transform.position + topLocalPosition;
        }
    }

    public Vector3 BottomPosition
    {
        get
        {
            return transform.position + bottomLocalPosition;
        }
    }

    public Vector3 BottomToTop
    {
        get
        {
            return TopPosition - BottomPosition;
        }
    }

    public Vector3 FacingDirectionVector
    {
        get
        {
            return facingDirectionVector;
        }
    }

    public int GetClosestStepIndex( Vector3 referencePosition )
    {
        int outputIndex = 0;
        float minSqrDistance = Mathf.Infinity;

        for( int i = 0 ; i < steps.Count ; i++ )
        {
            float sqrDistance = Vector3.SqrMagnitude( steps[i] - referencePosition );
            
            if( sqrDistance < minSqrDistance )
            {
                minSqrDistance = sqrDistance;
                outputIndex = i;
            }
        }

        return outputIndex;
    }

    void Awake()
    {
        
        switch( facingDirection )
        {
            case FacingDirection.PositiveZ:
                facingDirectionVector = transform.forward;
                break;
            case FacingDirection.NegativeZ:
                facingDirectionVector = - transform.forward;
                break;
            case FacingDirection.PositiveX:
                facingDirectionVector = transform.right;
                break;
            case FacingDirection.NegativeX:
                facingDirectionVector = - transform.right;
                break;
        }

        Vector3 minDisplacement = ( 1f / ( stepsNumber - 1 ) ) * BottomToTop;

        for( int i = 0 ; i < stepsNumber ; i++ )
        {
            Vector3 stepPosition = BottomPosition + i * minDisplacement;
            steps.Add( stepPosition );
        }
    }

    void OnDrawGizmos()
    {
        if( !showGizmos )
            return;
        
        Gizmos.color = new Color( 0f , 1f , 0f , 0.2f );
        Gizmos.DrawSphere( transform.position + topLocalPosition , 0.5f );

        Gizmos.color = new Color( 0f , 0f , 1f , 0.2f );
        Gizmos.DrawSphere( transform.position + bottomLocalPosition , 0.5f );

        Gizmos.color = new Color( 1f , 0f , 0f , 0.5f );

        Vector3 minDisplacement = ( 1f / ( stepsNumber - 1 ) ) * BottomToTop;

        for( int i = 0 ; i < stepsNumber ; i++ )
        {
            Vector3 stepPosition = BottomPosition + i * minDisplacement;
            Gizmos.DrawSphere( stepPosition , 0.1f );
        }
        
        if( topExit != null )
        {
            Gizmos.color = new Color( 0f , 1f , 0f , 0.2f );
            Gizmos.DrawCube( topExit.position , Vector3.one * 0.5f );
        }

        if( bottomExit != null )
        {
            Gizmos.color = new Color( 0f , 0f , 1f , 0.2f );
            Gizmos.DrawCube( bottomExit.position , Vector3.one * 0.5f );
        }
        
        switch( facingDirection )
        {
            case FacingDirection.PositiveZ:
                facingDirectionVector = transform.forward;
                break;
            case FacingDirection.NegativeZ:
                facingDirectionVector = - transform.forward;
                break;
            case FacingDirection.PositiveX:
                facingDirectionVector = transform.right;
                break;
            case FacingDirection.NegativeX:
                facingDirectionVector = - transform.right;
                break;
        }

        CustomUtilities.DrawArrowGizmo( transform.position , transform.position + facingDirectionVector , Color.blue );


        Gizmos.color = Color.white;
    }
    
}
