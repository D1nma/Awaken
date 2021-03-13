using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

/// <summary>
/// This ScriptableObject contains all the properties used by the volumes and the surfaces. Create many instances as you want to create different environments.
/// </summary>
[CreateAssetMenu( menuName = "Character Controller Pro/Implementation/Materials/Material Properties")]
public class MaterialsProperties : ScriptableObject
{
    [SerializeField]
    Surface defaultSurface = new Surface();

    [SerializeField]
    Volume defaultVolume = new Volume();

    [SerializeField]
    Surface[] surfaces = null;

    [SerializeField]
    Volume[] volumes = null;

    public Surface DefaultSurface
    {
        get
        {
            return defaultSurface;
        }
    }

    public Volume DefaultVolume
    {
        get
        {
            return defaultVolume;
        }
    }

    public Surface[] Surfaces
    {
        get
        {
            return surfaces;
        }
    }    

    public Volume[] Volumes
    {
        get
        {
            return volumes;
        }
    }

    public bool GetSurface( string tag , ref Surface outputSurface )
    {        
        outputSurface = null;

        for( int i = 0 ; i < surfaces.Length ; i++ )
        {
            Surface surface = surfaces[i];
            
            if( string.Equals( tag , surface.tagName ) )
            {               
                outputSurface = surface; 
                return true;
            }                
        }

        return false;
    }

    public bool GetVolume( string tag , ref Volume outputVolume )
    {        
        outputVolume = null;
        
        for( int i = 0 ; i < volumes.Length ; i++ )
        {
            Volume volume = volumes[i];
            
            if( string.Equals( tag , volume.tagName ) )
            {               
                outputVolume = volume; 
                return true;
            }                
        }

        return false;
    }
}

public enum MaterialType
{
    Solid ,
    Trigger
}

[System.Serializable]
public class Surface
{
    public string tagName = "";
    
    [Header("Movement")]

    [Range( 0.01f , 1f )]
    public float controlMultiplier = 0.4f;

    [Range( 0.05f , 5f )]
    public float speedMultiplier = 1f;

    [Header("Particles")]

    public Color color = Color.gray;
}


[System.Serializable]
public class Volume
{    
    public string tagName = "";

    [Header("Movement")]

    [Range( 0.01f , 1f )]
    public float controlMultiplier = 0.2f;
    
    [Range( 0.05f , 50f )]
    public float gravityPositiveMultiplier = 1f;

    [Range( 0.05f , 50f )]
    public float gravityNegativeMultiplier = 1f;

    [Range( 0.05f , 5f )]
    public float speedMultiplier = 1f;



}

}

