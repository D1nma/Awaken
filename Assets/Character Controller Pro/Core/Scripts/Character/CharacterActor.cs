using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{


public enum CharacterOrientationMode
{
	FixedDirection ,
	GravityCenter
}

public enum GravityCenterMode
{
	Towards ,
	Away
}


/// <summary>
/// This class represents a character actor. It contains all the character information, collision flags, collision events, and so on. It also responsible for the execution order 
/// of everything related to the character, such as movement, rotation, teleportation, rigidbodies interactions, body size, etc. Since the character can be 2D or 3D, this abstract class must be implemented in the 
/// two formats, one for 2D and one for 3D.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Actor")]
[RequireComponent(typeof( CharacterBody ))]
public class CharacterActor : MonoBehaviour
{  
	
	[Header("Debug")]

	[SerializeField]
	bool showGizmos = true;

	[Header("Tags & Layers")]

	[Tooltip("Important, assign this scriptable object in order to define the layers to work with.")]
	[SerializeField]
	protected CharacterTagsAndLayersProfile tagsAndLayersProfile = null;
	
	[Header("Slopes")]

	[Tooltip("The slope limit set the maximum angle considered as stable.")]
	[SerializeField]
	[Range(1f, 85f)]
	protected float slopeLimit = 60f;

	[Header("Steps")]

	[Tooltip("By Turning this field off the character will not be able to \"properly\" walk over steps, but it will save a few calculations.")]
	[SerializeField]
	bool detectSteps = true;

	
	[Tooltip("The maximum step height the character is capable of climbing.")]
	[Range_NoSlider(true)]
	[SerializeField]
	protected float stepOffset = 0.5f;

	[Header("Grounding")]

	[Tooltip("The distance the character is capable of detecting ground. Use this value to clamp (or not) the character to the ground.")]
	[SerializeField]
	[Range_NoSlider(true)]
	protected float stepDownDistance = 0.5f;

	[Tooltip("This toggle will enable the \"edge compensation\" feature, simulating a cylinder collider when the character is standing on a edge." + 
	"TIP: Use it for 2Dplatformers if you want to imitate a box collider.")]
	[SerializeField]
	bool edgeCompensation = false;

	[Tooltip("Enable this flag if you want to completely ignore the grounded state.")]
	[SerializeField]
	bool alwaysNotGrounded = false;

	[Header("Size")]
	
	[Tooltip("The speed the character is going to change its size. Basically it sets the \"t\" lerp factor.")]
	[SerializeField]
	[Range_NoSlider( true )]
	float sizeChangeLerpSpeed = 10f;

	[Header("Orientation")]

	
	[SerializeField]
	protected CharacterOrientationMode orientationMode = CharacterOrientationMode.FixedDirection;

	[Tooltip("Gravity used for the \"FixedDirection\" orientation mode.")]
    [SerializeField]
    protected Vector3 worldGravityDirection = Vector3.down;

	[Tooltip("Gravity center used for the calculation of the gravity in \"GravityCenter\" orientation mode.")]
    [SerializeField]
    protected Transform gravityCenter = null;

	
	[Tooltip("Should the gravity center attract or repel the character?.")]
	[SerializeField]
    protected GravityCenterMode gravityCenterMode = GravityCenterMode.Towards;
	

	[Header("Dynamic Ground")]

	[Tooltip("Should the character be affected by the movement of the ground (only for kinematic rigidbodies).")]
	[SerializeField]
	protected bool supportDynamicGround = true;
	
	[Tooltip("If this toggle is enabled the forward direction will be affected by the rotation of the dynamic ground (only yaw motion allowed).")]
	[SerializeField]
	bool rotateForwardDirection = true;



	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	CharacterBody characterBody = null;	

	/// <summary>
	/// Gets the CharacterBody component associated with this character actor.
	/// </summary>
	public CharacterBody CharacterBody
	{
		get
		{
			if( characterBody == null)
				characterBody = GetComponent<CharacterBody>();
			
			return characterBody;
		}
	}

	CharacterActorBehaviour characterActorBehaviour = null;	

	/// <summary>
	/// Gets the CharacterActorBehaviour component associated with this character actor.
	/// </summary>
	public CharacterActorBehaviour CharacterActorBehaviour
	{
		get
		{
			if( characterActorBehaviour == null)
				characterActorBehaviour = GetComponent<CharacterActorBehaviour>();
			
			return characterActorBehaviour;
		}
	}

	PhysicsComponent physicsComponent = null;

	/// <summary>
	/// Gets the physics component from the character.
	/// </summary>
	public PhysicsComponent PhysicsComponent
	{
		get
		{
			return physicsComponent;
		}
	}

	#region Collision Properties
	
	/// <summary>
	/// Returns true if the character is standing on an edge.
	/// </summary>
	public bool IsOnEdge
	{
		get
		{
			return characterCollisionInfo.isOnEdge;
		}
	}

	/// <summary>
	/// Gets the grounded state, true if the ground object is not null, false otherwise.
	/// </summary>
	public bool IsGrounded
	{
		get
		{
			return characterCollisionInfo.groundObject != null;
		}
	}

	
	/// <summary>
	/// Gets the angle between the up vector and the stable normal.
	/// </summary>
	public float GroundSlopeAngle
	{
		get
		{
			return characterCollisionInfo.stableSlopeAngle;
		}
	}

	/// <summary>
	/// Gets the contact point obtained directly from the ground test (sphere cast).
	/// </summary>
	public Vector3 GroundContactPoint
	{ 
		get
		{ 
			return characterCollisionInfo.groundContactPoint; 
		} 
	}

	/// <summary>
	/// Gets the normal vector obtained directly from the ground test (sphere cast).
	/// </summary>
	public Vector3 GroundContactNormal
	{ 
		get
		{ 
			return characterCollisionInfo.groundContactNormal; 
		} 
	}

	/// <summary>
	/// Gets the normal vector used to determine stability. This may or may not be the normal obtained from the ground test.
	/// </summary>
	public Vector3 GroundStableNormal
	{ 
		get
		{ 
			return characterCollisionInfo.groundStableNormal; 
		} 
	}

	/// <summary>
	/// Gets the GameObject component of the current ground.
	/// </summary>
	public GameObject GroundObject 
	{ 
		get
		{ 
			return characterCollisionInfo.groundObject; 
		} 
	}

	/// <summary>
	/// Gets the Transform component of the current ground.
	/// </summary>
	public Transform GroundTransform
	{ 
		get
		{ 
			return characterCollisionInfo.groundObject.transform; 
		} 
	}
	
	/// <summary>
	/// Gets the Collider2D component of the current ground.
	/// </summary>
	public Collider2D GroundCollider2D
	{ 
		get
		{ 
			return characterCollisionInfo.groundCollider2D; 
		} 
	}

	/// <summary>
	/// Gets the Collider3D component of the current ground.
	/// </summary>
	public Collider GroundCollider3D
	{ 
		get
		{ 
			return characterCollisionInfo.groundCollider3D; 
		} 
	}

	/// <summary>
	/// Gets the current trigger GameObject.
	/// </summary>
	public GameObject CurrentTrigger
	{ 
		get
		{ 
			if( physicsComponent.Triggers.Count == 0 )
				return null;
			
			return physicsComponent.Triggers[ physicsComponent.Triggers.Count - 1 ]; 
		} 
	}

	/// <summary>
	/// Gets the current trigger GameObject.
	/// </summary>
	public List<GameObject> Triggers
	{ 
		get
		{			
			return physicsComponent.Triggers; 
		} 
	}

	

	/// <summary>
	/// Gets the wall collision flag, true if the character hit a wall, false otherwise.
	/// </summary>
	public bool WallCollision
	{ 
		get
		{ 
			return characterCollisionInfo.wallCollision; 
		} 
	}

	/// <summary>
	/// Gets the angle of the wall. A 90 degrees wall will return a wall angle of 0 degrees.
	/// </summary>
	public float WallAngle
	{ 
		get
		{ 
			return characterCollisionInfo.wallAngle; 
		} 
	}

	/// <summary>
	/// Gets the current collided wall GameObject.
	/// </summary>
	public GameObject WallObject
	{ 
		get
		{ 
			return characterCollisionInfo.wallObject; 
		} 
	}

	/// <summary>
	/// Gets the wall contact point from the collision info.
	/// </summary>
	public Vector3 WallContactPoint
	{
		get
		{
			return characterCollisionInfo.wallContactPoint;
		}
	}

	/// <summary>
	/// Gets the wall contact normal from the collision info.
	/// </summary>
	public Vector3 WallContactNormal
	{
		get
		{
			return characterCollisionInfo.wallContactNormal;
		}
	}

	
	/// <summary>
	/// Gets the current stability state of the character. Stability is equal to "grounded + slope angle <= slope limit".
	/// </summary>
	public bool IsStable
	{
		get
		{
			return IsGrounded && characterCollisionInfo.stableSlopeAngle <= slopeLimit;
		}
	}

	/// <summary>
	/// Returns true if the character is grounded onto an unstable ground, false otherwise.
	/// </summary>
	public bool IsOnUnstableGround
	{
		get
		{
			return IsGrounded && characterCollisionInfo.stableSlopeAngle > slopeLimit;
		}
	}

	bool wasGrounded = false;
	
	/// <summary>
	/// Gets the previous grounded state.
	/// </summary>
    public bool WasGrounded
    {
        get
        {
            return wasGrounded;
        }
    }
	
	
    bool wasStable = false;

	/// <summary>
	/// Gets the previous stability state.
	/// </summary>
    public bool WasStable
    {
        get
        {
            return wasStable;
        }
    }

	

	/// <summary>
	/// Returns true if the collided wall is a Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsWallARigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.wallRigidbody2D != null : characterCollisionInfo.wallRigidbody3D != null;
		}
	}

	/// <summary>
	/// Returns true if the collided wall is a kinematic Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsWallAKinematicRigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.wallRigidbody2D.isKinematic : characterCollisionInfo.wallRigidbody3D.isKinematic;
		}
	}

	/// <summary>
	/// Returns true if the current ground is a Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsGroundARigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.groundRigidbody2D != null : characterCollisionInfo.groundRigidbody3D != null;
		}
	}

	/// <summary>
	/// Returns true if the current ground is a kinematic Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsGroundAKinematicRigidbody
	{
		get
		{
			return characterBody.Is2D ? characterCollisionInfo.groundRigidbody2D.isKinematic : characterCollisionInfo.groundRigidbody3D.isKinematic;
		}
	}

	/// <summary>
	/// Returns true if the current ground is a kinematic Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public Vector3 DynamicGroundPointVelocity
	{
		get
		{			
			if( !dynamicGroundInfo.IsActive )
				return Vector3.zero;
			
			return dynamicGroundInfo.GetPointVelocity( Position );			
			
			//return characterBody.Is2D ? (Vector3)characterCollisionInfo.groundRigidbody2D.velocity : characterCollisionInfo.groundRigidbody3D.velocity;
		}
	}
	
	/// <summary>
	/// Gets a concatenated string containing all the current collision information.
	/// </summary>
	public override string ToString()
	{
		const string nullString = " ---- ";
		
		return string.Concat( 
			"Ground : \n" ,
			"──────────────────\n" ,
			"Is Grounded : " , IsGrounded , '\n' ,
			"Stable Ground : " , IsStable , '\n' ,
			"Slope Angle : " , characterCollisionInfo.stableSlopeAngle , '\n' ,
			"On Edge : " , characterCollisionInfo.isOnEdge , '\n' ,
			"Edge Angle : " , characterCollisionInfo.edgeAngle , '\n' ,
			"Contact Point : " , characterCollisionInfo.groundContactPoint , '\n' ,
			"Contact Normal : " , characterCollisionInfo.groundContactNormal , '\n' ,
			"Object Name : " , characterCollisionInfo.groundObject != null ? characterCollisionInfo.groundObject.name : nullString , '\n' ,
			"Layer : " , LayerMask.LayerToName( characterCollisionInfo.groundLayer ) , '\n' , 	
			"Is a Rigidbody : " , IsGrounded ? IsGroundARigidbody.ToString() : nullString , '\n' ,
			"Is a Kinematic Rigidbody : " , IsGroundARigidbody ? IsGroundAKinematicRigidbody.ToString() : nullString , '\n' ,
			"Dynamic Ground : " , dynamicGroundInfo.IsActive ? dynamicGroundInfo.Transform.name : nullString , "\n\n" ,			
			"Walls : \n" ,
			"──────────────────\n" ,			
			"Wall Collision : " , characterCollisionInfo.wallCollision , '\n' ,	
			"Wall Angle : " , characterCollisionInfo.wallAngle , '\n' ,
			"Is a Rigidbody : " , IsGrounded ? IsWallARigidbody.ToString() : nullString , '\n' ,
			"Is a Kinematic Rigidbody : " , IsWallARigidbody ? IsWallAKinematicRigidbody.ToString() : nullString , '\n' ,
			"──────────────────\n" ,
			"Current trigger : " , CurrentTrigger != null ? CurrentTrigger.name : nullString , '\n' ,
			"Trigger count : " , Triggers.Count , '\n' 			
		);
	}

	#endregion

	protected CharacterCollisionInfo characterCollisionInfo = new CharacterCollisionInfo();

	protected DynamicGroundInfo dynamicGroundInfo = new DynamicGroundInfo();

	Dictionary< Transform , KinematicPlatform > kinematicPlatforms = new Dictionary< Transform , KinematicPlatform >();


	protected Vector2 currentBodySize = Vector2.one;

	Vector2 targetBodySize = Vector2.one;

	/// <summary>
	/// Gets the alwaysNotGrounded flag.
	/// </summary>
	public bool AlwaysNotGrounded
    {
        get
        {
            return alwaysNotGrounded;
        }
		set
        {
            alwaysNotGrounded = value;
        }
    }

	/// <summary>
	/// Gets the initial body size.
	/// </summary>
	public Vector2 DefaultBodySize
	{
		get
		{
			return characterBody.BodySize;
		}
	}

	/// <summary>
	/// Gets the current body size.
	/// </summary>
	public Vector2 BodySize
	{
		get
		{
			return currentBodySize;
		}
	}

	/// <summary>
	/// Gets the LayerMask with all the considered static obstacles.
	/// </summary>
	public LayerMask StaticObstaclesLayerMask
	{
		get
		{
			return tagsAndLayersProfile.staticObstaclesLayerMask;
		}
	}

	/// <summary>
	/// Gets the LayerMask with all the considered dynamic rigidbodies.
	/// </summary>
	public LayerMask DynamicRigidbodiesLayerMask
	{
		get
		{
			return tagsAndLayersProfile.dynamicGroundLayerMask;
		}
	}

	/// <summary>
	/// Gets the LayerMask with all the considered dynamic obstacles.
	/// </summary>
	public LayerMask DynamicGroundLayerMask
	{
		get
		{
			return tagsAndLayersProfile.dynamicGroundLayerMask;
		}
	}

	/// <summary>
	/// Gets the tags and layers profile asset used by the character actor.
	/// </summary>
	public CharacterTagsAndLayersProfile TagsAndLayersProfile
	{
		get
		{
			return tagsAndLayersProfile;
		}
	}
    
    Vector3 inputVelocity = Vector3.zero;
	 

	/// <summary>
	/// Gets the current linear velocity.
	/// </summary>
	public Vector3 InputVelocity
	{
		get
		{
			return inputVelocity;
		}
	}

	/// <summary>
	/// Gets the current local velocity (independent from the orientation).
	/// </summary>
	public Vector3 LocalInputVelocity
	{
		get
		{
			return transform.InverseTransformVector( inputVelocity );
		}
	}
	
	/// <summary>
	/// Sets the character linear velocity.
	/// </summary>
    public void SetInputVelocity( Vector3 inputVelocity )
    {
        this.inputVelocity = inputVelocity;
    }

	/// <summary>
	/// Adds a velocity to the current linear velocity.
	/// </summary>
    public void AddInputVelocity( Vector3 inputVelocity )
    {
        this.inputVelocity += inputVelocity;
    }

	/// <summary>
	/// Gets the applied velocity to the rigidbody (a.k.a the \"real\" linear velocity).
	/// </summary>
	public Vector3 RigidbodyVelocity
	{
		get
		{
			return RigidbodyComponent.Velocity;
		}
	}

	/// <summary>
	/// Gets the applied velocity to the rigidbody (a.k.a the \"real\" linear velocity), without taking into account the dynamic ground velocity.
	/// </summary>
	public Vector3 RigidbodyStaticVelocity
	{
		get
		{
			return rigidbodyStaticVelocity;
		}
	}

	Vector3 rigidbodyStaticVelocity = default( Vector3 );

    
	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	protected Vector3 GetCenter( Vector3 position )
	{
		return position + RigidbodyUp * currentBodySize.y / 2f;
	}

	/// <summary>
	/// Gets the top most point of the collision shape.
	/// </summary>
	protected Vector3 GetTop( Vector3 position )
	{
		return position + RigidbodyUp * ( currentBodySize.y - CharacterConstants.SkinWidth );
	}

	/// <summary>
	/// Gets the bottom most point of the collision shape.
	/// </summary>
	protected Vector3 GetBottom( Vector3 position )
	{
		return position + RigidbodyUp * CharacterConstants.SkinWidth;
	}

	/// <summary>
	/// Gets the center of the top sphere of the collision shape.
	/// </summary>
	protected Vector3 GetTopCenter( Vector3 position )
	{
		return position + RigidbodyUp * ( currentBodySize.y - currentBodySize.x / 2f );
	}

	/// <summary>
	/// Gets the center of the top sphere of the collision shape (considering an arbitrary body size).
	/// </summary>
	protected Vector3 GetTopCenter( Vector3 position , Vector2 bodySize )
	{
		return position + RigidbodyUp * ( bodySize.y - bodySize.x / 2f );
	}

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape.
	/// </summary>
	protected Vector3 GetBottomCenter( Vector3 position )
	{
		return position + RigidbodyUp * currentBodySize.x / 2f;
	}

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape (considering an arbitrary body size).
	/// </summary>
	protected Vector3 GetBottomCenter( Vector3 position , Vector2 bodySize )
	{
		return position + RigidbodyUp * bodySize.x / 2f;
	}

	/// <summary>
	/// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
	/// </summary>
	protected Vector3 GetBottomCenterToTopCenter()
	{
		return RigidbodyUp * ( currentBodySize.y - currentBodySize.x );
	}

	/// <summary>
	/// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
	/// </summary>
	protected Vector3 GetBottomCenterToTopCenter( Vector2 bodySize )
	{
		return RigidbodyUp * ( bodySize.y - bodySize.x );
	}

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape, considering the collider bottom offset.
	/// </summary>
	protected Vector3 GetOffsettedBottomCenter( Vector3 position )
	{
		return position + RigidbodyUp * ( currentBodySize.x / 2f + CharacterConstants.ColliderBottomOffset );
	}
	
	CharacterGraphics characterGraphics = null;

	RigidbodyConstraints initialRigidbodyConstraints;

	void Awake()
	{	
		characterBody = GetComponent<CharacterBody>();

		if( characterBody == null )
			Debug.Log("GameObject " + gameObject.name + " doesn't have a CharacterBody component assigned to it.");

		characterBody.Initialize();


		if( tagsAndLayersProfile == null )
			Debug.Log("GameObject " + gameObject.name + " doesn't have a tags and layers profile assigned to it.");
		
		if( characterBody.Is2D )
			physicsComponent = gameObject.AddComponent<PhysicsComponent2D>();
		else
			physicsComponent = gameObject.AddComponent<PhysicsComponent3D>(); 
		
		currentBodySize = characterBody.BodySize;
		targetBodySize = currentBodySize;	
		

		SetColliderSize();

		
		RigidbodyComponent.Mass = characterBody.Mass;
		RigidbodyComponent.IsKinematic = false;
		RigidbodyComponent.UseGravity = false;
		RigidbodyComponent.UseInterpolation = true;
		RigidbodyComponent.ContinuousCollisionDetection = true;
		RigidbodyComponent.Constraints = RigidbodyConstraints.FreezeRotation;	

		initialRigidbodyConstraints = RigidbodyComponent.Constraints;

	 	characterGraphics = GetComponentInChildren<CharacterGraphics>();

		if( characterGraphics != null )
		{
			SetForwardDirection( characterGraphics.transform.forward );
		}


		// If there is no sceneController in the scene create one.
		if( SceneController.Instance == null )
		{	
			Debug.Log("There is no Scene controller found in the scene. Please add");
		}
		
		
		// Initialize the the character actor behaviour
		characterActorBehaviour = GetComponent<CharacterActorBehaviour>();

		if( characterActorBehaviour == null )
			Debug.Log("GameObject " + gameObject.name + " doesn't have a CharacterActorBehaviour component associated with it.");

		characterActorBehaviour.Initialize( this );
	}

	protected virtual void OnEnable()
	{		
		// Add this actor to the scene controller list
		SceneController.Instance.AddActor( this );

		RigidbodyComponent.Constraints = initialRigidbodyConstraints;

		physicsComponent.OnTriggerEnterEvent += OnTriggerEnterMethod;
		physicsComponent.OnTriggerExitEvent += OnTriggerExitMethod;

		OnStepUp += OnStepUpMethod;
	}

	protected virtual void OnDisable()
	{
		// Remove this actor from the scene controller list
		SceneController.Instance.RemoveActor( this );
		
		RigidbodyComponent.Constraints = RigidbodyConstraints.FreezeAll;

		physicsComponent.OnTriggerEnterEvent -= OnTriggerEnterMethod;
		physicsComponent.OnTriggerExitEvent -= OnTriggerExitMethod;

		OnStepUp -= OnStepUpMethod;
	}

	
	void ResetParameters()
	{
		inputVelocity = Vector3.zero;
	}

	protected virtual void Start()
    {
        	
    }

	/// <summary>
	/// Applies a force at the ground contact point, in the direction of the weight (mass times gravity).
	/// </summary>
	protected virtual void ApplyWeight( Vector3 contactPoint )
    {
		if( characterBody.Is2D )
		{
			if( GroundCollider2D == null )
            return;

			if( GroundCollider2D.attachedRigidbody == null )
				return;

        
        	GroundCollider2D.attachedRigidbody.AddForceAtPosition( - transform.up * characterBody.Mass * CharacterConstants.DefaultGravity , contactPoint );
		}
		else
		{
			if( GroundCollider3D == null )
            return;

			if( GroundCollider3D.attachedRigidbody == null )
				return;

        
        	GroundCollider3D.attachedRigidbody.AddForceAtPosition( - transform.up * characterBody.Mass * CharacterConstants.DefaultGravity , contactPoint );
		}

        
    }

	/// <summary>
	/// Performs the movement and rotation based on the current and previous dynamic ground info.
	/// </summary>
    protected virtual void ProcessDynamicGround( ref Vector3 position , float dt )
    {
		
        if( !dynamicGroundInfo.IsActive )
			return;

		Vector3 initialUp = RigidbodyUp;
			
		Quaternion deltaRotation = dynamicGroundInfo.RigidbodyRotation * Quaternion.Inverse( dynamicGroundInfo.previousRotation );
		

		Vector3 centerToCharacter = position - dynamicGroundInfo.previousPosition;
		Vector3 rotatedCenterToCharacter = deltaRotation * centerToCharacter; 
		
		if( rotateForwardDirection )
		{
			Vector3 forwardDirection = deltaRotation * this.forwardDirection;
			SetForwardDirection( forwardDirection );
		}

		position = dynamicGroundInfo.RigidbodyPosition + rotatedCenterToCharacter;
		
    }

	

	void FindAndUpdateDynamicGround( Transform groundTransform , Vector3 footPosition )
    {
        KinematicPlatform kinematicPlatform;
		bool found = kinematicPlatforms.TryGetValue( groundTransform , out kinematicPlatform );

		if( found )
		{
			dynamicGroundInfo.UpdateTarget( kinematicPlatform , footPosition );
		}
		else
		{
			kinematicPlatform = GroundObject.GetComponent<KinematicPlatform>();

			if( kinematicPlatform != null )
			{
				kinematicPlatforms.Add( groundTransform , kinematicPlatform );
				dynamicGroundInfo.UpdateTarget( kinematicPlatform , footPosition );
			}
			else
			{
				dynamicGroundInfo.Reset();
			}
			
		}
    }    

	/// <summary>
	/// Checks for any dynamic ground. If the result is positive it updates the dynamic ground info.
	/// </summary>
	protected virtual void UpdateDynamicGround( Vector3 position )
	{
		if( !IsGrounded || !CustomUtilities.BelongsToLayerMask(characterCollisionInfo.groundLayer , tagsAndLayersProfile.dynamicGroundLayerMask ))
		{
                dynamicGroundInfo.Reset();
			return;
		}

		if( !IsGroundARigidbody )
		{
			dynamicGroundInfo.Reset();
			return;	
		}
		else if( !IsGroundAKinematicRigidbody )
		{
			dynamicGroundInfo.Reset();
			return;	
		}
							
		FindAndUpdateDynamicGround( GroundTransform , position );
	
	}


	/// <summary>
	/// Sets the gravity direction (FixedDirection).
	/// </summary>
	public void SetWorldGravityDirection( Vector3 gravityDirection )
    {
        gravityDirection.Normalize();
        this.worldGravityDirection = gravityDirection;
    }

	/// <summary>
	/// Sets the gravity mode.
	/// </summary>
	public void SetGravityMode( CharacterOrientationMode gravityMode )
	{
		this.orientationMode = gravityMode;
	}

	Vector3 currentGravityDirection = Vector3.down;

	/// <summary>
	/// Gets the current world gravity direction.
	/// </summary>
	public Vector3 CurrentGravityDirection
	{
		get
		{
			return currentGravityDirection;
		}
	}



	/// <summary>
	/// Sets a new gravity center.
	/// </summary>
    public void SetGravityCenter( Transform gravityCenter , GravityCenterMode gravityCenterMode = GravityCenterMode.Towards )
    {
        this.gravityCenter = gravityCenter;
		this.gravityCenterMode = gravityCenterMode;    
    }

	void SetColliderSize()
    {
        float verticalOffset = IsGrounded ? CharacterConstants.ColliderBottomOffset : 0f;
        float radius = currentBodySize.x / 2f;
		float height = currentBodySize.y - verticalOffset;

        ColliderComponent.Size = new Vector2( 2f * radius , height );
		ColliderComponent.Offset = Vector2.up * ( verticalOffset + height / 2f );
    }

	void RotateCharacter( Vector3 up )
    {
		if( characterBody.Is2D )
		{	
			up.z = 0;         

			float angle = RigidbodyComponent.Rotation.eulerAngles.z;
        	angle += Vector2.SignedAngle( RigidbodyUp , up );

			RigidbodyComponent.Rotation = Quaternion.Euler( 
				RigidbodyComponent.Rotation.eulerAngles.x ,
				RigidbodyComponent.Rotation.eulerAngles.y , 
				angle
			);
		}
		else
		{
			Quaternion deltaRotation = Quaternion.FromToRotation( RigidbodyUp , up );  
        	RigidbodyComponent.Rotation = deltaRotation * RigidbodyComponent.Rotation;
		}
		
    }
	
	/// <summary>
	/// Gets the current gravity center.
	/// </summary>
	public Transform GravityCenter
	{
		get
		{
			return gravityCenter;
		}
	}


	/// <summary>
    /// Gets the current rigidbody position.
    /// </summary>
	public Vector3 Position
	{
		get
		{
			return RigidbodyComponent.Position;
		}
		set
		{
			RigidbodyComponent.Position = value;
		}
	}

	/// <summary>
    /// Gets the current rigidbody rotation.
    /// </summary>
	public Quaternion Rotation
	{
		get
		{
			return RigidbodyComponent.Rotation;
		}
		set
		{
			RigidbodyComponent.Rotation = value;
		}
	}
	
    /// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.up).
    /// </summary>
	public Vector3 RigidbodyUp
	{
		get
		{
			return RigidbodyComponent.Up;
		}
	}

	/// <summary>
    /// Gets the current forward direction based on the rigidbody rotation (not necessarily transform.forward). Note that Forward does not means forwardDirection.
    /// </summary>
	public Vector3 RigidbodyForward
	{
		get
		{
			return RigidbodyComponent.Rotation * Vector3.forward;
		}
	}

	/// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right).
    /// </summary>
	public Vector3 RigidbodyRight
	{
		get
		{
			return RigidbodyComponent.Rotation * Vector3.right;
		}
	}

	/// <summary>
    /// Gets the RigidbodyComponent component associated with the character.
    /// </summary>
	public RigidbodyComponent RigidbodyComponent
	{
		get
		{
			return characterBody.RigidbodyComponent;
		}
	}



	/// <summary>
    /// Gets the ColliderComponent component associated with the character.
    /// </summary>
	public ColliderComponent ColliderComponent
	{
		get
		{
			return characterBody.ColliderComponent;
		}
	}

	/// <summary>
    /// Gets a list with all the current contacts.
    /// </summary>
	public List<Contact> Contacts
	{
		get
		{
			if( physicsComponent == null)
				return null;
			
			return physicsComponent.contactsList;
		}
	}

	/// <summary>
    /// Gets a list with all the valid contacts for collision response. These are basically contacts that come from OnCollisionEnter messages, due to dynamic rigidbodies that use the "Contact Rigidbodies" tag.
    /// </summary>
	public List<Contact> CollisionResponseContacts
	{
		get
		{
			if( physicsComponent == null)
				return null;
			
			return collisionResponseContacts;
		}
	}
	
	List<Contact> collisionResponseContacts = new List<Contact>();

	void HandleRotation( float dt)
    {		
		// Calculate the gravity direction
		if( orientationMode == CharacterOrientationMode.FixedDirection )
		{
			currentGravityDirection = worldGravityDirection;
		}
		else if( orientationMode == CharacterOrientationMode.GravityCenter )
		{
			float gravityModeMultiplier = gravityCenterMode == GravityCenterMode.Towards ? 1f : -1f;
			 

			if( characterBody.Is2D )
			{
				currentGravityDirection = gravityModeMultiplier * ( gravityCenter.position - RigidbodyComponent.Position ); 
				currentGravityDirection.z = 0f;
				currentGravityDirection.Normalize();
			}
			else
			{
				currentGravityDirection = ( gravityModeMultiplier * ( gravityCenter.position - RigidbodyComponent.Position ) ).normalized; 
			}
			
		}        

		// Define the character up vector
		Vector3 up = - currentGravityDirection;   

		// Get the delta rotation between the previous up and the current one
		Quaternion deltaRotation = Quaternion.FromToRotation( RigidbodyUp , up );    

		// Calculate the forward direction vector
		forwardDirection = deltaRotation * forwardDirection;

		RotateCharacter( up );
    }

	Vector3 forwardDirection = default( Vector3 );


	/// <summary>
	/// Gets the forward direction vector. This vector is configurable and can be any.
	/// </summary>
    public Vector3 UpDirection
    {
        get
        {
            return RigidbodyUp;
        }
    }
	
	/// <summary>
	/// Gets the forward direction vector. This vector is configurable and can be any.
	/// </summary>
    public Vector3 ForwardDirection
    {
        get
        {
            return forwardDirection;
        }
    }

	/// <summary>
	/// Gets the forward direction vector. This vector is configurable and can be any.
	/// </summary>
    public Vector3 RightDirection
    {
        get
        {
            return Vector3.Cross( UpDirection , ForwardDirection );
        }
    }

	/// <summary>
	/// Sets the forward direction vector. This vector can be use to make the graphics component to look towards a certain direction (only yaw motion is accepted). 
	/// </summary>
    public void SetForwardDirection( Vector3 forwardDirection )
    {
		
		if( forwardDirection == Vector3.zero )
			return;
		
		Vector3 projectLookingDirection = Vector3.ProjectOnPlane( forwardDirection , RigidbodyUp ).normalized;

		if( projectLookingDirection == Vector3.zero )
			return;

        this.forwardDirection = projectLookingDirection;
    }
	
	void GetNewestContacts()
	{
		collisionResponseContacts.Clear();

		for( int i = 0 ; i < Contacts.Count ; i++ )
        {
            Contact contact = Contacts[i];

            if( !contact.firstContact )
                continue;

            if( !contact.isRigidbody )
                continue;

            if( contact.isKinematicRigidbody )
                continue;
            
            if( !contact.gameObject.CompareTag( tagsAndLayersProfile.contactRigidbodiesTag ) )
                continue;

            collisionResponseContacts.Add( contact );     
        }
	}

	/// <summary>
	/// Updates the character size, position and rotation.
	/// </summary>
	public void UpdateCharacter( float dt )
	{		
		GetNewestContacts();

		characterActorBehaviour.UpdateBehaviour( dt );

		wasGrounded = IsGrounded;
        wasStable = IsStable;

		HandleTeleportation();

		Vector3 initialPosition = RigidbodyComponent.Position;
		Vector3 position = initialPosition;
		
		HandleSize( position , dt );
		
		HandleRotation( dt );
		HandlePosition( ref position , ref initialPosition , inputVelocity * dt , dt );		
		
		
		
		physicsComponent.ClearContacts();
	}

	
	void HandleSize( Vector3 position , float dt )
	{		    
        bool isValid = CheckTargetBodySize( position );

        if( !isValid )
        {
            return; 
        }

        currentBodySize = Vector2.Lerp( currentBodySize , targetBodySize , sizeChangeLerpSpeed * dt );     

		SetColliderSize();
	}
	

	public void IgnoreLayerMask( bool ignore , LayerMask layerMask )
    {
        int characterLayer = gameObject.layer;
        int layerMaskValue = layerMask.value;
        int currentLayer = 1;

		for( int i = 0 ; i < 32 ; i++ )
		{
			bool exist = ( layerMaskValue & currentLayer ) > 0;

            if( exist )
                IgnoreLayer( i , ignore );

            currentLayer <<= 1;
		}
        
    }

	public void IgnoreLayer( int ignoredLayer , bool ignore )
    {
		physicsComponent.IgnoreLayerCollision( gameObject.layer , ignoredLayer , ignore );
    }
	
	#region Events

		
	void OnTriggerEnterMethod( GameObject trigger )
	{
		if( OnTriggerEnter != null )
			OnTriggerEnter();
	}

	void OnTriggerExitMethod( GameObject trigger )
	{
		if( OnTriggerExit != null )
			OnTriggerExit();
	}


	/// <summary>
	/// This event is called when the character enters a trigger.
	/// </summary>
	public event System.Action OnTriggerEnter;

	/// <summary>
	/// This event is called when the character exits a trigger.
	/// </summary>
	public event System.Action OnTriggerExit;

	/// <summary>
	/// This event is called when the character hits its head (not grounded).
	/// 
	/// The related collision information struct is passed as an argument.
	/// </summary>
	public event System.Action<CollisionInfo> OnHeadHit;

	/// <summary>
	/// This event is called everytime the character is blocked by an unallowed geometry, this could be
	/// a wall or a steep slope (depending on the "slopeLimit" value).
	/// 
	/// The related collision information struct is passed as an argument.
	/// </summary>
	public event System.Action<CollisionInfo> OnWallHit;

	/// <summary>
	/// This event is called everytime the character teleports.
	/// 
	/// The teleported position and rotation are passed as arguments.
	/// </summary>
	public event System.Action<Vector3,Quaternion> OnTeleport;

	/// <summary>
	/// This event is called everytime the character step up successfully.
	/// 
	/// The final position and the step height are passed as arguments.
	/// </summary>
	public event System.Action<Vector3,float> OnStepUp;	

	/// <summary>
	/// This event is called when the character enters the grounded state.
	/// 
	/// The local linear velocity is passed as an argument.
	/// </summary>
	public event System.Action<Vector3> OnGroundedStateEnter;

	/// <summary>
	/// This event is called when the character exits the grounded state.
	/// </summary>
	public event System.Action OnGroundedStateExit;
	

	#endregion

	bool teleportFlag = false;
	
	Vector3 teleportPosition = default(Vector3);
	Quaternion teleportRotation = default(Quaternion);

	/// <summary>
	/// Sets the teleportation position and rotation using an external Transform reference. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Transform reference )
	{
		teleportPosition = reference.position;
		teleportRotation = reference.rotation;

		teleportFlag = true;
	}	

	/// <summary>
	/// Sets the teleportation position and rotation. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Vector3 position , Quaternion rotation )
	{
		teleportPosition = position;
		teleportRotation = rotation;

		teleportFlag = true;
	}	

	void HandleTeleportation()
	{
		if( !teleportFlag )
			return;

		
		RigidbodyComponent.Position = teleportPosition;
		RigidbodyComponent.Rotation = teleportRotation;

		SetForwardDirection( teleportRotation * Vector3.forward );

		if( OnTeleport != null )
			OnTeleport( teleportPosition , teleportRotation );

		teleportFlag = false;
	}

	Vector3 dynamicGroundDisplacement;

	bool stepUpPhases = false;

	void HandlePosition( ref Vector3 position , ref Vector3 initialPosition , Vector3 displacement , float dt )
	{
		
		// Phase 2 --------------------------------------------------------------------
		// Phase 1 was already performed
		if( stepUpPhases )
		{
			stepUpPhases = false;

			// Define the target position (phase 2)
			targetPosition = targetStepUpPosition;

			Vector3 stepUpDisplacement = targetPosition - initialPosition;

			// Set the vertical step up velocity (phase 2)
			RigidbodyComponent.Velocity = stepUpDisplacement / dt;
			return;
		}	


		if( alwaysNotGrounded )
			characterCollisionInfo.Reset();

		if( forceNotGroundedFlag )
		{
			ForceNotGroundedInternal();
			forceNotGroundedFlag = false;
		}

		bool stepUpResult = false;

		if( IsGrounded )
		{	
			Vector3 oldInitialPosition = initialPosition;

			if( supportDynamicGround )
				ProcessDynamicGround( ref position , dt );

			dynamicGroundDisplacement = position - initialPosition;
			
			GroundedMovement( ref position , ref initialPosition , displacement , ref stepUpResult );
		}
		else
		{		
			NotGroundedMovement( ref position , ref initialPosition , displacement );
		}

		

		if( supportDynamicGround )
			UpdateDynamicGround( position );
		


		
		// Phase 1 --------------------------------------------------------------------
		if( stepUpPhases )
		{

			targetStepUpPosition = position;

			Vector3 totalDisplacement = targetStepUpPosition - initialPosition;
			
			Vector3 verticalStepUpDisplacement = Vector3.Project( totalDisplacement , RigidbodyUp );

			// Define the target position (phase 1)
			targetPosition = Position + verticalStepUpDisplacement;

			// Set the vertical step up velocity (phase 1)
			RigidbodyComponent.Velocity = verticalStepUpDisplacement / dt;

			rigidbodyStaticVelocity = ( totalDisplacement - dynamicGroundDisplacement ) / dt;
			
		}
		else
		{	
			targetPosition = position;

			Vector3 totalDisplacement = targetPosition - initialPosition;

			RigidbodyComponent.Velocity = totalDisplacement / dt;

			rigidbodyStaticVelocity = ( totalDisplacement - dynamicGroundDisplacement ) / dt;		

		}

		
	}

	void OnStepUpMethod( Vector3 position , float stepUpHeight )
	{
		
		if( stepUpHeight <= characterBody.BodySize.x / 2f )
			return;
		
		stepUpPhases = true;
	}

	Vector3 targetStepUpPosition = default( Vector3 );

	Vector3 targetPosition = default( Vector3 );

	/// <summary>
	/// Gets the approximated position the character will be in after the physics simulation.
	/// </summary>
	public Vector3 TargetPosition
	{
		get
		{
			return targetPosition;
		}
	}
	
	

	void GroundedMovement( ref Vector3 position , ref Vector3 initialPosition , Vector3 displacement , ref bool stepUpResult )
	{
				
		ApplyWeight( GroundContactPoint );
		
		displacement = CustomUtilities.ProjectVectorOnPlane( 
			displacement ,
            GroundStableNormal,
            Vector3.Cross( displacement , RigidbodyUp).normalized 
		);

		CollideAndSlide( ref position , ref initialPosition , displacement , GroundStableNormal , ref stepUpResult );
		
		
		ProbeGround( ref position , true );
			
	}

	
	void NotGroundedMovement( ref Vector3 position , ref Vector3 initialPosition , Vector3 displacement )
	{

        Vector3 planarDisplacement = CustomUtilities.RemoveComponent( displacement , RigidbodyUp);
		Vector3 verticalDisplacement = Vector3.Project( displacement , RigidbodyUp );
		
		if( verticalDisplacement.magnitude > CharacterConstants.MinMovementAmount )
			NotGroundedVerticalMovement( ref position , verticalDisplacement );

		if( planarDisplacement.magnitude > CharacterConstants.MinMovementAmount )
			NotGroundedPlanarMovement( ref position , ref initialPosition , planarDisplacement );

		
		
	}

	void NotGroundedPlanarMovement( ref Vector3 position , ref Vector3 initialPosition , Vector3 planarDisplacement )
	{
		CollideAndSlide( ref position , ref initialPosition , planarDisplacement , RigidbodyUp );

	}


	void NotGroundedVerticalMovement( ref Vector3 position , Vector3 verticalDisplacement )
	{
		float localVerticalComponent = transform.InverseTransformDirection( verticalDisplacement ).y;
		bool positiveVerticalMovement = localVerticalComponent > 0;
		
		LayerMask groundLayerMask = tagsAndLayersProfile.staticObstaclesLayerMask | tagsAndLayersProfile.dynamicRigidbodiesLayerMask;

		float verticalDelta = Mathf.Sign( localVerticalComponent ) * verticalDisplacement.magnitude;

		CollisionInfo collisionInfo;
		bool hit = CastBodyVertically(
			out collisionInfo ,
			position ,
			verticalDelta ,
			groundLayerMask
		);		
		
		
		if( positiveVerticalMovement )
		{			
			// "verticalDisplacement" for more natural movement
			position += verticalDisplacement;

			if( hit )
			{
				if( OnHeadHit != null )
					OnHeadHit( collisionInfo );
			}
			// else
			// {
			// 	bool groundHit = CheckOverlapWithLayerMask( position , staticObstaclesLayerMask );
			
			// 	if( groundHit )
			// 		SetGroundCollisionInfo( collisionInfo );
			// }

			

		}
		else
		{
			// "collisionInfo.displacement" necessary for ground detection.
			position += collisionInfo.displacement;

			if( hit && !alwaysNotGrounded )
			{	
				SetGroundCollisionInfo( collisionInfo );

				if( OnGroundedStateEnter != null )
					OnGroundedStateEnter( LocalInputVelocity );	
			}			
						
				
		}

		
	}

	
	void SetWallCollisionInfo( CollisionInfo collisionInfo )
	{
		bool wasCollidingWithWall = characterCollisionInfo.wallCollision;
	

		if( collisionInfo.collision )
		{			
			characterCollisionInfo.wallCollision = collisionInfo.contactSlopeAngle > slopeLimit;	
			characterCollisionInfo.wallContactNormal = collisionInfo.hitInfo.normal;
			characterCollisionInfo.wallContactPoint = collisionInfo.hitInfo.point;
			characterCollisionInfo.wallObject = collisionInfo.hitInfo.transform.gameObject;
			characterCollisionInfo.wallAngle = Vector3.Angle( - collisionInfo.hitInfo.direction , collisionInfo.hitInfo.normal );
			characterCollisionInfo.wallCollider2D = collisionInfo.hitInfo.collider2D;
			characterCollisionInfo.wallCollider3D = collisionInfo.hitInfo.collider3D;
			characterCollisionInfo.wallRigidbody2D = collisionInfo.hitInfo.rigidbody2D;
			characterCollisionInfo.wallRigidbody3D = collisionInfo.hitInfo.rigidbody3D;

		}
		else
		{
			characterCollisionInfo.ResetWallInfo();
		}
			

		
		

		if( characterCollisionInfo.wallCollision && !wasCollidingWithWall )
		{
			if( OnWallHit != null )
				OnWallHit( collisionInfo );
		}

	}

	void SetGroundCollisionInfo( CollisionInfo collisionInfo )
	{
		characterCollisionInfo.ResetGroundInfo();

		characterCollisionInfo.isOnEdge = collisionInfo.isAnEdge;

		float contactSlopeAngle =  Vector3.Angle( RigidbodyUp , collisionInfo.hitInfo.normal );
		characterCollisionInfo.groundContactNormal = contactSlopeAngle < 90f ? collisionInfo.hitInfo.normal : RigidbodyUp;
		characterCollisionInfo.groundContactPoint = collisionInfo.hitInfo.point;

		//Normal 
		if( characterCollisionInfo.isOnEdge )
		{
			characterCollisionInfo.edgeAngle = collisionInfo.edgeAngle;
			if( collisionInfo.edgeUpperSlopeAngle <= slopeLimit )
			{
				characterCollisionInfo.groundStableNormal = collisionInfo.edgeUpperNormal;
			}
			else if( collisionInfo.edgeLowerSlopeAngle <= slopeLimit )
			{
				characterCollisionInfo.groundStableNormal = collisionInfo.edgeLowerNormal;
			}
			else
			{
				characterCollisionInfo.groundStableNormal = collisionInfo.hitInfo.normal;
			}

		}
		else
		{	
			characterCollisionInfo.groundStableNormal = collisionInfo.hitInfo.normal;
		}

		characterCollisionInfo.stableSlopeAngle = Vector3.Angle( RigidbodyUp , characterCollisionInfo.groundStableNormal );		
		characterCollisionInfo.groundObject = collisionInfo.hitInfo.transform.gameObject;

		if( characterCollisionInfo.groundObject != null )
		{
			characterCollisionInfo.groundLayer = characterCollisionInfo.groundObject.layer;
			characterCollisionInfo.groundCollider2D = collisionInfo.hitInfo.collider2D; 
			characterCollisionInfo.groundCollider3D = collisionInfo.hitInfo.collider3D; 

			characterCollisionInfo.groundRigidbody2D = collisionInfo.hitInfo.rigidbody2D;
			characterCollisionInfo.groundRigidbody3D = collisionInfo.hitInfo.rigidbody3D;
		}

	}


	bool CheckForGround( out CollisionInfo collisionInfo , Vector3 footPosition , bool grounded , LayerMask layerMask )
    {        
        collisionInfo = new CollisionInfo();

        Vector3 origin = GetOffsettedBottomCenter( footPosition );
		float radius = BodySize.x / 2f - CharacterConstants.SkinWidth;
        float skin = CharacterConstants.ColliderBottomOffset + CharacterConstants.SkinWidth;
        float extraDistance = grounded ? Mathf.Max( CharacterConstants.GroundCheckDistance , stepDownDistance ) : CharacterConstants.GroundCheckDistance;
        Vector3 castDisplacement = - RigidbodyUp * ( skin + extraDistance );
        
		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast(
			out hitInfo ,
			origin ,
			radius ,
			castDisplacement ,
			layerMask
		);

		UpdateCollisionInfo( out collisionInfo , hitInfo , castDisplacement , skin , layerMask );     

        return collisionInfo.collision;
    }
	
	protected bool CheckForStableGround( out CollisionInfo collisionInfo , Vector3 footPosition , Vector3 direction , LayerMask layerMask )
    {
        collisionInfo = new CollisionInfo();

        float skin = CharacterConstants.SkinWidth;
		float radius = BodySize.x / 2f - CharacterConstants.SkinWidth;
        Vector3 origin = GetBottomCenter( footPosition );
        Vector3 castDisplacement = direction * ( CharacterConstants.GroundCheckDistance + skin );

		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast( 
			out hitInfo ,
			origin ,
			radius ,
			castDisplacement ,
			layerMask
		);


		UpdateCollisionInfo( out collisionInfo , hitInfo , castDisplacement , skin , layerMask );      

        return collisionInfo.collision;
    }
	
	protected bool CastBody( out CollisionInfo collisionInfo , Vector3 footPosition , Vector3 displacement , bool grounded , LayerMask layerMask )
    {
        collisionInfo = new CollisionInfo();
		
        float skin = CharacterConstants.SkinWidth;

        Vector3 bottom = GetBottomCenter( footPosition );   
        Vector3 top = GetTopCenter( footPosition );      
		float radius = currentBodySize.x / 2f - CharacterConstants.SkinWidth;   

        Vector3 castDisplacement = displacement + displacement.normalized * skin;

		HitInfo hitInfo;
		int hits = 0;

		if( characterBody.BodyType == CharacterBodyType.Sphere )
		{
			hits = physicsComponent.SphereCast(
				out hitInfo ,
				GetCenter( footPosition ) ,
				radius ,
				castDisplacement ,
				layerMask
			);
		}
		else
		{
			hits = physicsComponent.CapsuleCast(
				out hitInfo ,
				bottom ,
				top ,
				radius ,
				castDisplacement ,
				layerMask
			);
		}


		UpdateCollisionInfo( out collisionInfo , hitInfo , castDisplacement , skin , layerMask );     

        return collisionInfo.collision;
    }


	protected bool CastBodyVertically( out CollisionInfo collisionInfo , Vector3 footPosition , float verticalComponent , LayerMask layerMask )
    {		
        collisionInfo = new CollisionInfo();

		float backstepDistance = currentBodySize.y / 2f;
		float skin = backstepDistance + CharacterConstants.SkinWidth;

		Vector3 castDirection = verticalComponent > 0 ? RigidbodyUp : - RigidbodyUp;

        Vector3 center = verticalComponent > 0 ? 
		GetTopCenter( footPosition ) - castDirection * skin :
		GetBottomCenter( footPosition ) - castDirection * skin;

        Vector3 castDisplacement = castDirection * ( Mathf.Abs( verticalComponent ) + skin );

		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast(
			out hitInfo ,
			center ,
			BodySize.x / 2f - CharacterConstants.SkinWidth ,
			castDisplacement ,
			layerMask
		);
		
		UpdateCollisionInfo( out collisionInfo , hitInfo , castDisplacement , skin , layerMask );
		
        return collisionInfo.collision;
    }

	/// <summary>
	/// Checks if the character is currently overlapping with any obstacle from a given layermask.
	/// </summary>
	public bool CheckOverlapWithLayerMask( Vector3 footPosition , LayerMask layerMask )
	{
		Vector3 bottom = GetBottomCenter( footPosition );   
        Vector3 top = GetTopCenter( footPosition );      
		float radius = currentBodySize.x / 2f - CharacterConstants.SkinWidth;  
		
		bool overlap = physicsComponent.OverlapCapsule(
			bottom ,
			top ,
			radius ,
			layerMask
		);		
		
		return overlap;
	}

	bool CheckTargetBodySize( Vector3 position )
    {        
        Vector3 bottom = GetBottomCenter( position , targetBodySize );   
        Vector3 top = GetTopCenter( position , targetBodySize ); 
		float radius = targetBodySize.x / 2f - CharacterConstants.SkinWidth;

		// GetBottomCenterToTopCenter.normalized ---> Up

        Vector3 castDisplacement = GetBottomCenterToTopCenter( targetBodySize ) + RigidbodyUp * CharacterConstants.SkinWidth;

		HitInfo hitInfo;
		physicsComponent.SphereCast( 
			out hitInfo ,
			bottom ,
			radius ,
			castDisplacement ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);


		bool overlap = hitInfo.hit;
		
		return !overlap;
    }

	

	void ProbeGround( ref Vector3 position , bool grounded )
	{		
		Vector3 initialFootPosition = position;

		float groundCheckDistance = edgeCompensation ? 
		currentBodySize.x / 2f + CharacterConstants.GroundCheckDistance :
		CharacterConstants.GroundCheckDistance;

		float groundProbingDistance = IsStable ? 
		Mathf.Max( groundCheckDistance , stepDownDistance ) : 
		CharacterConstants.GroundCheckDistance;

		Vector3 displacement = - RigidbodyUp * groundProbingDistance;
		
		LayerMask groundLayerMask = tagsAndLayersProfile.staticObstaclesLayerMask | tagsAndLayersProfile.dynamicRigidbodiesLayerMask;
		
		CollisionInfo firstCollisionInfo;
		bool hit = CheckForGround( 
			out firstCollisionInfo ,
			position , 
			grounded , 
			groundLayerMask 
		);


		if( !hit )
		{			
			ForceNotGroundedInternal();
			return;
		}


		float slopeAngle = IsAStableEdge( firstCollisionInfo ) ? 
		Vector3.Angle( RigidbodyUp , firstCollisionInfo.edgeUpperNormal ) : 
		Vector3.Angle( RigidbodyUp , firstCollisionInfo.hitInfo.normal );
			
		if( slopeAngle <= slopeLimit)
		{			
			position += firstCollisionInfo.displacement;	
			SetGroundCollisionInfo( firstCollisionInfo );
			
		}
		else
		{
			// It Hit ustable ground -> Keep checking

			// Backup the collision data
			CollisionInfo unstableGroundCollisionInfo = new CollisionInfo();
			unstableGroundCollisionInfo = firstCollisionInfo;

			position += unstableGroundCollisionInfo.displacement;

			Vector3 downwardsDirection = Vector3.ProjectOnPlane( - RigidbodyUp , unstableGroundCollisionInfo.hitInfo.normal ).normalized;
			

			CollisionInfo secondCollisionInfo;
			hit = CheckForStableGround( 
				out secondCollisionInfo , 
				position , 
				downwardsDirection , 
				groundLayerMask 
			);

			if( hit )
			{
				
				slopeAngle = Vector3.Angle( RigidbodyUp , secondCollisionInfo.hitInfo.normal );

				if( slopeAngle <= slopeLimit )
				{
					// It Hit a stable slope -> Set Info
					position += secondCollisionInfo.displacement;
					SetGroundCollisionInfo( secondCollisionInfo );			
					
				}
				else
				{
					SetGroundCollisionInfo( unstableGroundCollisionInfo );							
				}
				
			}
			else
			{
				SetGroundCollisionInfo( unstableGroundCollisionInfo );
			}
		}

		EdgeCompensation( ref position );
			
	}

	void EdgeCompensation( ref Vector3 position )
	{
		if( !edgeCompensation )
			return;
		
		if( IsOnEdge && IsStable )
		{
			Vector3 compensation = Vector3.Project( ( GroundContactPoint - position ) , RigidbodyUp );

			position += compensation;
		}
		
	}

	
	
	

	/// <summary>
	/// Sets the character body size. The validation of this size value will be evaluated internally.
	/// If the result is negative the body size will not be changed.
	/// </summary>
	public void SetTargetBodySize( Vector2 targetBodySize )
	{
        this.targetBodySize = targetBodySize;
	}

	
	void ForceNotGroundedInternal()
	{			
		characterCollisionInfo.Reset();

		if( OnGroundedStateExit != null )
			OnGroundedStateExit();
	}

	/// <summary>
	/// Set the "force not grounded" internal flag. The character will use this flag to abandon the
	/// grounded state (isGrounded = false). 
	/// 
	/// For example, this is useful when making the character jump.
	/// </summary>
	public void ForceNotGrounded()
	{
		forceNotGroundedFlag = true;	
	}

	bool forceNotGroundedFlag = false;

	bool IsAStableEdge( CollisionInfo collisionInfo )
	{
		return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle <= slopeLimit;
	}

	bool IsAnUnstableEdge( CollisionInfo collisionInfo )
	{
		return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle > slopeLimit;
	}	

	
	bool IsValidForStepUp( CollisionInfo collisionInfo )
	{
		if( CustomUtilities.BelongsToLayerMask( collisionInfo.hitInfo.transform.gameObject.layer , tagsAndLayersProfile.dynamicRigidbodiesLayerMask ) )
			return false;
		
		if( IsAStableEdge( collisionInfo ) )
			return true;		
		
		if( CustomUtilities.isBetween( collisionInfo.contactSlopeAngle , CharacterConstants.MinStepAngle , CharacterConstants.MaxStepAngle , true ) )
			return true;
		

		return false;
	}

	protected virtual void CollideAndSlide( ref Vector3 position , ref Vector3 initialPosition , Vector3 displacement , Vector3 groundPlaneNormal , ref bool stepUpResult )
	{
		Vector3 slidingPlaneNormal = Vector3.zero;

		Vector3 initialFootPosition = position;            

		int iteration = 0;

		while( displacement.magnitude > CharacterConstants.MinMovementAmount && iteration < CharacterConstants.MaxSlideIterations )
        {
			iteration++;
			
			Vector3 previousFootPosition = position;


			CollisionInfo collisionInfo;
			bool hit = CastBody(
				out collisionInfo ,
				position ,
				displacement ,
				true ,
				tagsAndLayersProfile.staticObstaclesLayerMask
			);
			
			
			if( !hit )
			{
				position += displacement;

				if( iteration == 1 )
					SetWallCollisionInfo( collisionInfo );
				
				break;
			}

			// Save the collision info
			CollisionInfo initialCollisionInfo = collisionInfo;

			position += collisionInfo.displacement;
			displacement -= collisionInfo.displacement;    


			// Step handling ----------------------------------------------------------------------------------------------------------
			if( detectSteps )
			{					
				if( IsValidForStepUp( collisionInfo ) )
				{
					Vector3 preStepUpPosition = position;

					CollisionInfo stepUpResultInfo;

					
					stepUpResult = StepUp( ref position , ref displacement , out stepUpResultInfo );
					

					if( stepUpResult )
					{			
						Vector3 deltaY = Vector3.Project( position - preStepUpPosition , RigidbodyUp );
						//Vector3 stepUpHeight = Vector3.Project( position - preStepUpPosition , Up );

						if( OnStepUp != null )
							OnStepUp( position , deltaY.magnitude );


						displacement = CustomUtilities.ProjectVectorOnPlane( 
							displacement , 
							stepUpResultInfo.hitInfo.normal ,
							Vector3.Cross( displacement , RigidbodyUp).normalized 
						);

						groundPlaneNormal = stepUpResultInfo.hitInfo.normal;
						slidingPlaneNormal = Vector3.zero;
						continue;
					}
				}
			}

			

			// Slide ------------------------------------------------------------------------------------------------------------------
			bool blocked = UpdateSlidingPlanes( 
				iteration , 
				false , 
				initialCollisionInfo , 
				ref slidingPlaneNormal , 
				ref groundPlaneNormal , 
				ref displacement 
			);

			if( iteration == 1 )
			{
				SetWallCollisionInfo( initialCollisionInfo );				
			}
           	
			
		}

	}

	protected virtual void CollideAndSlide( ref Vector3 position , ref Vector3 initialPosition , Vector3 displacement , Vector3 groundPlaneNormal )
	{

		Vector3 slidingPlaneNormal = Vector3.zero;

		Vector3 initialFootPosition = position;            

		int iteration = 0;

		while( displacement.magnitude > CharacterConstants.MinMovementAmount && iteration < CharacterConstants.MaxSlideIterations )
        {
			iteration++;
			
			Vector3 previousFootPosition = position;

			CollisionInfo collisionInfo;
			bool hit = CastBody(
				out collisionInfo ,
				position ,
				displacement ,
				true ,
				tagsAndLayersProfile.staticObstaclesLayerMask
			);
			
			
			if( !hit )
			{
				position += displacement;

				if( iteration == 1 )
					SetWallCollisionInfo( collisionInfo );
				
				break;
			}

			// Save the collision info
			CollisionInfo initialCollisionInfo = collisionInfo;

			position += collisionInfo.displacement;
			displacement -= collisionInfo.displacement;  


			// Slide ------------------------------------------------------------------------------------------------------------------
			bool blocked = UpdateSlidingPlanes( 
				iteration , 
				false , 
				initialCollisionInfo , 
				ref slidingPlaneNormal , 
				ref groundPlaneNormal , 
				ref displacement 
			);

			if( iteration == 1 )
			{
				SetWallCollisionInfo( initialCollisionInfo );				
			}
           	
			
		}

	}
		
	
	bool UpdateSlidingPlanes( 
		int iteration , 
		bool stepUpResult ,
		CollisionInfo collisionInfo , 
		ref Vector3 slidingPlaneNormal , 
		ref Vector3 groundPlaneNormal , 
		ref Vector3 displacement )
	{

		Vector3 normal = collisionInfo.hitInfo.normal;
	
		
		float slopeAngle = Vector3.Angle( normal , RigidbodyUp );

		if( slopeAngle > slopeLimit )
		{    
			
			if( slidingPlaneNormal != Vector3.zero )
			{
				float correlation = Vector3.Dot( normal , slidingPlaneNormal );
				
				if( correlation > 0 )
					displacement = CustomUtilities.DeflectVector( displacement , groundPlaneNormal , normal );
				else
					displacement = Vector3.zero;                            
				
			}
			else
			{
				displacement = CustomUtilities.DeflectVector( displacement , groundPlaneNormal , normal );
			}

			slidingPlaneNormal = normal;                     
		}
		else
		{
			displacement = CustomUtilities.ProjectVectorOnPlane( 
				displacement , 
				normal ,
                Vector3.Cross( displacement , RigidbodyUp).normalized 
			);

			groundPlaneNormal = normal;
			slidingPlaneNormal = Vector3.zero;

		}

		return displacement == Vector3.zero;
	}


		

	
	bool StepUp( ref Vector3 position , ref Vector3 displacement , out CollisionInfo stepUpResultInfo )
	{		
		stepUpResultInfo = new CollisionInfo();

		Vector3 initialFootPosition = position;
		Vector3 initialDisplacement = displacement;
		
		Vector3 ascendingDisplacement = RigidbodyUp * stepOffset;
		Vector3 stepUpDisplacement = Mathf.Max( CharacterConstants.StepExtraMovement , displacement.magnitude ) * displacement.normalized; 
		Vector3 descendingDisplacement = - ascendingDisplacement;
		
		bool hit = false;


		// Ascend --------------------------------------------------------------------------
		CollisionInfo collisionInfo;
		hit = CastBody(
			out collisionInfo ,
			position ,
			ascendingDisplacement ,
			true ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);

		position += collisionInfo.displacement;
		

		// Move -----------------------------------------------------------------------------		
		hit = CastBody(
			out collisionInfo ,
			position ,
			stepUpDisplacement ,
			true ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);
		
		//Vector3 stepUpDisplacement = collisionInfo.displacement.normalized * ( collisionInfo.displacement.magnitude + CharacterConstants.StepExtraMovement );
		position += collisionInfo.displacement;
		displacement -= collisionInfo.displacement;
		
		// Return if the upper normal is unstable.
		if( hit )
		{			
			
			if( IsAnUnstableEdge( collisionInfo ) || collisionInfo.contactSlopeAngle > slopeLimit )
			{				
				position = initialFootPosition;
				displacement = initialDisplacement;
				
				return false;
				
			}
		}	

		// Descend -------------------------------------------------------------------------------
		
		hit = CastBody(
			out collisionInfo ,
			position ,
			descendingDisplacement ,
			true ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);

		position += collisionInfo.displacement;
		
		// Return if the upper normal is unstable.
		if( hit )
		{
			if( IsAnUnstableEdge( collisionInfo ) )
			{			
				position = initialFootPosition;
				displacement = initialDisplacement;
				return false;
			}		
		}
		
		bool stepUpPerformed = ( position - initialFootPosition ).magnitude > CharacterConstants.MinStepUpDifference;
		bool allowedHeight = Vector3.Project( collisionInfo.hitInfo.point - initialFootPosition , RigidbodyUp ).magnitude <= stepOffset;


		if( stepUpPerformed && allowedHeight )
		{	
			stepUpResultInfo = collisionInfo;
			return true;
		}
		else
		{
			position = initialFootPosition;
			displacement = initialDisplacement;
			return false;
		}
		
	}


	bool StepUpShrink( ref Vector3 position , ref Vector3 displacement , out CollisionInfo stepUpResultInfo )
	{		
		stepUpResultInfo = new CollisionInfo();

		Vector3 initialFootPosition = position;
		Vector3 initialDisplacement = displacement;
		
		Vector3 ascendingDisplacement = RigidbodyUp * stepOffset;
		Vector3 descendingDisplacement = - ascendingDisplacement;
		bool hit = false;


		// Ascend and Move ---------------------------------------------------------------------		
		position += ascendingDisplacement + displacement;

		// Descend -------------------------------------------------------------------------------
		CollisionInfo collisionInfo;
		hit = CastBody(
			out collisionInfo ,
			position ,
			descendingDisplacement ,
			true ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);


		position += collisionInfo.displacement;
		
		bool isOverlapping = CheckOverlapWithLayerMask( 
			position ,
			tagsAndLayersProfile.staticObstaclesLayerMask
		);
		
		if( isOverlapping )
		{
			position = initialFootPosition;
			displacement = initialDisplacement;
			return false;
		}

		return true;		
		
	}


	void UpdateCollisionInfo( out CollisionInfo collisionInfo , HitInfo hitInfo , Vector3 castDisplacement , float skin , LayerMask layerMask )
    {
		collisionInfo = new CollisionInfo();

        collisionInfo.collision = hitInfo.hit;

        if( collisionInfo.collision )
        {            
            collisionInfo.displacement = castDisplacement.normalized * ( hitInfo.distance - skin );
                    
            collisionInfo.hitInfo = hitInfo;
            collisionInfo.contactSlopeAngle = Vector3.Angle( transform.up , hitInfo.normal );            

            UpdateEdgeInfo( ref collisionInfo , layerMask );
        }
        else
        {
            collisionInfo.displacement = castDisplacement.normalized * ( castDisplacement.magnitude - skin );
                    

        }

    }	


	void UpdateEdgeInfo( ref CollisionInfo collisionInfo , LayerMask layerMask )
    {
        Vector3 center = RigidbodyComponent.Position + RigidbodyUp;

        Vector3 castDirection = ( collisionInfo.hitInfo.point - center ).normalized;
		Vector3 castDisplacement = castDirection * CharacterConstants.EdgeRaysCastDistance;

		Vector3 upperHitPosition = center + transform.up * CharacterConstants.EdgeRaysSeparation;
		Vector3 lowerHitPosition = center - transform.up * CharacterConstants.EdgeRaysSeparation;

		HitInfo upperHitInfo;
		physicsComponent.Raycast(
			out upperHitInfo,
			upperHitPosition ,
			castDisplacement ,
			layerMask
		);

        
        HitInfo lowerHitInfo;
		physicsComponent.Raycast(
			out lowerHitInfo,
			lowerHitPosition ,
			castDisplacement ,
			layerMask
		);
        		

		collisionInfo.edgeUpperNormal = upperHitInfo.normal;      
		collisionInfo.edgeLowerNormal = lowerHitInfo.normal;

        collisionInfo.edgeUpperSlopeAngle = Vector3.Angle( collisionInfo.edgeUpperNormal , transform.up );
        collisionInfo.edgeLowerSlopeAngle = Vector3.Angle( collisionInfo.edgeLowerNormal , transform.up );
	
		collisionInfo.edgeAngle = Vector3.Angle( collisionInfo.edgeUpperNormal , collisionInfo.edgeLowerNormal );

        collisionInfo.isAnEdge = CustomUtilities.isBetween( collisionInfo.edgeAngle , CharacterConstants.MinEdgeAngle , CharacterConstants.MaxEdgeAngle , true );
        collisionInfo.isAStep = CustomUtilities.isBetween( collisionInfo.edgeAngle , CharacterConstants.MinStepAngle , CharacterConstants.MaxStepAngle , true );
        
        
    }

		


#if UNITY_EDITOR

	void OnDrawGizmos()
	{
		
		
		if( !showGizmos )
			return;		
		

		//Gravity
		if( orientationMode == CharacterOrientationMode.GravityCenter )
		{
			if( gravityCenter != null )
			{
				float gravityModeMultiplier = gravityCenterMode == GravityCenterMode.Towards ? 1f : -1f;
				  
				CustomUtilities.DrawArrowGizmo(
                    transform.position ,
                    transform.position + 2f * gravityModeMultiplier * (gravityCenter.position - transform.position).normalized ,
                    Color.white 
				);
			}
		}
		else if( orientationMode == CharacterOrientationMode.FixedDirection )
		{
			CustomUtilities.DrawArrowGizmo(
                transform.position ,
                transform.position + 2f * worldGravityDirection,
                Color.white 
			);
		}

		
		//Forward Direction
		CustomUtilities.DrawArrowGizmo(
            transform.position + transform.up,
            transform.position + transform.up + forwardDirection,
            Color.green 
		);
	}

#endif
	
	
}

}

