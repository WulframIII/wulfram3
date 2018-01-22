using UnityEngine;

/// <summary>
/// Sends a TargetScan message to collided targets.
/// Used for selector, stealth area, and scanner
/// Modifies Target State
/// </summary>
[AddComponentMenu("Targeting/TargetScanner")]
[RequireComponent(typeof(Collider))]
public class TargetScanner : MonoBehaviour
{
    /// <summary>
    /// Defines the message to send to the target
    /// </summary>
    public enum ScannerMode
    {
        /// <summary>
        /// Detect +1 / -1
        /// </summary>
        Detect,
        /// <summary>
        /// Select +1 / -1
        /// </summary>
        Select,
        /// <summary>
        /// Stealth +1 / -1
        /// </summary>
        Stealth,
        /// <summary>
        /// Stealth -1 / +1
        /// </summary>
        AntiSteal

    }

    /// <summary>
    /// Defines the message to send to the target
    /// </summary>
    public ScannerMode Mode;

    /// <summary>
    /// The message sent to the target on enter.
    /// Defined in OnModeChange
    /// </summary>
    protected TargetScan EnterMessage = new TargetScan { Detected = 1, Selected = 0, Stealth = 0 };

    /// <summary>
    /// The message sent to the target on exit.
    /// Defined in OnModeChange
    /// </summary>
    protected TargetScan ExitMessage = new TargetScan { Detected = -1, Selected = 0, Stealth = 0 };
    
    void OnEnable()
    {
        OnModeChanged();

        GetComponent<Collider>().enabled = true;
        GetComponent<Collider>().isTrigger = true;

        // rigidbody needed if the scanner moves
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().isKinematic = true;

    }

    void OnDisable()
    {
        GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// call if Mode Changes.
    /// Defines EnterMessage / ExitMessage
    /// </summary>
    [ContextMenu("OnModeChanged")]
    public void OnModeChanged()
    {
        switch (Mode)
        {
            case ScannerMode.Detect:
                EnterMessage.Detected = 1;
                EnterMessage.Stealth = 0;
                EnterMessage.Selected = 0;
                ExitMessage.Detected = -1;
                ExitMessage.Stealth = 0;
                ExitMessage.Selected = 0;
                break;
            case ScannerMode.Select:
                EnterMessage.Detected = 0;
                EnterMessage.Stealth = 0;
                EnterMessage.Selected = 1;
                ExitMessage.Detected = 0;
                ExitMessage.Stealth = 0;
                ExitMessage.Selected = -1;
                break;
            case ScannerMode.Stealth:
                EnterMessage.Detected = 0;
                EnterMessage.Stealth = 1;
                EnterMessage.Selected = 0;
                ExitMessage.Detected = 0;
                ExitMessage.Stealth = -1;
                ExitMessage.Selected = 0;
                break;
            case ScannerMode.AntiSteal:
                EnterMessage.Detected = 1;
                EnterMessage.Stealth = -1;
                EnterMessage.Selected = 0;
                ExitMessage.Detected = 0;
                ExitMessage.Stealth = 1;
                ExitMessage.Selected = 0;
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (TargetScan.SendMessage(other.gameObject, EnterMessage))
        {
            if (GetComponent<AudioSource>() && !GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        TargetScan.SendMessage(other.gameObject, ExitMessage);
    }
}