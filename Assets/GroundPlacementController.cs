using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Wulfram3;

public class GroundPlacementController : Photon.PunBehaviour
{
    [SerializeField]
    public GameObject placeableObjectPrefab;
    
    [SerializeField]
    public KeyCode newObjectHotkey = KeyCode.Comma;

    public GameObject currentPlaceableObject;

    public float mouseWheelRotation;

    public Transform placeObject;

    public Material PreviewMaterial;

    public GameManager gameManager;

    public UnitType deployType;
    public PunTeams.Team deployTeam;

    private float maxPlaceDistance;
    private float minPlaceDistance;



    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        maxPlaceDistance = Vector3.Distance(transform.position, placeObject.position);
    }

    public void Update()
    {
        Debug.Log("............................................................................");
        /*
        HandleNewObjectHotkey();
        baseUnit = this.GetComponentInParent<CargoManager>().pickedUpCargo;
        if (currentPlaceableObject != null)
        {
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();
        }
        */
    }

    public void HandleNewObjectHotkey()
    {
        /*
        if (Input.GetKeyDown(newObjectHotkey) && !(baseUnit == ""))
        {
            if (currentPlaceableObject != null)
            {
                Destroy(currentPlaceableObject);
            }
            else
            {
                currentPlaceableObject = Instantiate(baseUnit, placeObject.transform.position, placeObject.transform.rotation, 0);
                MakeDummyObject(currentPlaceableObject);
                minPlaceDistance = Mathf.Ceil(currentPlaceableObject.GetComponent<Collider>().bounds.size.x, currentPlaceableObject.GetComponent<Collider>().bounds.size.z, 2.0f);
            }
        }
        */
    }

    private void MakeDummyObject()
    {
        /*
        ChangeMaterial(PreviewMaterial);
        GetComponent<Collider>().enabled = false;
        foreach (MonoBehaviour script in currentPlaceableObject.GetComponents<MonoBehaviour>(true))
            script.enabled = false;
        foreach (MonoBehaviour script in currentPlaceableObject.GetComponentsInChildren<MonoBehaviour>(true))
            script.enabled = false;
            */
    }

    public void MoveCurrentObjectToMouse()
    {
        /*
        Vector3 moveTo = Vector3.zero;
        Vector3 lookAtPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        float lookAtDistance = Vector3.Distance(transform.position, lookAtPosition);
        if (lookAtDistance <= maxPlaceDistance && lookAtDistance >= minPlaceDistance)
        {
            moveTo = lookAtPosition;
        } else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                moveTo = hitInfo.transform.position;
            } else
            {
                moveTo = placeObject.position;
            }
        }
        currentPlaceableObject.transform.position = moveTo; */

    }
    /*

    public void RotateFromMouseWheel()
    {
        //Debug.Log(Input.mouseScrollDelta);
        mouseWheelRotation += Input.mouseScrollDelta.y;
        currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
    }

    public void ChangeMaterial(Material newMat)
    {
        foreach (Renderer rend in currentPlaceableObject.GetComponentsInChildren<Renderer>())
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }

    public void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //get baseunit to deploy based on its name in the resources folder (Must match)
            
            Debug.Log(baseUnit);

            gameManager.requestCargoDeployment(currentPlaceableObject.transform.position, currentPlaceableObject.transform.rotation, deployType, deployTeam);
          
            Destroy(currentPlaceableObject);
            PhotonNetwork.Instantiate(baseUnit, placeObject.transform.position, placeObject.transform.rotation, 0);
            photonView.RPC("DroppedCargo", PhotonTargets.All);
            //this.GetComponentInParent<CargoManager>().SetPickedUpCargo("");
        }
    }
    */
}


