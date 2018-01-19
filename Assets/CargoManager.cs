using Assets.Wulfram3.Scripts.InternalApis.Classes;
using UnityEngine;

namespace Com.Wulfram3
{
    public class CargoManager : Photon.PunBehaviour {

        private GameManager gameManager;
        private Transform currentPlaceableObject;
        private float maxPlaceDistance;
        private float minPlaceDistance;

        private float mouseWheelRotation = 0f;
        private float rotationSpeed = 10f;

        private float dropDelay = 0f;
        private float deployDelay = 0f;

        public Transform placeObjectPoint;
        public Material ghostObjectMaterial;

        public AudioClip cargoPickupSound;
        public AudioClip cargoDropSound;
        public UnitType cargoType;
        public PunTeams.Team cargoTeam;
        public bool hasCargo = false;
        public bool isDeploying = false;
        public Transform dropPosition;

        public PunTeams.Team myTeam;


        void Start() {
            gameManager = FindObjectOfType<GameManager>();
            maxPlaceDistance = Vector3.Distance(transform.position, placeObjectPoint.position);
            myTeam = GetComponent<Unit>().unitTeam;
        }

        void Update() {
            if (photonView.isMine) {
                if (Input.GetAxisRaw("DropCargo") != 0f && Time.time > dropDelay) {
                    if (currentPlaceableObject != null)
                        CancelDeployCargo();
                    dropDelay = Time.time + 0.2f;
                    gameManager.photonView.RPC("RequestDropCargo", PhotonTargets.MasterClient, photonView.viewID);
                }

                if (hasCargo) {
                    if (Input.GetAxisRaw("DeployCargo") != 0f && Time.time > deployDelay) {
                        deployDelay = Time.time + 0.2f;
                        ToggleDeployMode();
                    }
                    if (currentPlaceableObject != null)
                    {
                        currentPlaceableObject.transform.position = GetBestPosition();
                        RotateFromMouseWheel();
                        if (Input.GetMouseButtonDown(0))
                            CheckFinalPlacement();
                    }
                    //TODO: show animation of cargo in player HUD
                } else
                {
                    if (Time.time > deployDelay)
                    {
                        isDeploying = false;
                    }
                }
            }

        }

        private void ToggleDeployMode()
        {
            if (currentPlaceableObject == null)
            {
                StartDeployCargo();
            }
            else
            {
                CancelDeployCargo();
            }
        }

        [PunRPC]
        public void GetCargo(PunTeams.Team t, UnitType u, int viewID)
        {
            hasCargo = true;
            cargoType = u;
            cargoTeam = t;
            GetComponent<AudioSource>().PlayOneShot(cargoPickupSound, 1.0f);
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
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

        public void CancelDeployCargo() {
            if (currentPlaceableObject != null)
            {
                Destroy(currentPlaceableObject.gameObject);
                currentPlaceableObject = null;
                isDeploying = false;
            }

        }

        public void StartDeployCargo()
        {
            if (Input.GetAxis("DeployCargo") > 0f && hasCargo && currentPlaceableObject == null)
            {
                isDeploying = true;
                string prefab = Unit.GetPrefabName(cargoType, myTeam);
                GameObject newObject = Instantiate(Resources.Load(prefab), GetBestPosition(), transform.rotation) as GameObject;
                newObject.GetComponent<Rigidbody>().isKinematic = true;
                newObject.GetComponent<Collider>().enabled = false;
                foreach (var script in newObject.GetComponents<MonoBehaviour>())
                    script.enabled = false;
                foreach (var script in newObject.GetComponentsInChildren<MonoBehaviour>())
                    script.enabled = false;
                currentPlaceableObject = newObject.transform;
                ChangeMaterial(ghostObjectMaterial);
                minPlaceDistance = Mathf.Max(currentPlaceableObject.GetComponent<Collider>().bounds.size.x, currentPlaceableObject.GetComponent<Collider>().bounds.size.z, 2.0f);
                maxPlaceDistance += minPlaceDistance;
            }
        }

        private Vector3 GetBestPosition()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, maxPlaceDistance))
                return new Vector3(hit.point.x, hit.point.y + (minPlaceDistance * 0.5f), hit.point.z);
            return new Vector3(placeObjectPoint.position.x, placeObjectPoint.position.y + (minPlaceDistance * 0.85f), placeObjectPoint.position.z);
        }


        public void RotateFromMouseWheel()
        {
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
            if (currentPlaceableObject != null)
            {
                Destroy(currentPlaceableObject.gameObject);
                currentPlaceableObject = null;
            }
            deployDelay = Time.time + 0.4f;
            hasCargo = false;
            cargoType = UnitType.None;
            cargoTeam = myTeam;

        }

        public void CheckFinalPlacement()
        {
            if (Input.GetMouseButtonDown(0) && currentPlaceableObject != null)
            {
                object[] args = new object[5];
                args[0] = currentPlaceableObject.transform.position;
                args[1] = currentPlaceableObject.transform.rotation;
                args[2] = this.cargoType;
                args[3] = this.cargoTeam;
                args[4] = this.photonView.viewID;
                gameManager.photonView.RPC("RequestDeployCargo", PhotonTargets.MasterClient, args);
            }
        }
    }
}
