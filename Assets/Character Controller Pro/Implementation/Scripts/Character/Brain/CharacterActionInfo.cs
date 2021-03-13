using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This struct contains all the inputs actions available for the character to interact with.
/// </summary>
[System.Serializable]
public struct CharacterActionsInfo 
{
    [Header("Character")]
    public AxesCompositeAction inputAxes;
    
    public ButtonAction run;  
    public ButtonAction jump;  
    public ButtonAction shrink;  
    public ButtonAction dash;  
    public ButtonAction jetPack;
    public ButtonAction interact;

    [Header("Camera")]
    public AxesCompositeAction cameraAxes;
    public AxisAction zoomAxis;
     
    /// <summary>
    /// Reset all the actions.
    /// </summary>
	public void Reset()
	{
		inputAxes.Reset();
        run.Reset();
        jump.Reset();
        shrink.Reset();
        dash.Reset();
        jetPack.Reset();
        interact.Reset();

        cameraAxes.Reset();    
        zoomAxis.Reset(); 
	}

    /// <summary>
    /// Initialize the actions by instantiate them.
    /// </summary>
    public void InitializeActions()
    {
        inputAxes = new AxesCompositeAction();        
        run = new ButtonAction();  
        jump = new ButtonAction();  
        shrink = new ButtonAction();  
        dash = new ButtonAction();  
        jetPack = new ButtonAction();
        interact = new ButtonAction();

        cameraAxes = new AxesCompositeAction();
        zoomAxis = new AxisAction();
    }

    

}

// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// Actions ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


/// <summary>
/// This struct contains all the button states, which are updated frame by frame.
/// </summary>
[System.Serializable]
public struct ButtonAction
{       
    public bool isHeldDown;
	public bool isPressed;
	public bool isReleased;

    /// <summary>
    /// Reset the fields values.
    /// </summary>
    public void Reset()
    {
        isHeldDown = false;
        isPressed = false;
        isReleased = false;        
    }

    /// <summary>
    /// Updates the fields based on the current Get, GetDown and GetUp values. For the classic Input manager these would be Input.GetButton, Input.GetButtonDown and 
    /// Input.GetButtonUp respectively.
    /// </summary>
    public void Update( bool getState , bool getDownState , bool getUpState )
    {
        isHeldDown |= getState;
        isPressed |= getDownState;
        isReleased |= getUpState;      
        
    }    
    
    
}

[System.Serializable]
public struct AxisAction
{
    
    public float axisValue;
    
    public float AxisValue
    {
        get
        {
            return axisValue;
        }
    }    

    public void Reset()
    {
        axisValue = 0f;
    }

    public void Update( float getAxisState )
    {
        axisValue = getAxisState;
        
    }
   
}

[System.Serializable]
public struct AxesCompositeAction
{
    
    public Vector2 axesValue;

    public void Reset()
    {       
        axesValue = Vector2.zero;
    }

    public void Update( float getHorizontalAxisState , float getVerticalAxisState )
    {
        axesValue = new Vector2( getHorizontalAxisState , getVerticalAxisState );
        axesValue = Vector2.ClampMagnitude( axesValue , 1f );       
        
    }

    public void Update( Vector2 axesValue )
    {
        this.axesValue = Vector2.ClampMagnitude( axesValue , 1f );        
    }

    
    public bool AxesDetected
    {
        get
        {
            return axesValue != Vector2.zero;
        }
    }

	public bool Right
    {
        get
        {
            return axesValue.x > 0;
        }
    }

	public bool Left
    {
        get
        {
            return axesValue.x < 0;
        }
    }

	public bool Up
    {
        get
        {
            return axesValue.y > 0;
        }
    }

	public bool Down
    {
        get
        {
            return axesValue.y < 0;
        }
    }

    
}


#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( ButtonAction ) )]
public class ButtonActionEditor : PropertyDrawer
{
    GUIStyle style = new GUIStyle();

    Texture arrowTexture = null;

    string[] enumOptions = new string[]
    { 
        " ----- " ,
        "Pressed" ,
        "Released" ,
        "HeldDown" 
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( position , label , property );
        
        SerializedProperty isPressed = property.FindPropertyRelative("isPressed");
        SerializedProperty isHeldDown = property.FindPropertyRelative("isHeldDown");
        SerializedProperty isReleased = property.FindPropertyRelative("isReleased");

        if( arrowTexture == null )
            arrowTexture = Resources.Load<Texture>("whiteArrowFilledIcon");


        Vector2 labelSize = style.CalcSize( label );
        
        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width = 100;

        EditorGUI.LabelField( fieldRect , label );
        
        fieldRect.x += 110;        

        int selected = 0;

        if( isPressed.boolValue )
            selected = 1;
        else if( isReleased.boolValue )
            selected = 2;
        else if( isHeldDown.boolValue )
            selected = 3;
        else
            selected = 0;

        selected = EditorGUI.Popup( fieldRect , selected , enumOptions );

        isPressed.boolValue = selected == 1;
        isReleased.boolValue = selected == 2;
        isHeldDown.boolValue = selected == 3;
       
        

        EditorGUI.EndProperty();
    }

    void DrawToggleWithIcon( ref Rect fieldRect , SerializedProperty property , Texture iconTexture , float iconAngle )
    {
        fieldRect.width = fieldRect.height;

        
        GUI.color = Color.gray;
        GUIUtility.RotateAroundPivot( iconAngle , fieldRect.center );
        GUI.DrawTexture( fieldRect , iconTexture );
        GUIUtility.RotateAroundPivot( - iconAngle , fieldRect.center );
        GUI.color = Color.white;

        
        fieldRect.x += fieldRect.width;       

        fieldRect.width = 50;  // <-- Important!
        EditorGUI.PropertyField( fieldRect , property , GUIContent.none );
        fieldRect.x += 50;        
    }

 
}


[CustomPropertyDrawer( typeof( AxesCompositeAction ) )]
public class AxesCompositeActionEditor : PropertyDrawer
{
    

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( position , label , property );
        
        SerializedProperty axesValue = property.FindPropertyRelative("axesValue");
               
        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width = 100;

        EditorGUI.LabelField( fieldRect , label );
        
        fieldRect.x += 110;        

        EditorGUI.PropertyField( fieldRect , axesValue , GUIContent.none );
        

        EditorGUI.EndProperty();
    }

   
 
}

[CustomPropertyDrawer( typeof( AxisAction ) )]
public class AxisActionEditor : PropertyDrawer
{
    

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( position , label , property );
        
        SerializedProperty axesValue = property.FindPropertyRelative("axisValue");
               
        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width = 100;

        EditorGUI.LabelField( fieldRect , label );
        
        fieldRect.x += 110;        

        EditorGUI.PropertyField( fieldRect , axesValue , GUIContent.none );
        

        EditorGUI.EndProperty();
    }

   
 
}

#endif


}