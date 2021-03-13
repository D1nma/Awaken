using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

public class GravityModifier3D : GravityModifier
{
    void OnTriggerEnter( Collider other )
    {
        if( !isReady )
            return;
        
        CharacterActor characterActor = GetCharacter( other.transform );
        if( characterActor != null )
            ChangeGravitySettings( characterActor );
    }
}

}
