using UnityEngine;

/// <summary>
/// Recycle script for an object managed by the object pool service.
/// Handles recycle logic when disabled
/// </summary>
[AddComponentMenu("ObjectPool/ObjectRecycle")]
public class ObjectRecycle : MonoBehaviour
{
    /// <summary>
    /// ovveride flag, for when the object should not recycle
    /// </summary>
    public bool AutoRecycle = true;

    /// <summary>
    /// Recycles this instance.
    /// </summary>
    public void Recycle()
    {
        ObjectPool.Instance.Recycle(gameObject);
    }

    void OnDisable()
    {
        // no point. gona be destroyed anyways.
        if(Application.isLoadingLevel)
            return;

        if (AutoRecycle)
            Recycle();
    }
    
    void OnApplicationQuit()
    {
        AutoRecycle = false;
    }
}