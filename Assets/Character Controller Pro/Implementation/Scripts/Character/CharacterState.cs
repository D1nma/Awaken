using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Implementation
{	


/// <summary>
/// This class represents a state, that is, a basic element used by the character state controller (finite state machine).
/// </summary>
[RequireComponent( typeof( CharacterStateController ) )]
public abstract class CharacterState : MonoBehaviour , IUpdatable
{
     CharacterActor characterActor;
     CharacterBrain characterBrain;
     CharacterStateController characterStateController;   
     
     /// <summary>
     /// Gets the CharacterActor component of the gameObject.
     /// </summary>
     public CharacterActor CharacterActor
     {
          get
          {
               if( characterActor == null)
				characterActor = GetComponent<CharacterActor>();
               
               return characterActor;
          }
     }
     
     /// <summary>
     /// Gets the CharacterBrain component of the gameObject.
     /// </summary>
     public CharacterBrain CharacterBrain
     {
          get
          {
               if( characterBrain == null)
				characterBrain = GetComponent<CharacterBrain>();
               
               return characterBrain;
          }
     }

     /// <summary>
     /// Gets the current brain actions CharacterBrain component of the gameObject.
     /// </summary>
     public CharacterActionsInfo CharacterActions
     {
          get
          {
               if( characterBrain == null)
				characterBrain = GetComponent<CharacterBrain>();
               
               return characterBrain.CharacterActions;
          }
     }

     /// <summary>
     /// Gets the CharacterStateController component of the gameObject.
     /// </summary>
     public CharacterStateController CharacterStateController
     {
          get
          {
               if( characterStateController == null)
				characterStateController = GetComponent<CharacterStateController>();
               
               return characterStateController;
          }
     }


     /// <summary>
     /// Gets the state name. Since this is an abstract property the state must implement it.
     /// </summary>
     public abstract string Name{ get; }        
     

     protected virtual void Awake()
     {          
          characterStateController = GetComponent<CharacterStateController>();          
          characterBrain = characterStateController.CharacterBrain;
          characterActor = characterStateController.CharacterActor;

     }

     /// <summary>
     /// This method runs once when the state has entered the state machine.
     /// </summary>
     public virtual void EnterBehaviour( float dt )
     {
     }

     /// <summary>
     /// This methods runs before the main Update method.
     /// </summary>
     public virtual void PreUpdateBehaviour( float dt )
     {
     }

     /// <summary>
     /// This method runs frame by frame, and should be implemented by the derived state class.
     /// </summary>
     public abstract void UpdateBehaviour( float dt );

     /// <summary>
     /// This methods runs after the main Update method.
     /// </summary>
     public virtual void PostUpdateBehaviour( float dt )
     {
     }

     /// <summary>
     /// This method runs once when the state has exited the state machine.
     /// </summary>
     public virtual void ExitBehaviour( float dt )
     {
     }

     /// <summary>
     /// Checks if the required conditions to exit this state are true. If so it returns the desired state (null otherwise). After this the state machine will
     /// proceed to evaluate the "enter transition" condition on the target state.
     /// </summary>
     public virtual CharacterState CheckExitTransition()
     {
          return null;
     }

     /// <summary>
     /// Checks if the required conditions to enter this state are true. If so the state machine will automatically change the current state to the desired one.
     /// </summary>  
     public virtual bool CheckEnterTransition( CharacterState fromState )
     {
          return true;
     }
    
     
     public virtual string GetInfo()
     {
          return "";
     }
	
}

}
