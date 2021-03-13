using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Implementation
{

public enum AIBehaviourType
{
	Sequence ,
	FollowTarget
}

public enum HumanInputType
{
	UnityInputManager ,
	UI_Mobile ,
	Custom
}


/// <summary>
/// This class is responsable for detecting inputs and managing character actions. These actions may come from a human (the player) or an AI.
/// </summary>
[RequireComponent( typeof(CharacterActor) )]
[AddComponentMenu("Character Controller Pro/Implementation/Character/Character Brain")]
// [DefaultExecutionOrder(-1)]
public class CharacterBrain : MonoBehaviour
{
    
    [SerializeField]
    bool isAI = false;


    // Human brain ----------------------------------------------------------------------------
    [SerializeField]
	InputHandler inputHandler = null;

	[SerializeField]
	HumanInputType humanInputType = HumanInputType.UnityInputManager;

	public CharacterInputData inputData;
	

    // AI brain -------------------------------------------------------------------------------
    public AIBehaviourType behaviourType;

	[Tooltip("Set it to true if you want to force an axis value (float) to an integer value (0, -1 or 1). This is used only for the Unity's Input Manager type.")]
	[SerializeField]
	bool useRawAxes = true;
	
	[SerializeField]
	CharacterAISequenceBehaviour sequenceBehaviour = null;


	[Tooltip("The target transform used by the follow behaviour.")]
	[SerializeField] 
	Transform followTarget = null;

	[Tooltip("Desired distance to the target. if the distance to the target is less than this value the character will not move.")]
	[SerializeField] 
	float reachDistance = 3f;

	[Tooltip("The wait time between actions updates.")]
	[Range_NoSlider(true)] 
	[SerializeField] 
	float refreshTime = 0.65f;

	
	
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	CharacterActionsInfo characterActions = new CharacterActionsInfo();


	int currentActionIndex = 0;

	float waitTime = 0f;
	float time = 0f;
	
	bool dirty = false;

	CharacterActor characterActor = null;

	
	
	/// <summary>
	/// Gets the current brain mode (AI or Human).
	/// </summary>
	public bool IsAI
	{
		get
		{
			return isAI;
		}
	}

	/// <summary>
	/// Gets the current character action info used by the character.
	/// </summary>
	public CharacterActionsInfo CharacterActions
	{
		get
		{
			return characterActions;
		}
	}


	protected virtual void Awake()
	{
		characterActor = GetComponent<CharacterActor>();

		characterActions.InitializeActions();

		navMeshPath = new NavMeshPath();



		switch( humanInputType )
		{
			case HumanInputType.UnityInputManager:

				inputHandler = gameObject.AddComponent<UnityInputHandler>();
				inputHandler.hideFlags = HideFlags.HideInInspector;

				break;
			case HumanInputType.UI_Mobile:

				inputHandler = gameObject.AddComponent<UIInputHandler>();
				inputHandler.hideFlags = HideFlags.HideInInspector;
				
				
				break;
			case HumanInputType.Custom:

				// --------------------------------------------------------------------------------
				// inputHandler = ** YOUR CUSTOM SOLUTION **;
				// --------------------------------------------------------------------------------
				if( inputHandler == null )
				{
					Debug.Log("If the custom mode is selected an input handler component must be assigned!");
				}
				
				break;
		}


		
		
	}

		
	void OnEnable()
	{
		characterActor.OnWallHit += OnWallHit;
		SceneController.Instance.OnSimulationEnd += OnSimulationEnd;
		
	}

	void OnDisable()
	{
		characterActions.Reset();

		characterActor.OnWallHit -= OnWallHit;
		SceneController.Instance.OnSimulationEnd -= OnSimulationEnd;
	}

	
    void Start()
    {
        SetBrainType( isAI );
    }

	/// <summary>
	/// Sets a custom character action info.
	/// </summary>
	public void SetAction( CharacterActionsInfo characterAction )
	{
		this.characterActions = characterAction;
	}

	/// <summary>
	/// Sets a custom sequence of character actions.
	/// </summary>
	public void SetSequence( CharacterAISequenceBehaviour sequenceBehaviour , bool forceUpdate = true )
	{
		this.sequenceBehaviour = sequenceBehaviour;

		if( forceUpdate )
			time = waitTime + Mathf.Epsilon;
	}

	/// <summary>
	/// Sets the target to follow (only for the follow behaviour).
	/// </summary>
	public void SetFollowTarget( Transform followTarget , bool forceUpdate = true )
	{
		this.followTarget = followTarget;

		if( forceUpdate )
			time = waitTime + Mathf.Epsilon;
	}

	    
	/// <summary>
	/// Sets the type of brain.
	/// </summary>
    public void SetBrainType( bool AI )
    {
		characterActions.Reset();

        if( !AI )
        {
            if( inputData == null )
            {
                Debug.Log( "The input data field is null" );
                return;
            }
            
        }
        else
        {
            SetAIBehaviour( behaviourType );        
        }

        this.isAI = AI;
        
        

    }

	/// <summary>
	/// Sets the AI behaviour type.
	/// </summary>
    public void SetAIBehaviour( AIBehaviourType type )
    {
		characterActions.Reset();

        switch( type )
        {
            case AIBehaviourType.Sequence:

                if( sequenceBehaviour == null )
                {
                    Debug.Log( "Sequence behaviour is null." );
                    return;
                }

				currentActionIndex = 0;
				characterActions = sequenceBehaviour.ActionSequence[currentActionIndex].action;

				if( yAxisToXAxis2D && characterActor.CharacterBody.Is2D )
				{
					if( characterActions.inputAxes.axesValue.x == 0f )
						characterActions.inputAxes.axesValue.x = characterActions.inputAxes.axesValue.y;
					
				}
				
				if( sequenceBehaviour.ActionSequence[currentActionIndex].sequenceType == SequenceType.Duration )
				{							
					waitTime = sequenceBehaviour.ActionSequence[currentActionIndex].duration;
				}
                	

            	break;
           
            case AIBehaviourType.FollowTarget:

				if(followTarget == null)
				{
					Debug.Log( "Follow target behaviour is null." );
					return;
				}                

				waitTime = refreshTime;

            	break;
        }
	
		
        behaviourType = type;	

        time = 0;
    }
	
	
	
	
	void Update()
	{
		float dt = Time.deltaTime;

		if( dirty )
		{
			dirty = false;
			characterActions.Reset();
		}

		UpdateBrain( dt );
	}

	
	void OnSimulationEnd( float dt )
    {		
		if( !IsAI )
			dirty = true;
    }

	public void UpdateBrain( float dt = 0f )
	{

		if( isAI )
            UpdateAIBrain( dt );
		else
            UpdateHumanBrain( dt );
	}

    #region Human

		
	/// <summary>
	/// Updates the character actions by reading the inputs produced by the player.
	/// </summary>
    void UpdateHumanBrain( float dt )
    {

        if( inputData == null || Time.timeScale == 0 )
			return;

		characterActions.inputAxes.Update( inputHandler.GetAxis( inputData.horizontalAxis , useRawAxes ) , inputHandler.GetAxis( inputData.verticalAxis , useRawAxes ) ); 		
		characterActions.run.Update( inputHandler.GetButton( inputData.run ) , inputHandler.GetButtonDown( inputData.run ) , inputHandler.GetButtonUp( inputData.run ) );
		characterActions.jump.Update( inputHandler.GetButton( inputData.jump ) , inputHandler.GetButtonDown( inputData.jump ) , inputHandler.GetButtonUp( inputData.jump ) );
		characterActions.shrink.Update( inputHandler.GetButton( inputData.shrink ) , inputHandler.GetButtonDown( inputData.shrink ) , inputHandler.GetButtonUp( inputData.shrink ) );
		characterActions.dash.Update( inputHandler.GetButton( inputData.dash ) , inputHandler.GetButtonDown( inputData.dash ) , inputHandler.GetButtonUp( inputData.dash ) );
		characterActions.jetPack.Update( inputHandler.GetButton( inputData.jetPack ) , inputHandler.GetButtonDown( inputData.jetPack ) , inputHandler.GetButtonUp( inputData.jetPack ) );
		characterActions.interact.Update( inputHandler.GetButton( inputData.interact ) , inputHandler.GetButtonDown( inputData.interact ) , inputHandler.GetButtonUp( inputData.interact ) );

		characterActions.cameraAxes.Update( inputHandler.GetAxis( inputData.cameraHorizontalAxis , useRawAxes ) , inputHandler.GetAxis( inputData.cameraVerticalAxis , useRawAxes ) );
		characterActions.zoomAxis.Update( inputHandler.GetAxis( inputData.cameraZoomAxis , useRawAxes ) );
		

	}
	


    #endregion


    #region AI
    
	NavMeshPath navMeshPath;

	void OnWallHit( CollisionInfo collisionInfo )
	{
		if( !isAI )
			return;
		
		if( sequenceBehaviour.ActionSequence[currentActionIndex].sequenceType != SequenceType.OnWallHit )
			return;
		
		SelectNextSequenceElement();
		
	}

	
    void UpdateAIBrain( float dt )
	{	
		
		switch( behaviourType )
		{
			case AIBehaviourType.Sequence:

				if( sequenceBehaviour.ActionSequence[currentActionIndex].sequenceType == SequenceType.Duration )
				{
					if( time >= waitTime )
					{
						SelectNextSequenceElement();	
						time = 0f;
					}
					else
					{
						time += dt;
					}	
				}
				

				break;

			case AIBehaviourType.FollowTarget:				

				if( time >= waitTime )
				{
					UpdateFollowTargetBehaviour();	
				}		
				else
				{
					time += dt;
				}			

				break;
		}
		
		
	}
	

	// Sequence Behaviour --------------------------------------------------------------------------------------------------

	void SelectNextSequenceElement()
	{
		if( sequenceBehaviour == null )
			return;

		// Careful here! a reset will erase the original data from the scriptableObjects :(

		if( currentActionIndex == ( sequenceBehaviour.ActionSequence.Count - 1 ) )
			currentActionIndex = 0;
		else
			currentActionIndex++;

		
		characterActions = sequenceBehaviour.ActionSequence[currentActionIndex].action;	

		if( yAxisToXAxis2D && characterActor.CharacterBody.Is2D )
		{
			if( characterActions.inputAxes.axesValue.x == 0f )
				characterActions.inputAxes.axesValue.x = characterActions.inputAxes.axesValue.y;
		}

		waitTime = sequenceBehaviour.ActionSequence[currentActionIndex].duration;


		
		
	}

	[Tooltip("This will copy the y value of the input axes to the x value (nolu for 2D characters). " + 
	"This us useful to reuse some AI sequence behaviours that were defined for 3D using forward and backward direction.")]
	[SerializeField]
	bool yAxisToXAxis2D = true;

	// Follow Behaviour --------------------------------------------------------------------------------------------------

	void UpdateFollowTargetBehaviour()
	{
		if( followTarget == null )
			return;
		
		characterActions.Reset();		

		
		NavMesh.CalculatePath( transform.position , followTarget.position , NavMesh.AllAreas , navMeshPath );

		// ---> Uncomment to see the path drawn in the editor <----
// #if UNTIY_EDITOR
// 		for( int i = 0 ; i < navMeshPath.corners.Length - 1 ; i++ )
// 		{
//             Debug.DrawLine( navMeshPath.corners[i] , navMeshPath.corners[i + 1] , Color.red);
// 		}
// #endif

		if( navMeshPath.corners.Length < 2 )
			return;

		Vector3 path = navMeshPath.corners[1] - navMeshPath.corners[0];

		bool isDirectPath = navMeshPath.corners.Length == 2;
		if( isDirectPath && path.magnitude <= reachDistance )
		{
			return;
		}


		if( navMeshPath.corners.Length > 1 )
		{
			Vector3 inputXZ = Vector3.ProjectOnPlane( navMeshPath.corners[1] - navMeshPath.corners[0] , transform.up ).normalized;

			// from XZ to XY
			Vector3 localInputXZ = transform.InverseTransformVector( inputXZ );

			float x = localInputXZ.x;
			float y = localInputXZ.z;

			Vector2 inputXY = (new Vector2( x , y )).normalized;				

			characterActions.inputAxes.axesValue = inputXY;
		}		
	
	}



    #endregion
}

}
