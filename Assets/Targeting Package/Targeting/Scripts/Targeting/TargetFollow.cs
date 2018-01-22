using UnityEngine;

/// <summary>
/// So the visual can match the position and rotation of the target
/// </summary>
[AddComponentMenu("Targeting/TargetFollow")]
public class TargetFollow: MonoBehaviour
{
    /// <summary>
    /// Target to follow
    /// </summary>
    public Transform Follow;
    
    void Awake()
    {
        enabled = Follow != null;
    }

    void OnDisable()
    {
        enabled = false;
        Follow = null;
    }

    /// <summary>
    /// Sent from Target
    /// </summary>
    /// <param name="state"></param>
    void OnTargetState(TargetState state)
    {
        Follow = state.Transform;
        // run update?
        enabled = Follow != null && state.Visible;
    }
    
    void Update()
    {
        transform.position = Follow.position;
        transform.rotation = Follow.rotation;
    }
}