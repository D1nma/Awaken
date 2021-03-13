using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// Struct that contains the information of the contact, gathered from the collision message ("enter" and "stay").
/// </summary>
public struct Contact
{
    
    /// <summary>
    /// Flag that indicates if this contact was created in the OnCollisionEnter (2D/3D) message.
    /// </summary>
	public bool firstContact;

    /// <summary>
    /// The contact point.
    /// </summary>
	public Vector3 point;

    /// <summary>
    /// The contact normal.
    /// </summary>
    public Vector3 normal;

    /// <summary>
    /// The 2D collider component associated with the collided object.
    /// </summary>
    public Collider2D collider2D;

    /// <summary>
    /// The 3D collider component associated with the collided object.
    /// </summary>
    public Collider collider3D;

    /// <summary>
    /// Flag that indicates if the collided object is a rigidbody or not.
    /// </summary>
	public bool isRigidbody;

    /// <summary>
    /// Flag that indicates if the collided object is a kinematic rigidbody or not.
    /// </summary>
	public bool isKinematicRigidbody;

    /// <summary>
    /// The point velocity of the rigidbody associated at the contact point.
    /// </summary>
    public Vector3 pointVelocity;

    /// <summary>
    /// The gameObject representing the collided object.
    /// </summary>
    public GameObject gameObject;
}

}

