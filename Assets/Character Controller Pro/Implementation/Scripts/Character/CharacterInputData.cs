using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This ScriptableObject contains all the names used as input actions by the human brain. The name of the action will matters depending on the input handler used.
/// </summary>
[CreateAssetMenu( menuName = "Character Controller Pro/Implementation/Inputs/Character input data" )]
public class CharacterInputData : ScriptableObject
{
		
	[Header("Axes")]
	
	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";

	public string cameraHorizontalAxis = "Mouse X";
	public string cameraVerticalAxis = "Mouse Y";
	public string cameraZoomAxis = "Mouse ScrollWheel";

	[Header("Buttons")]

	public string run = "Run";
	public string jump = "Jump";
	public string shrink = "Shrink";
	public string dash = "Dash";
	public string jetPack = "JetPack";
	public string interact = "Interact";

	
}

}