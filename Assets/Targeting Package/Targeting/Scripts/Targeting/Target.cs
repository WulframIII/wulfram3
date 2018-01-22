using System;
using UnityEngine;

/// <summary>
/// Message passed From the target to its visuals
/// Defines the visual state of the target
/// </summary>
[Serializable]
public class TargetState
{
    /// <summary>
    /// Targets color
    /// </summary>
    public Color Color;

    /// <summary>
    /// Target's name
    /// </summary>
    public string Name;

    /// <summary>
    /// Is this visible
    /// </summary>
    public bool Visible;

    /// <summary>
    /// Is this selected
    /// </summary>
    public bool Selected;

    /// <summary>
    /// transform of the target
    /// </summary>
    public Transform Transform;

}

/// <summary>
/// Message passed from a scanner to resident targets
/// Augments the targets visual state
/// </summary>
[Serializable]
public class TargetScan : ObjectMessage<TargetScan>
{
    /// <summary>
    /// positive if entering a detection area
    /// negative if leaving a detection area
    /// </summary>
    public int Detected;

    /// <summary>
    /// positive if entering a selection area
    /// negative if leaving a selection area
    /// </summary>
    public int Selected;

    /// <summary>
    /// positive if entering a stealth area
    /// negative if leaving a stealth area
    /// </summary>
    public int Stealth;
}

/// <summary>
/// Main target script. Attach onto any targetable object.
/// Receives TargetScan messages 
/// Sends TargetState messages
///  which may be picked up by the Targeting System.
/// Handles the TargetMessage
/// </summary>
[AddComponentMenu("Targeting/Target")]
public class Target : MonoBehaviour
{
    /// <summary>
    /// prefab of the visual to use.
    /// </summary>
    public GameObject Prefab;

    /// <summary>
    /// the visual instance
    /// </summary>
    [HideInInspector]
    protected GameObject Instance;

    /// <summary>
    /// If true the target info will be sent across this objects hierarchy.
    /// This is usefull for the painting of meshes and such
    /// </summary>
    public bool BroadcastState = false;

    /// <summary>
    /// Color to paint the visual
    /// </summary>
    public Color Color = Color.red;

    /// <summary>
    /// name to write on the visual
    /// </summary>
    public string Name = "Target";

    /// <summary>
    /// Counter for detect modifiers (enables)
    /// </summary>
    [HideInInspector]
    public int DetectedCounter;

    /// <summary>
    /// Counter for select modifiers
    /// </summary>
    [HideInInspector]
    public int SelectCounter;

    /// <summary>
    /// Counter for stealth modifiers
    /// </summary>
    [HideInInspector]
    public int StealthCounter;

    /// <summary>
    /// Instance of a message sent.
    /// </summary>
    protected TargetState State = new TargetState();

    void OnEnable()
    {
        // create new visual instance
        if (Instance == null && Prefab != null)
        {
            // use pool ?
            if (ObjectPool.Instance)
            {
                Instance = ObjectPool.Instance.GetNext(Prefab, true);
            }
            else
            {
                Instance = GameObject.Instantiate(Prefab) as GameObject;
            }

        }

        //update visual
        ApplyState();

        TargetScan.Subscribe(gameObject, OnTargetScan);
    }

    void OnDisable()
    {
        //recycle
        if (Instance)
        {
            if (ObjectPool.Instance)
            {
                ObjectPool.Instance.Recycle(Instance);
            }
            else
            {
                Destroy(Instance);
            }

            Instance = null;
        }

        TargetScan.Unsubscribe(gameObject, OnTargetScan);

        //reset
        DetectedCounter = SelectCounter = StealthCounter = 0;
    }

    /// <summary>
    /// sent from TargetScanner
    /// </summary>
    /// <param name="m"></param>
    void OnTargetScan(TargetScan m)
    {
        //count
        StealthCounter = Mathf.Clamp(m.Stealth + StealthCounter, 0, 1);
        DetectedCounter = Mathf.Clamp(m.Detected + DetectedCounter, 0, 1);
        SelectCounter = Mathf.Clamp(m.Selected + SelectCounter, 0, 1);

        //update visual
        ApplyState();
    }

    /// <summary>
    /// should be called if target visibility have changed 
    /// </summary>
    [ContextMenu("ApplyState")]
    public void ApplyState()
    {
        //set state message
        State.Color = Color;
        State.Name = Name;
        State.Visible = DetectedCounter > 0 && StealthCounter <= 0;
        State.Selected = State.Visible && SelectCounter > 0;
        State.Transform = transform;

        // send to instance
        if (Instance)
            Instance.SendMessage("OnTargetState", State);

        //send change
        if (BroadcastState)
            gameObject.BroadcastMessage("OnTargetState", State, SendMessageOptions.DontRequireReceiver);
    }

    [ContextMenu("PrintDebug")]
    public void PrintDebug()
    {
        //set state message
        State.Color = Color;
        State.Name = Name;
        State.Visible = DetectedCounter > 0 && StealthCounter <= 0;
        State.Selected = SelectCounter > 0;

        Debug.Log("State.Color " + State.Color);
        Debug.Log("State.Name " + State.Name);
        Debug.Log("State.Visible " + State.Visible);
        Debug.Log("State.Selected " + State.Selected);

    }
}