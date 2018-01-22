using UnityEngine;

/// <summary>
/// look at main camera
/// </summary>
[AddComponentMenu("Targeting/LookAtMain")]
public class LookAtMain:MonoBehaviour
{
    protected Transform Transform;

    public Vector3 Up = Vector3.up;

    void Awake()
    {
        Transform = transform;
    }

    void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform, Up);
    }
}