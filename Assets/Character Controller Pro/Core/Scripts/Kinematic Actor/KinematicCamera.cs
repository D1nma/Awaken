using System.Collections;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    

/// <summary>
/// This abstract class represents a kinematic camera that can be updated by setting its position and rotation in every FixedUpdate.
/// </summary>
public abstract class KinematicCamera : KinematicActor
{        
    /// <summary>
    /// Gets the interpolation flag state. If this is true the scene controller will perform interpolation over the kinematic camera.
    /// Otherwise it will ignore it.
    /// </summary>
    public bool InterpolationFlag{ get; protected set; }

    protected virtual void Start()
    {
        // If there is no sceneController in the scene create one.
		if( SceneController.Instance == null )
		{			
			GameObject sceneController = new GameObject("Scene Controller");			
			sceneController.AddComponent<SceneController>();			
		}

		// Add this actor to the scene controller list
		SceneController.Instance.AddActor( this );
	
    }



}

}

