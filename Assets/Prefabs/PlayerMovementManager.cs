using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3
{
    public class PlayerMovementManager : Photon.PunBehaviour
    {

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public AudioClip jumpSource;
        public AudioClip landSource;
        public AudioClip takeoffSource;

        private TerrainCollider terrainCollider;
        private GameManager gameManager;

        public float timeBetweenShots = 3.0f;
        public float timeBetweenJumps = 3.0f;
        public float timestamp;
        public float jumptimestamp;
        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 1.5F;
        public float sensitivityY = 1.5F;
        public float minimumX = -360F;
        public float maximumX = 360F;
        public float minimumY = -60F;
        public float maximumY = 60F;


        public float frThrust = 50f;
        public float lrThrust = 35f;

        public int fuelPerPulse = 180;
        public int fuelPerJump = 200;

        public float destroyDelayWhenDead = 5;
        private float timeSinceDead = 0;

        [HideInInspector]
        public bool isDead = false;
        private bool isSpawning = false;

        float rotationX = 0F;
        float rotationY = 0F;
        float lastRotationX = 0F;
        float lastRotationY = 0F;
        Quaternion originalRotation;
        float jumpForce = 700f;
        float height = 1.8f; // tank's level above ground


        private bool isLanded = false;
        private bool requestLand = false;
        private float maxDistanceToLand = 0.6f; //max distance between ground and tank that allows to land

        private bool requestJump = false;

        private Rigidbody myRigidbody;

        private HitPointsManager hitpointsManager;

        public Transform myMesh;
        public Transform gunEnd;

        public float maximumHeight = 4.0f;
        private float mass = 2.0f;
        public float riseSpeed = 0.065f;
        public float lowerSpeed = 0.035f;

        // Use this for initialization
        void Start()
        {
            hitpointsManager = GetComponent<HitPointsManager>();
            gameManager = FindObjectOfType<GameManager>();
            myRigidbody = GetComponent<Rigidbody>();
            if (!photonView.isMine)
            {
                myRigidbody.isKinematic = true;
                return;
            }

            myRigidbody.mass = mass;
            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();
            originalRotation = transform.localRotation;
        }

        private void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerMovementManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        public void Reset()
        {
            if (photonView.isMine)
            {
                myRigidbody.freezeRotation = false;
                isLanded = false;
                requestLand = false;
                requestJump = false;
                timeSinceDead = 0;
                isSpawning = true;
                Camera.main.enabled = false;
                gameManager.Respawn(this);
            }
        }

        [PunRPC]
        public void SetPosAndRotation(Vector3 pos, Quaternion rot)
        {
            if (!photonView.isMine)
                return;
            CheckRespawn();
            transform.position = pos;
            transform.rotation = rot;
        }

        void CheckRespawn()
        {
            if (isSpawning)
            {
                isSpawning = false;
                myRigidbody.isKinematic = false;
                Camera.main.enabled = true;
                myMesh.gameObject.SetActive(true);
                GetComponent<Collider>().enabled = true;
                GetComponent<KGFMapIcon>().SetVisibility(true);
            }

        }

        void PrepareForRespawn()
        {
            myMesh.gameObject.SetActive(false);
            myRigidbody.isKinematic = true;
            GetComponent<Collider>().enabled = false;
            GetComponent<KGFMapIcon>().SetVisibility(false);
            gameManager.SpawnExplosion(transform.position);
            if (photonView.isMine)
            {
                Reset();
            }

        }

        void CheckIsDead()
        {
            isDead = hitpointsManager.health <= 0;
            if (isDead && !isSpawning)
            {
                if (timeSinceDead == 0)
                {
                    myRigidbody.freezeRotation = false;
                }
                timeSinceDead += Time.deltaTime;
                if (timeSinceDead >= destroyDelayWhenDead)
                {
                    PrepareForRespawn();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            CheckIsDead();

            if (!photonView.isMine || isDead || isSpawning)
                return;


            if (!Cursor.visible)
            {
                myRigidbody.freezeRotation = false;
                if (!isLanded)
                {
                    if (axes == RotationAxes.MouseXAndY)
                    {
                        // Read the mouse input axis
                        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                        rotationX = ClampAngle(rotationX, minimumX, maximumX);
                        rotationY = ClampAngle(rotationY, minimumY, maximumY);
                        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
                        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
                    }
                    else if (axes == RotationAxes.MouseX)
                    {
                        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                        rotationX = ClampAngle(rotationX, minimumX, maximumX);
                        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                        transform.localRotation = originalRotation * xQuaternion;
                    }
                    else
                    {
                        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                        rotationY = ClampAngle(rotationY, minimumY, maximumY);
                        Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                        transform.localRotation = originalRotation * yQuaternion;
                    }
                }

                //Fire Pulse
                if (!isLanded && Time.time >= timestamp && (Input.GetMouseButtonDown(1)) && this.gameObject.GetComponent<Unit>().unitType == UnitType.Tank)
                {
                    if (GetComponent<FuelManager>().TakeFuel(fuelPerPulse))
                    {
                        CmdFirePulseShell();
                        timestamp = Time.time + timeBetweenShots;
                    }
                }

                //Tank Jump
                if (!isLanded && Time.time >= jumptimestamp && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Keypad0)))
                {
                    if (GetComponent<FuelManager>().TakeFuel(fuelPerJump))
                    {
                        requestJump = true;
                        jumptimestamp = Time.time + timeBetweenJumps;
                    }
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // raise level
                    height = Mathf.Min(height + riseSpeed, maximumHeight);
                    if (isLanded && height > 0.001)
                    {
                        TakeOff();
                    }
                }

                if (!isLanded && Input.GetKey(KeyCode.LeftControl))
                {
                    // lower level
                    height = Mathf.Max(height - lowerSpeed, 0f);
                    if (height < 0.001)
                    {
                        requestLand = true;
                    }
                }
            }
            else
            {
                myRigidbody.freezeRotation = true;
            }

            if (requestLand)
            {
                if (CanLand())
                {
                    Debug.Log("CanLand true");
                    Land();
                }
                else
                {
                    Debug.Log("CanLand false");
                }
            }

        }

        private bool CanLand()
        {
            if (height > maxDistanceToLand)
            {
                return false;
            }

            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down);
            RaycastHit hit;
            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();
            return terrainCollider.Raycast(ray, out hit, 2.0f) && hit.distance <= maxDistanceToLand;
        }

        private void Land()
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down);
            RaycastHit hit;
            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();
            if (terrainCollider.Raycast(ray, out hit, 2.0f))
            {
                Vector3 fwd = transform.forward;
                Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
                transform.rotation = Quaternion.LookRotation(proj, hit.normal);
                transform.Translate(hit.point - transform.position);
                //transform.Translate(Vector3.up * 0.15f); //fixme: distance is calculated from center of the tank model, this moves the tank 'right' amount so that tank does not get clipped with the ground
                myRigidbody.isKinematic = true; //do not let physics forces affect this body
                isLanded = true;
                requestLand = false;
                AudioSource.PlayClipAtPoint(landSource, transform.position);
                GetComponent<AudioSource>().Stop();
            }
            else
            {
                //cannot land
                isLanded = false;
            }
        }

        private void TakeOff()
        {
            myRigidbody.isKinematic = false; //let physics forces affect this body again
            requestLand = false;
            isLanded = false;
            AudioSource.PlayClipAtPoint(takeoffSource, transform.position);
            GetComponent<AudioSource>().Play();
        }

        void CmdFirePulseShell()
        {
            object[] args = new object[3];
            args[0] = gunEnd.position;
            args[1] = gunEnd.rotation;
            args[2] = transform.GetComponent<Unit>().unitTeam;
            gameManager.photonView.RPC("SpawnPulseShell", PhotonTargets.MasterClient, args);
            myRigidbody.AddForce(-transform.forward * 100f);
        }

        public void FixedUpdate()
        {
            if (!photonView.isMine || isDead)
                return;

            float x = Input.GetAxis("Horizontal") * 0.1f;
            float z = Input.GetAxis("Vertical") * 0.1f;

            if (Cursor.visible)
            {
                x = 0;
                z = 0;
            }
            else
            {
                x = Input.GetAxis("Horizontal") * 0.1f;
                z = Input.GetAxis("Vertical") * 0.1f;
            }

            Ray ray = new Ray(transform.position, -Vector3.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, height))
            {
                if (hit.distance < height)
                {
                    float ftcGravity = Physics.gravity.y * myRigidbody.mass;
                    float ftcVelocity = myRigidbody.velocity.y * myRigidbody.mass;
                    float multi = (height - hit.distance) / height;
                    float force = (ftcGravity + ftcVelocity) * (multi * myRigidbody.mass);
                    myRigidbody.AddForce(new Vector3(0f, -force, 0f));
                }
            }



            /*
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down);
            RaycastHit hit;
            terrainCollider = GameObject.FindObjectOfType<TerrainCollider>();
            if (terrainCollider.Raycast(ray, out hit, height * 2)) {
                Vector3 direction = Vector3.up; //transform.up
                //Debug.Log(direction.ToString());
                //Debug.Log((direction * Mathf.Min(forceMultiplier / (Mathf.Max(hit.distance - height, 0.01f) / 2.0f), 20.0f)).ToString());
                //Debug.Log("");
                rb.AddForce(direction * Mathf.Min(forceMultiplier / (Mathf.Max(hit.distance - height, 0.01f) / 2.0f), 20.0f));


                //transform.rotation = Quaternion.LookRotation(proj, hit.normal);

                //Quaternion target = Quaternion.FromToRotation(Vector3.up, hit.normal);
                //if (z >= 0.01f)
                //{
                //    Vector3 fwd = transform.forward;
                //    Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
                //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(proj, hit.normal), 0.05f);
                //}

                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(hit.normal), 0.05f);
                Debug.DrawLine(transform.position, hit.point);

            }
            */



            //Tank Jump
            if (requestJump)
            {
                AudioSource.PlayClipAtPoint(jumpSource, transform.position);
                myRigidbody.AddForce(transform.up * jumpForce * myRigidbody.mass);
                requestJump = false;
            }

            if (isLanded && (x > 0f || z > 0f))
            {
                TakeOff();
                height = 1.2f;
            }

            myRigidbody.AddRelativeForce(new Vector3(x * lrThrust * myRigidbody.mass, 0, z * frThrust * myRigidbody.mass));
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
