using Assets.Wulfram3.Scripts.InternalApis.Classes;
using UnityEngine;

namespace Com.Wulfram3
{
    public class CargoManager : Photon.PunBehaviour {


        private Transform currentPlaceableObject;

        public Transform placeObjectPoint;

        public Material ghostObjectMaterial;

        private GameManager gameManager;

        private float maxPlaceDistance;
        private float minPlaceDistance;
        public AudioClip cargoPickupSound;
        public AudioClip cargoDropSound;
        public UnitType cargoType;
        public PunTeams.Team cargoTeam;
        public bool hasCargo = false;
        public Transform dropPosition;

        private PunTeams.Team myTeam;

        private float mouseWheelRotation = 0f;
        private float rotationSpeed = 10f;

        private bool triedToDeploy = false;
        private float timeSinceTry = 0f;

        // Use this for initialization
        void Start() {
            maxPlaceDistance = Vector3.Distance(transform.position, placeObjectPoint.position);
            myTeam = GetComponent<Unit>().unitTeam;
        }

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update() {
            if (photonView.isMine) {
                if (Input.GetAxis("DropCargo") != 0f) {
                    gameManager.DropCargo(this);
                }

                if (hasCargo) {
                    if (currentPlaceableObject == null)
                    {
                        CheckDeployCargo();
                    } else
                    {
                        UpdatePosition();
                    }
                    //TODO: show animation of cargo in player HUD
                }
            }

        }

        [PunRPC]
        public void GetCargo(PunTeams.Team t, UnitType u, int viewID, PhotonMessageInfo info)
        {
            if (hasCargo)
            {
                return;
            } else
            {
                hasCargo = true;
                cargoType = u;
                cargoTeam = t;
                GetComponent<AudioSource>().PlayOneShot(cargoPickupSound, 1.0f);
                PhotonView.Find(viewID).RPC("PickedUp", PhotonTargets.MasterClient, this.photonView.viewID);
            }
        }

        [PunRPC]
        public void DroppedCargo()
        {
            GetComponent<AudioSource>().PlayOneShot(cargoDropSound, 1.0f);
            hasCargo = false;
            cargoType = UnitType.None;
            cargoTeam = myTeam;
        }

        public void CheckDeployCargo()
        {
            if (Input.GetAxis("DeployCargo") > 0f && hasCargo)
            {
                if (currentPlaceableObject != null)
                {
                    Destroy(currentPlaceableObject);
                }
                else
                {
                    string prefab = gameManager.UnitTypeToPrefabString(cargoType, myTeam);
                    Debug.Log("CargoManager.cs == Instantiating Locally. " + prefab);
                    GameObject newObject = Instantiate((GameObject) Resources.Load(prefab), GetBestPosition(), transform.rotation);
                    Debug.Log("CargoManager.cs == Instantiation Success.");
                    currentPlaceableObject = MakeDummyObject(newObject.transform);
                    minPlaceDistance = Mathf.Max(currentPlaceableObject.GetComponent<Collider>().bounds.size.x, currentPlaceableObject.GetComponent<Collider>().bounds.size.z, 2.0f);
                }
            }
        }

        private Transform MakeDummyObject(Transform t)
        {
            ChangeMaterial(ghostObjectMaterial);
            GetComponent<Collider>().enabled = false;
            foreach (MonoBehaviour script in currentPlaceableObject.GetComponents<MonoBehaviour>())
                script.enabled = false;
            foreach (MonoBehaviour script in currentPlaceableObject.GetComponentsInChildren<MonoBehaviour>())
                script.enabled = false;
            return t;
        }

        private Vector3 GetBestPosition()
        {
            Vector3 moveTo = Vector3.zero;
            Vector3 lookAtPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            float lookAtDistance = Vector3.Distance(transform.position, lookAtPosition);
            if (lookAtDistance <= maxPlaceDistance && lookAtDistance >= minPlaceDistance)
            {
                return lookAtPosition;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    return hitInfo.transform.position;
                }
                else
                {
                    return placeObjectPoint.position;
                }
            }
        }

        public void UpdatePosition()
        {
            currentPlaceableObject.transform.position = GetBestPosition();

        }

        public void RotateFromMouseWheel()
        {
            //Debug.Log(Input.mouseScrollDelta);
            mouseWheelRotation += Input.mouseScrollDelta.y;
            currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * rotationSpeed);
        }

        public void ChangeMaterial(Material newMat)
        {
            foreach (Renderer rend in currentPlaceableObject.transform.GetComponentsInChildren<Renderer>())
            {
                var mats = new Material[rend.materials.Length];
                for (var j = 0; j < rend.materials.Length; j++)
                {
                    mats[j] = newMat;
                }
                rend.materials = mats;
            }
        }

        [PunRPC]
        public void DeployedCargo()
        {
            Destroy(currentPlaceableObject);
            currentPlaceableObject = null;
            hasCargo = false;
            cargoType = UnitType.None;
            cargoTeam = myTeam;

        }

        public void ReleaseIfClicked()
        {
            if (Input.GetMouseButtonDown(0) && currentPlaceableObject != null)
            {
                //get baseunit to deploy based on its name in the resources folder (Must match)

                Debug.Log("Attempting to deploy: " + cargoType.ToString());

                object[] args = new object[4];
                args[0] = currentPlaceableObject.transform.position;
                args[1] = currentPlaceableObject.transform.rotation;
                args[2] = cargoType;
                args[3] = myTeam;
                photonView.RPC("requestDeployCargo", PhotonTargets.MasterClient, args);
                //gameManager.requestCargoDeployment(currentPlaceableObject.transform.position, currentPlaceableObject.transform.rotation, cargoType, myTeam);


                //PhotonNetwork.Instantiate(baseUnit, placeObject.transform.position, placeObject.transform.rotation, 0);
                //photonView.RPC("DroppedCargo", PhotonTargets.All);
                //this.GetComponentInParent<CargoManager>().SetPickedUpCargo("");
            }
        }
    }
}
