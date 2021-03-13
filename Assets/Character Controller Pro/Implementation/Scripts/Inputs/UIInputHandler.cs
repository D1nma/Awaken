using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This input handler implements the input detection for UI elements (mobile UI).
/// </summary>
public class UIInputHandler : InputHandler
{    
    
    Dictionary< string , MobileInput > axesDictionary = new Dictionary< string , MobileInput >();

    void Awake()
    {
        MobileInput[] axesArray = GameObject.FindObjectsOfType<MobileInput>();

        for( int i = 0 ; i < axesArray.Length ; i++ )
		{
            MobileInput axes = axesArray[i];
            
            axesDictionary.Add( axes.AxisName , axes );            
        }		

    }

    public override float GetAxis( string axisName , bool raw = true )
	{        
		MobileInput axes;
        bool found = axesDictionary.TryGetValue( axisName , out axes );

        if( !found )
            return 0f;
        else
        {
            return axes.AxisValue;		    
        }	
	}

	public override bool GetButton( string actionInputName )
	{
        MobileInput button;
        bool found = axesDictionary.TryGetValue( actionInputName , out button );

        if( !found )
            return false;
        else
		    return button.IsHeldDown;
	}

	public override bool GetButtonDown( string actionInputName )
	{
		MobileInput button;
        bool found = axesDictionary.TryGetValue( actionInputName , out button );

        if( !found )
            return false;
        else
		    return button.IsPressed;
	}

	public override bool GetButtonUp( string actionInputName )
	{
		MobileInput button;
        bool found = axesDictionary.TryGetValue( actionInputName , out button );

        if( !found )
            return false;
        else
		    return button.IsReleased;
	}
}

}
