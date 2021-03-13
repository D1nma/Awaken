using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

public class GravityModifier2D : GravityModifier
{
    void OnTriggerEnter2D( Collider2D other )
    {
        if( !isReady )
            return;
        
        CharacterActor characterActor = GetCharacter( other.transform );
        if( characterActor != null )
            ChangeGravitySettings( characterActor );
    }
}

}
