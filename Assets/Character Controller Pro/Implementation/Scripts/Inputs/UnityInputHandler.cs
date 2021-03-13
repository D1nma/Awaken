using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This input handler implements the input detection following the Unity's Input Manager convention. This scheme is used for desktop games.
/// </summary>
public class UnityInputHandler : InputHandler
{
    public override float GetAxis( string axisName , bool raw = true )
	{
		return raw ? Input.GetAxisRaw( axisName ) : Input.GetAxis( axisName );		
	}

	public override bool GetButton( string actionInputName )
	{
		return Input.GetButton( actionInputName );
	}

	public override bool GetButtonDown( string actionInputName )
	{
		return Input.GetButtonDown( actionInputName );
	}

	public override bool GetButtonUp( string actionInputName )
	{
		return Input.GetButtonUp( actionInputName );
	}
}

}
