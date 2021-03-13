using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// Represents the main component for the UI input handler (mobile). This component is updated in the scene by UI elements.
/// </summary>
[AddComponentMenu("Character Controller Pro/Implementation/UI/Mobile Input")]
public class MobileInput : MonoBehaviour
{
    [SerializeField]
    string axisName = null;
    
    // IButtonInput ----------------------------------------------------------------------------------------
    public string AxisName
    {
        get
        {
            return axisName;
        }
    }    

    public float AxisValue{ get; set;}
    public bool IsPressed{ get; set;}
    public bool IsReleased{ get; set;}
    public bool IsHeldDown { get; set;}    
    

}

}
