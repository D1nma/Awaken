using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{
	
/// <summary>
/// This abstract class contains all the input methods that are used by the character brain. This is the base class for all the input detection methods available.
/// </summary>
public abstract class InputHandler : MonoBehaviour
{

	public abstract float GetAxis( string axisName , bool raw = true );

	public abstract bool GetButton( string actionInputName );

	public abstract bool GetButtonDown( string actionInputName );

	public abstract bool GetButtonUp( string actionInputName );
}

}
