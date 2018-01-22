using UnityEngine;


/// <summary>
/// The visual part of a taget.
/// Listens to TargetState messages
/// </summary>
[AddComponentMenu("Targeting/TargetVisual")]
public class TargetVisual : MonoBehaviour
{
    /// <summary>
    /// Meshes to color
    /// </summary>
    public MeshRenderer[] Meshes;

    /// <summary>
    /// NameTags to write name
    /// </summary>
    public TextMesh[] TextMesh;

    /// <summary>
    /// Activated on visibility
    /// </summary>
    public GameObject VisibleVisual;

    /// <summary>
    /// Activated on selection
    /// </summary>
    public GameObject SelectedVisual;

    //cache to prevent excessive updates
    protected Color LastColor;
    //cache to prevent excessive updates
    protected string LastName;
    //cache to prevent excessive updates
    protected bool lastVisible;
    //cache to prevent excessive updates
    protected bool lastSelected;

    void OnEnable()
    {
        if (SelectedVisual)
            SelectedVisual.SetActive(false);
        if (VisibleVisual)
            VisibleVisual.SetActive(false);
    }

    void OnDisable()
    {
        if (SelectedVisual)
            SelectedVisual.SetActive(false);
        if (VisibleVisual)
            VisibleVisual.SetActive(false);
    }


    /// <summary>
    /// Sent from Target
    /// </summary>
    /// <param name="state"></param>
    void OnTargetState(TargetState state)
    {
        // update componenets

        //check if changed
        if (LastColor != state.Color)
        {
            foreach (var texture in Meshes)
            {
                // so fades are not reset
                var color = new Color(state.Color.r, state.Color.g, state.Color.b, texture.material.color.a);

                texture.material.SetColor("_TintColor", color);
                texture.material.SetColor("_MainColor", color);
                texture.material.color = color;
            }

            LastColor = state.Color;
        }

        //check if changed
        if (LastName != state.Name)
        {
            foreach (var tm in TextMesh)
            {
                tm.text = state.Name;
            }

            LastName = state.Name;
        }

        //check if changed
        if (lastVisible != state.Visible)
        {
            if (VisibleVisual)
            {
                VisibleVisual.SetActive(state.Visible);
            }
            lastVisible = state.Visible;
        }

        //check if changed
        if (lastSelected != state.Selected)
        {
            if (VisibleVisual)
            {
                SelectedVisual.SetActive(state.Selected);
            }
            lastSelected = state.Selected;
        }
    }
    
    /// <summary>
    /// find child componenets helper
    /// </summary>
    [ContextMenu("FindMeshesHelper")]
    public void FindMeshesHelper()
    {
        Meshes = GetComponentsInChildren<MeshRenderer>();
        TextMesh = GetComponentsInChildren<TextMesh>();
    }
}