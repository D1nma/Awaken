using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Implementation
{


public abstract class CharacterAbility
{
    protected CharacterStateController characterStateController = null;

    protected CharacterActor CharacterActor
    {
        get
        {
            return characterStateController.CharacterActor;
        }
    }

    protected CharacterBrain CharacterBrain
    {
        get
        {
            return characterStateController.CharacterBrain;
        }
    }

    protected CharacterActionsInfo CharacterActions
    {
        get
        {
            return characterStateController.CharacterBrain.CharacterActions;
        }
    }

    public void Initialize( CharacterStateController characterStateController )
     {          
        this.characterStateController = characterStateController;

     }

    public abstract void Update( float dt , ref Vector3 velocity , ref Vector3 size );
    
}

}
