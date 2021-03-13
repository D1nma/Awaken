﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// A "KinematicPlatform" implementation whose movement and rotation are purely based on nodes. Use this component to create platforms that moves and 
/// rotate precisely following a predefined path.
/// </summary>
[AddComponentMenu("Character Controller Pro/Implementation/Kinematic platform/Node based platform")]
public class NodeBasedPlatform : KinematicPlatform
{
    
    public enum SequenceType 
	{
		Rewind ,
		Loop ,
		OneWay
	}


	enum ActionState
	{
		Idle ,
		Ready, 
		Waiting, 
		Working , 
		Done
	}


	//[Header("Debug Options")]
	[SerializeField] bool drawHandles = true;
	public bool DrawHandles
	{
		get
		{
			return drawHandles;
		}
	}

	//[Header("Actions")]
	public bool move = true;
	public bool rotate = false;

	[SerializeField] 
	List<PlatformNode> actionsList = new List<PlatformNode>();

	public List<PlatformNode> ActionsList
	{
		get
		{
			return actionsList;
		}
	}

	//[Header("Properties")]

	public SequenceType sequenceType;
	public bool positiveSequenceDirection = true;

	[Range(0.1f, 50)]
	[SerializeField] 
	float globalSpeedModifier = 1;

	new Rigidbody2D rigidbody2D = null;
	Rigidbody rigidbody3D = null;

	ActionState actionState;

	Vector3 targetPosition;
	Vector3 targetRotation;

	Vector3 startingPosition;
	Vector3 startingRotation;


	bool updateInitialPosition = true;
	public bool UpdateInitialPosition
	{
		get
		{
			return updateInitialPosition;
		}
	}

	
	Vector3 initialPosition;
	public Vector3 InitialPosition
	{
		get
		{
			return initialPosition;
		}
	}

	float time = 0;

	PlatformNode currentAction = null;

	int currentActionIndex = 0;
	public int CurrentActionIndex
	{
		get
		{
			return currentActionIndex;
		}
	}

	
	protected override void Awake()
	{	
		base.Awake();

		updateInitialPosition = false;
		initialPosition = transform.position;

		actionState = ActionState.Ready;

		currentActionIndex = 0;
		currentAction = actionsList[0];

		if( GetComponent<Collider2D>() != null )
		{
			rigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
			rigidbody2D.isKinematic = true;
			rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;

		}
		else if( GetComponent<Collider>() != null )
		{
			rigidbody3D = gameObject.GetOrAddComponent<Rigidbody>();
			rigidbody3D.isKinematic = true;
			rigidbody3D.interpolation = RigidbodyInterpolation.Interpolate;
		}
		else
		{
			this.enabled = false;
		}
		
	}

	public override void UpdateKinematicActor( float dt )
    {
		
		switch(actionState)
		{
			case ActionState.Idle:
				
				break;
			case ActionState.Ready:

				SetTargets();


				actionState = ActionState.Working;
				

				break;
			
			case ActionState.Working:

				time += dt * globalSpeedModifier;
				if( time >= currentAction.targetTime )
				{

					actionState = ActionState.Done;					

					time = 0;
				}
				else
				{
					if( move )
						CalculatePosition();
						
					if( rotate )
						CalculateRotation();
				}
				

				break;
			case ActionState.Done:

				time = 0;
				
				if(positiveSequenceDirection)
				{

					if(currentActionIndex != (actionsList.Count - 1) )
					{
						currentActionIndex++;
						actionState = ActionState.Ready;
					}
					else
					{	
						switch(sequenceType)
						{
							case SequenceType.Loop:
							
							currentActionIndex = 0;
							actionState = ActionState.Ready;

							break;
							case SequenceType.Rewind:

							currentActionIndex--;
							positiveSequenceDirection = false;
							actionState = ActionState.Ready;

							break;
							case SequenceType.OneWay:

							actionState = ActionState.Idle;

							break;							
						}					

						

					}


				}
				else
				{
					if(currentActionIndex != 0 )
					{
						currentActionIndex--;
						actionState = ActionState.Ready;
					}
					else
					{

						switch(sequenceType)
						{
							case SequenceType.Loop:
							
							currentActionIndex = actionsList.Count-1;
							actionState = ActionState.Ready;

							break;
							case SequenceType.Rewind:

							currentActionIndex++;
							positiveSequenceDirection = true;
							actionState = ActionState.Ready;

							break;
							case SequenceType.OneWay:

							actionState = ActionState.Idle;

							break;							
						}
						

					}
				}

				currentAction = actionsList[currentActionIndex];

				break;
		}

	}

	
	public override string ToString()
	{
		return "Current Index = " + currentActionIndex + '\n' + 
		"State = " + actionState;
	}

	void SetTargets()
	{
		startingPosition = transform.position;
		startingRotation = transform.eulerAngles;

		targetPosition = initialPosition + currentAction.position;
		targetRotation = currentAction.eulerAngles;	

	}


	void CalculatePosition()
	{	

		float curveTime = time / currentAction.targetTime;

		RigidbodyComponent.Position = Vector3.Lerp(
            startingPosition, 
			this.targetPosition ,
            currentAction.movementCurve.Evaluate(curveTime) 
		);


        

	}

	void CalculateRotation()
	{		

		float curveTime = time / currentAction.targetTime;

		float curveResult = currentAction.rotationCurve.Evaluate(curveTime);
		
		Vector3 finalAngle;
		
		finalAngle.x = Mathf.LerpAngle(startingRotation.x , this.targetRotation.x , curveResult);
		finalAngle.y = Mathf.LerpAngle(startingRotation.y , this.targetRotation.y , curveResult);
		finalAngle.z = Mathf.LerpAngle(startingRotation.z , this.targetRotation.z , curveResult);
		
		
		RigidbodyComponent.Rotation = Quaternion.Euler( finalAngle );

		
		
	}

	
}

}
