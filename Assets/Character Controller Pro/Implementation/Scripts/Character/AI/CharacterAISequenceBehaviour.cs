using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This class is a ScriptableObject that works as a container of AI actions. Instances can be created and customized via the menu, thus creating many types of predefined behaviours.
/// </summary>
[CreateAssetMenu( menuName = "Character Controller Pro/Implementation/AI/Sequence Behaviour" )]
public class CharacterAISequenceBehaviour : ScriptableObject
{
	
	[SerializeField] 
	List<CharacterAIAction> actionSequence = new List<CharacterAIAction>();
	
	/// <summary>
	/// Gets the sequence list.
	/// </summary>
	public List<CharacterAIAction> ActionSequence
	{
		get
		{
			return actionSequence;
		}
	}

	

	
}


}
