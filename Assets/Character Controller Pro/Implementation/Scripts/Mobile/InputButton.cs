using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This class reads the actions of a 2D UI button and then sends the states flags to a mobile input component.
/// </summary>
[AddComponentMenu("Character Controller Pro/Implementation/UI/Input Button")]
public class InputButton : MonoBehaviour , IPointerUpHandler , IPointerDownHandler
{
    [Header("Target")]

    [SerializeField]
    MobileInput buttonMobileInput = null;
    
    bool wasHeldDown = false;

    public bool IsPressed{ get; set;}
    public bool IsReleased{ get; set;}
    public bool IsHeldDown { get; set;}  



    public void OnPointerDown(PointerEventData eventData)
    {
        IsHeldDown = true;           
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsHeldDown = false;        
    }

    

    void Update()
    {
        buttonMobileInput.IsPressed = IsHeldDown && !wasHeldDown;        
        buttonMobileInput.IsReleased = !IsHeldDown && wasHeldDown;
        buttonMobileInput.IsHeldDown = IsHeldDown;
        
        wasHeldDown = IsHeldDown;
    }
}


}