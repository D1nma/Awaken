using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{
    
[System.Serializable]
public class CharacterReferenceObject
{
    public Transform referenceTransform;

    public CharacterOrientationMode gravityMode = CharacterOrientationMode.FixedDirection;

    public GravityCenterMode gravityCenterMode = GravityCenterMode.Towards;

    public bool useNegativeUpAsGravity = true;

    public Transform gravityCenter = null;

}

}
