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

        private GameManager gameManager;

        [Tooltip("In Seconds")]
        public float timeBetweenShots = 3.0f;
        [Tooltip("In Seconds")]
        public float timeBetweenJumps = 3.0f;

        private float timestamp;
        private float jumptimestamp;
        private float landRequestTime;

        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 1.5F;
        public float sensitivityY = 1.5F;
        public float minimumX = -360F;
        public float maximumX = 360F;
        public float minimumY = -60F;
        public float maximumY = 60F;


        public float frThrust = 5f;
        public float lrThrust = 3f;

        private float takeOffBumpForce = 20f;

        public int fuelPerPulse = 180;
        public int fuelPerJump = 200;

        private float destroyDelayWhenDead = 3;
        private float timeSinceDead = 0;


        [HideInInspector]
        public bool isDead = false;
        [HideInInspector]
        public bool isSpawning = false;

        private float rotationX = 0F;
        private float rotationY = 0F;
        private float lastRotationX = 0F;
        private float lastRotationY = 0F;
        private Quaternion originalRotation;
        private float jumpForce = 700f;
        private float height = 1.8f;          // tank's level above ground
        private bool isLanded = false;
        private bool isGrounded = false;
        private bool requestLand = false;
        private bool requestJump = false;
        private float maxDistanceToLand = 0.6f; //max distance between ground and tank that allows to land

        private Rigidbody myRigidbody;
        private HitPointsManager hitpointsManager;

        public Transform myMesh;
        public Transform gunEnd;

        public float maximumHeight = 4.0f;
        public float riseSpeed = 0.055f;
        public float lowerSpeed = 0.025f;

        //private float mass = 2.0f;
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

            //myRigidbody.mass = mass;
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
                isGrounded = false;
                requestLand = false;
                requestJump = false;
                timeSinceDead = 0;
                isSpawning = true;
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
                myMesh.gameObject.SetActive(true);
                GetComponent<Collider>().enabled = true;
                GetComponent<KGFMapIcon>().SetVisibility(true);
            }

        }

        public void PrepareForRespawn()
        {
            myMesh.gameObject.SetActive(false);
            myRigidbody.isKinematic = true;
            GetComponent<Collider>().enabled = false;
            GetComponent<KGFMapIcon>().SetVisibility(false);
            //gameManager.SpawnExplosion(transform.position);
            if (photonView.isMine)
            {
                Reset();
            }

        }

        public void CheckIsDead()
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
                if (!isGrounded)
                {
                    if (axes == RotationAxes.MouseXAndY)
                    {
                        if (isLanded) { isLanded = false; }
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
                        if (isLanded) { isLanded = false; }
                        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                        rotationX = ClampAngle(rotationX, minimumX, maximumX);
                        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                        transform.localRotation = originalRotation * xQuaternion;
                    }
                    else
                    {
                        if (isLanded) { isLanded = false; }
                        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                        rotationY = ClampAngle(rotationY, minimumY, maximumY);
                        Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                        transform.localRotation = originalRotation * yQuaternion;
                    }
                }

                //Fire Pulse
                if (this.gameObject.GetComponent<Unit>().unitType == UnitType.Tank && Time.time >= timestamp && Input.GetAxisRaw("Fire2") != 0)
                {
                    if (GetComponent<FuelManager>().TakeFuel(fuelPerPulse))
                    {
                        CmdFirePulseShell();
                        timestamp = Time.time + timeBetweenShots;
                    }
                }

                // Altitude control, before jump to allow cancel of landing attempts by jumping
                if (Input.GetAxisRaw("ChangeAltitude") > 0)
                {
                    height = Mathf.Min(height + riseSpeed, maximumHeight);
                }
                else if (Input.GetAxisRaw("ChangeAltitude") < 0 && !isLanded && !isGrounded)
                {
                    height = Mathf.Max(height - lowerSpeed, 0f);
                }
                if (height > 0.001 && (isLanded || isGrounded))
                {
                    TakeOff();
                } else if (height <= 0.001 && !isLanded)
                {
                    isLanded = true;
                }

                // Tank Jump
                if (Time.time >= jumptimestamp && (Input.GetAxisRaw("Jump") != 0 || Input.GetKeyDown(KeyCode.Keypad0)))
                {
                    if (GetComponent<FuelManager>().TakeFuel(fuelPerJump))
                    {
                        if (isLanded || isGrounded)
                        {
                            TakeOff();
                        }
                        requestJump = true;
                        jumptimestamp = Time.time + timeBetweenJumps;
                    }
                }

                if (isLanded)
                {
                    CheckIsGrounded();
                    if (isGrounded)
                    {
                        Land();
                    }
                }

            }
            else
            {
                myRigidbody.freezeRotation = true;
            }


        }


        private void CheckIsGrounded()
        {
            isGrounded = Physics.Raycast(new Ray(transform.position, Vector3.down), maxDistanceToLand);
        }

        private void Land()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 fwd = transform.forward;
                Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
                transform.rotation = Quaternion.LookRotation(proj, hit.normal);
                transform.Translate(hit.point - transform.position);
                transform.Translate(Vector3.up * 0.15f);
                myRigidbody.isKinematic = true; //do not let physics forces affect this body
                AudioSource.PlayClipAtPoint(landSource, transform.position);
                GetComponent<AudioSource>().Stop();
            }
        }

        private void TakeOff()
        {
            myRigidbody.isKinematic = false; //let physics forces affect this body again
            isLanded = false;
            isGrounded = false;
            height = 1.8f;
            myRigidbody.AddForce(transform.up * (takeOffBumpForce * myRigidbody.mass));
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

            float x = Input.GetAxis("Strafe");
            float z = Input.GetAxis("Drive");

            if (Cursor.visible)
            {
                x = 0;
                z = 0;
            }


            if (!isLanded && !isGrounded)
            {
                /*
                 *  Start Hover Code
                 *  - This could likely be moved to a separate script for use on other things
                 *  - Provides mass relative upward force needed to maintain {height}
                 *  - TODO: Could handle high velocities a little better
                 */
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
                // End Hover Code
            }

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
