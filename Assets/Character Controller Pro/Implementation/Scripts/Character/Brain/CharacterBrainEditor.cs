using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR

using UnityEditor;


namespace Lightbug.CharacterControllerPro.Implementation
{
 

[CustomEditor( typeof(CharacterBrain) ) , CanEditMultipleObjects]
public class CharacterBrainEditor : Editor
{    
    
    SerializedProperty isAI = null;

    SerializedProperty inputData = null;   
    SerializedProperty useRawAxes = null;
    SerializedProperty humanInputType = null;

    SerializedProperty inputHandler = null;

    SerializedProperty behaviourType = null;	
    SerializedProperty sequenceBehaviour = null;
	SerializedProperty followTarget = null;
    SerializedProperty reachDistance = null;
	SerializedProperty refreshTime = null;


    Editor inputEditor = null;
    Editor sequenceEditor = null;

    void OnEnable()
    {
        isAI = serializedObject.FindProperty("isAI");

        inputData = serializedObject.FindProperty("inputData");
        useRawAxes = serializedObject.FindProperty("useRawAxes");
        humanInputType = serializedObject.FindProperty("humanInputType");
        inputHandler = serializedObject.FindProperty("inputHandler");


        behaviourType = serializedObject.FindProperty("behaviourType");
        sequenceBehaviour = serializedObject.FindProperty("sequenceBehaviour");
        followTarget = serializedObject.FindProperty("followTarget");
        reachDistance = serializedObject.FindProperty("reachDistance");
        refreshTime = serializedObject.FindProperty("refreshTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CustomUtilities.DrawMonoBehaviourField<CharacterBrain>( (CharacterBrain)target);

        GUILayout.Space(10);

        //GUILayout.BeginVertical( EditorStyles.helpBox );

        GUILayout.BeginHorizontal();

        //GUI.enabled = isAI.boolValue;
        GUI.color = isAI.boolValue ? Color.white : Color.green;
        if( GUILayout.Button( "Human" , EditorStyles.miniButton ) )
        {
            isAI.boolValue = false;
        }
        
        //GUI.enabled = !isAI.boolValue;
        GUI.color = !isAI.boolValue ? Color.white : Color.green;
        if( GUILayout.Button( "AI" , EditorStyles.miniButton ) )
        {
            isAI.boolValue = true;
        }

        GUI.color = Color.white;

        //GUI.enabled = true;

        GUILayout.EndHorizontal();

        CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

        GUILayout.Space(15);

        if( isAI.boolValue )
        {
            EditorGUILayout.PropertyField( behaviourType );

            GUILayout.Space(10);

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

            //GUILayout.BeginVertical( EditorStyles.helpBox );

            if( behaviourType.enumValueIndex == 0)
            {

                EditorGUILayout.PropertyField( sequenceBehaviour );



                if( sequenceBehaviour.objectReferenceValue != null )
                {
                    if( sequenceEditor == null )
                        CreateCachedEditor( sequenceBehaviour.objectReferenceValue , null , ref sequenceEditor );
                    
                    sequenceEditor.OnInspectorGUI();
                }
                else
                {
                    if( sequenceEditor != null )
                        sequenceEditor = null;

                    EditorGUILayout.HelpBox( "Select a Sequence Behaviour asset" , MessageType.Warning );
                }

            }
            else
            {
                EditorGUILayout.PropertyField( followTarget );

                if( followTarget.objectReferenceValue != null )
                {
                    EditorGUILayout.PropertyField( reachDistance );

                    EditorGUILayout.PropertyField( refreshTime );

                }
                else
                {
                    EditorGUILayout.HelpBox( "Assign a target" , MessageType.Warning );
                }

                
            }

            //GUILayout.EndVertical();

        }
        else
        {
            EditorGUILayout.PropertyField( humanInputType );

            switch( (HumanInputType)humanInputType.enumValueIndex )
            {
                case HumanInputType.UnityInputManager:

                    
                    
                    break;
                case HumanInputType.UI_Mobile:

                    EditorGUILayout.HelpBox(
                        "This mode will find the buttons (\"ButtonUIElement\") and axes (\"AxesUIElement\") from the scene, and store it into a dictionary. " + 
                        "Make sure these elements names match with the input data names you want to trigger." , 
                        MessageType.Info 
                    );
                    
                    break;
                case HumanInputType.Custom:

                    // It shows only for this mode
                    EditorGUILayout.PropertyField( inputHandler );

                    EditorGUILayout.HelpBox(
                        "Plug your own monobehaviour component into this field to use your custom input solution. Its class must derived from \"CharacterCustomInputs\" and implement all the " + 
                        "commonly used methods (GetButton, GetButtonDown, GetAxis, etc.)." , 
                        MessageType.Info 
                    );

                    
                    
                    break;
            }

            GUILayout.Space(10);
            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

            EditorGUILayout.PropertyField( inputData );
            EditorGUILayout.PropertyField( useRawAxes );

            if( inputData.objectReferenceValue != null )
            {

                if( inputEditor == null )
                    CreateCachedEditor( inputData.objectReferenceValue , null , ref inputEditor );
                
                inputEditor.OnInspectorGUI();

            }
            else
            {
                if( inputEditor != null )
                    inputEditor = null;
                
                EditorGUILayout.HelpBox( "Select a Character Input Data asset" , MessageType.Warning );
            }

            

            

            

            

            

            

        }

        GUILayout.Space(10);


        //GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    
    
}

}

#endif
