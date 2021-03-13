﻿using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.Utilities
{

/// <summary>
/// This static class contains all kind of useful methods used across the package.
/// </summary>
public static class CustomUtilities
{
    public static Vector3 ProjectVectorOnPlane( Vector3 vector , Vector3 planeNormal , Vector3 rotationAxis , bool mantainMagnitud = true )
    {        
        Vector3 componentToEliminate = Vector3.Project( planeNormal , rotationAxis );
		planeNormal = ( planeNormal - componentToEliminate ).normalized;
        
        Vector3 projectedVector = Vector3.ProjectOnPlane( vector , planeNormal );

        if( mantainMagnitud )
            projectedVector = vector.magnitude * projectedVector.normalized;
                

        return projectedVector;
                
    }

    public static Vector3 RemoveComponent( Vector3 vector , Vector3 component )
    {
        Vector3 componentToEliminate = Vector3.Project( vector , component );
        return vector - componentToEliminate;
        
    }

    public static Vector3 DeflectVector( Vector3 vector , Vector3 groundNormal , Vector3 planeNormal , bool mantainMagnitude = false )
    {
        Vector3 direction = Vector3.Cross( groundNormal , planeNormal ).normalized;

        Vector3 result = Vector3.Project( vector , direction );

        return result;
    }

	
    /// <summary>
	/// Returns true if the target value is between a and b ( both exclusive ). 
	/// To include the limits values set the \"inclusive\" bool to true.
	/// </summary>
	public static bool isBetween( float target, float a , float b , bool inclusive = false )
	{	

		if( b > a )
			return ( inclusive ? target >= a : target > a ) && ( inclusive ? target <= b : target < b );
		else
			return ( inclusive ? target >= b : target > b ) && ( inclusive ? target <= a : target < a );
	}

	/// <summary>
	/// Returns true if the target value is between a and b ( both exclusive ). 
	/// To include the limits values set the \"inclusive\" bool to true.
	/// </summary>
	public static bool isBetween( int target, int a , int b , bool inclusive = false )
	{	

		if( b > a )
			return ( inclusive ? target >= a : target > a ) && ( inclusive ? target <= b : target < b );
		else
			return ( inclusive ? target >= b : target > b ) && ( inclusive ? target <= a : target < a );
		
	}
	

	public static bool isCloseTo( Vector3 input , Vector3 target , float tolerance )
	{
		return Vector3.Distance(input, target) <= tolerance;
		
	}

	public static bool isCloseTo( float input , float target , float tolerance )
	{
		return Mathf.Abs(target - input) <= tolerance;
	}    
   
    
	public static T GetOrAddComponent<T>( this GameObject targetGameObject ) where T : Component
	{
		T existingComponent = targetGameObject.GetComponent<T>();
		if( existingComponent != null )
		{
			return existingComponent;
		}
		
		T component = targetGameObject.AddComponent<T>();
		
		return component;
	}

	public static bool IsNullOrEmpty( this string target )
	{
		return target == null || target.Length == 0;
	}

	public static bool IsNullOrWhiteSpace( this string target )
	{
		if( target == null )
			return true;

		for(int i = 0 ; i < target.Length ; i++ )
		{
			if( target[i] != ' ' )
				return false;
		}		
		
		return true;
	}

	public static List<T> FindInterfaces<T>()
	{
		List<T> interfaces = new List<T>();
		
		GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		
		foreach( var rootGameObject in rootGameObjects )
		{
			T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
			foreach( T childInterface in childrenInterfaces )
			{
				interfaces.Add( childInterface );
			}
		}
		
		return interfaces;
	}
	

	public static bool BelongsToLayerMask(int layer , int layerMask)
	{		
		return ( layerMask & (1 << layer) ) > 0;
	}


	public static T2 GetOrRegisterValue< T1, T2 >( this Dictionary< T1, T2 > dictionary , T1 key ) where T1 : Component where T2 : Component
	{
		if( key == null )
			return null;
	
		T2 value;				
		bool found = dictionary.TryGetValue( key, out value );
		
		if( !found )
		{
			value = key.GetComponent<T2>();

			if( value!= null )
				dictionary.Add( key , value );
		}

		return value;
	}

	

	public static float SignedAngle( Vector3 from , Vector3 to , Vector3 axis )
	{
		float angle = Vector3.Angle( from , to );
		Vector3 cross = Vector3.Cross( from , to ).normalized;
		float sign = cross == axis ? 1f : -1f;

		return sign * angle;
		
	}

	public static void DebugRay( Vector3 point , Vector3 direction = default( Vector3 ) , float duration = 2f , Color color = default( Color ) )
	{
		Vector3 drawDirection = direction == default( Vector3 ) ? Vector3.up : direction;
		Color drawColor = color == default( Color ) ? Color.blue : color;
		
		Debug.DrawRay( point , drawDirection , drawColor , duration );
	}

	public static void DrawArrowGizmo( Vector3 start , Vector3 end , Color color , float radius = 0.25f )
	{
		Gizmos.color = color;
		Gizmos.DrawLine( start , end );
		
		Gizmos.DrawRay( 
			end , 
			Quaternion.AngleAxis( 45 , Vector3.forward ) * ( start - end ).normalized * radius
		);

		Gizmos.DrawRay( 
			end , 
			Quaternion.AngleAxis( -45 , Vector3.forward ) * ( start - end ).normalized * radius
		);
	}

	public static void DrawCross( Vector3 point , float radius , Color color )
	{
		Gizmos.color = color;		
		
		Gizmos.DrawRay( 
			point + Vector3.up * 0.5f * radius , 
			Vector3.down * radius
		);

		Gizmos.DrawRay( 
			point + Vector3.right * 0.5f * radius , 
			Vector3.left * radius
		);
	}

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	
	#if UNITY_EDITOR

	

	public static void DrawMonoBehaviourField<T>( T target ) where T : MonoBehaviour
	{
		GUI.enabled = false;
		EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour( target ), typeof(T), false);
		GUI.enabled = true;
	}

	public static void DrawScriptableObjectField<T>( T target ) where T : ScriptableObject
	{
		GUI.enabled = false;
		EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject( target ), typeof(T), false);
		GUI.enabled = true;
	}

	public static void DrawEditorLayoutHorizontalLine(Color color, int thickness = 1, int padding = 10)
	{
		Rect rect = EditorGUILayout.GetControlRect( GUILayout.Height( padding + thickness ) );

		rect.height = thickness;
		rect.y += padding / 2;
		//rect.x -= 2;
		//rect.width +=6;

		EditorGUI.DrawRect( rect , color );
	}

	public static void DrawEditorHorizontalLine( ref Rect rect , Color color, int thickness = 1, int padding = 10)
	{		
		rect.height = thickness;
		rect.y += padding / 2;
		//rect.x -= 2;
		//rect.width +=6;

		EditorGUI.DrawRect( rect , color );

		rect.y += padding;
		rect.height = EditorGUIUtility.singleLineHeight;
	}

	public static void DrawWireCapsule( Vector3 position , Quaternion rotation, float radius, float height, Color color = default(Color) )
    {
        if (color != default(Color))
            Handles.color = color;
        
		Matrix4x4 angleMatrix = Matrix4x4.TRS( position, rotation, Handles.matrix.lossyScale );

        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (height - (radius * 2)) / 2;
 
            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
            Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);

            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
            Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);

            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
 
        }
    }

	
	
	
	public static void DrawArray( SerializedProperty rootProperty , string namePropertyString = null )
    {
        if( !rootProperty.isArray )
            return;
        

        for( int i = 0 ; i < rootProperty.arraySize ; i++ )
        {
            SerializedProperty item = rootProperty.GetArrayElementAtIndex( i );
            item.isExpanded = true;   

			GUILayout.BeginVertical( EditorStyles.helpBox );

            CustomUtilities.DrawArrayElement( item , namePropertyString );

			GUILayout.Space( 20 );
			if( GUILayout.Button("Remove element" , EditorStyles.miniButton ) )
			{
				rootProperty.DeleteArrayElementAtIndex( i );
				break;
			}

			GUILayout.EndVertical();

			GUILayout.Space( 20 );

        }

		

		if( GUILayout.Button("Add element" ) )
		{
			rootProperty.arraySize++;
		}
        
    }
	
	public static void DrawArrayElement( SerializedProperty property , string namePropertyString = null , bool skipFirstChild = false )
    {
        if( property.isArray )
            return;
        
        EditorGUI.indentLevel++;

		SerializedProperty nameProperty = null;

		if( namePropertyString != null )
		{
			nameProperty = property.FindPropertyRelative( namePropertyString );
			if( nameProperty != null )
			{
				string name = nameProperty.stringValue;

				
				EditorGUILayout.LabelField( name , EditorStyles.boldLabel );
				


			}

			EditorGUI.indentLevel++;
		}
		

        

        SerializedProperty itr = property.Copy();

        bool enterChildren = true;

        while( itr.Next( enterChildren ) )
        {
            
            if( SerializedProperty.EqualContents( itr , property.GetEndProperty() ) )
				break;
            
			if( enterChildren && skipFirstChild )
			{
				enterChildren = false;
				continue;
			}
			
            EditorGUILayout.PropertyField( itr , enterChildren );

            enterChildren = false;            

        }

        EditorGUI.indentLevel = nameProperty != null ? EditorGUI.indentLevel - 2 : EditorGUI.indentLevel - 1; 

        
    }

	#endif
    
}

}
