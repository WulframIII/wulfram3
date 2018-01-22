using UnityEngine;

// Wait for Object Pool

/// <summary>
/// When objects have object pool depedencies, 
/// use this class to delay their startup. 
/// </summary>
/// <remarks>
/// When the pool is ready, the "Ready" message is sent to the controller
/// </remarks>
[AddComponentMenu("ObjectPool/ObjectReady")]
public class ObjectReady : MonoBehaviour
{
    void OnEnable()
    {
        ObjectPool.OnReady += OnLoad;

        if (ObjectPool.Ready)
            OnLoad();
    }

    void OnDisable()
    {
        ObjectPool.OnReady -= OnLoad;
    }

    public void OnLoad()
    {
        //enable controllers
        SendMessage("Ready");
    }
}