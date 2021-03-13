using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

// [DefaultExecutionOrder(-1)]
public sealed class ActionController : MonoBehaviour
{   
    [Header("Input Handler")]
    [SerializeField]
	HumanInputType humanInputType = HumanInputType.UnityInputManager;

    [SerializeField]
	InputHandler inputHandler = null;

    [SerializeField]
    bool useRawAxis = true;

    [Header("Input data")]

    [SerializeField]
    AxisData[] axis = null;

    [SerializeField] 
    AxesData[] axes = null;

    [SerializeField] 
    ButtonData[] buttons = null;


    Dictionary< AxisData , AxisAction > axisDictionary = new Dictionary<AxisData, AxisAction>();
    Dictionary< AxesData , AxesCompositeAction > axesDictionary = new Dictionary<AxesData, AxesCompositeAction>();
    Dictionary< ButtonData , ButtonAction > buttonsDictionary = new Dictionary<ButtonData, ButtonAction>();

    void Awake()
    {
        for( int i = 0 ; i < axis.Length ; i++ )
            axisDictionary.Add( axis[i] , new AxisAction() );
        
        for( int i = 0 ; i < axes.Length ; i++ )
            axesDictionary.Add( axes[i] , new AxesCompositeAction() );

        for( int i = 0 ; i < buttons.Length ; i++ )
            buttonsDictionary.Add( buttons[i] , new ButtonAction() );

        
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
    
    void Update()
    {
        UpdateActions();
    }

    void FixedUpdate()
    {
        UpdateActions();
    }

    void UpdateActions()
    {
        if( Time.timeScale == 0 )
			return;
        
        foreach( KeyValuePair< AxisData, AxisAction> axis in axisDictionary )
            axis.Value.Update( inputHandler.GetAxis( axis.Key.name , useRawAxis ) );
        
        foreach( KeyValuePair< AxesData , AxesCompositeAction > axes in axesDictionary )
            axes.Value.Update( inputHandler.GetAxis( axes.Key.horizontalName , useRawAxis ) , inputHandler.GetAxis( axes.Key.verticalName , useRawAxis ) );
        
        foreach( KeyValuePair< ButtonData, ButtonAction > button in buttonsDictionary )
            button.Value.Update( inputHandler.GetButton( button.Key.name ) , inputHandler.GetButtonDown( button.Key.name ) , inputHandler.GetButtonUp( button.Key.name ) );
        
    }
}

[System.Serializable]
public struct ButtonData
{
    public string name;
}

[System.Serializable]
public struct AxisData
{
    public string name;
}

[System.Serializable]
public struct AxesData
{
    public string horizontalName;
    public string verticalName;
}

}