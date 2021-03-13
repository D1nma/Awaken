using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{
	
/// <summary>
/// This ScriptableObject contains of the info related to the layers and tags used by the CharacterActor component.
/// </summary>
[CreateAssetMenu( menuName = "Character Controller Pro/Core/Layers Profile" )]
public class CharacterTagsAndLayersProfile : ScriptableObject
{
	[Header("Layers")]	

    [Tooltip("Assign all the static geometry layers.")]
	public LayerMask staticObstaclesLayerMask;

	[Tooltip("Assign all the valid dynamic geometry layers (these objects will be considerer \"platforms\").")]	
	public LayerMask dynamicGroundLayerMask;
	
	[Tooltip("It's recommended to assign all the dynamic rigidbodies from the scene. they will serve as valid ground plus interactable rigidbodies.")]
	public LayerMask dynamicRigidbodiesLayerMask;


	[Header("Tags")]
	public string contactRigidbodiesTag = "ContactRigidbody";
}

}
