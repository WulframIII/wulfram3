using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3
{
    public class PlayerMovementManager : Photon.PunBehaviour
    {
        // Inspector Vars
        public AudioClip jumpSource;
        public AudioClip landSource;
        public AudioClip takeoffSource;
        public Transform myMesh;
        public Transform gunEnd;

        [Tooltip("Main Thrust Power")]
        private float baseThrust = 5f;
        [Tooltip("Strafe Thrust = Main Thrust * Strafe Percent")]
        public float strafePercent = 0.8f;
        [Tooltip("User Controlled, Also Starting Thrust Multiplier")]
        public float thrustMultiplier = 0.5f;
        public float jumpForce = 16f;
        public int fuelPerJump = 200;
        public float timeBetweenJumps = 3.0f;

        public int fuelPerPulse = 180;
        public float timeBetweenShots = 3f;
        public float pulseShellFiringImpulse = 8f;

        private float maxVelocityX = 8f;
        private float maxVelocityZ = 12f;
        private float boostMultiplier = 2.15f;
        private int minPropulsionFuel = 40;

        [Tooltip("User Controlled, Also Starting Height")]
        public float currentHeight = 1.2f;  // tank current (and starting) level above ground
        [Tooltip("Height assumed when taking off from landed")]
        public float defaultHeight = 1.2f;
        public float maximumHeight = 4.0f;
        public float riseSpeed = 0.055f;
        public float lowerSpeed = 0.025f;

        [Tooltip("Delay, in seconds, before respawn map appears")]
        public float destroyDelayWhenDead = 3;

        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 1.5F;
        public float sensitivityY = 1.5F;
        public float minimumX = -360F;
        public float maximumX = 360F;
        public float minimumY = -60F;
        public float maximumY = 60F;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        // Internal vars
        private GameManager gameManager;
        private float timestamp;
        private float jumptimestamp;
        private float landRequestTime;
        private float thrustStamp;
        private float propulsionStamp;
        private float timeSinceDead = 0;
        private float strafeThrust = 1f;
        private float takeOffBumpForce = 0.5f; // A little extra umph
        private float rotationX = 0F;
        private float rotationY = 0F;
        private float lastRotationX = 0F;
        private float lastRotationY = 0F;
        private float inputX = 0f;
        private float inputZ = 0f;
        [HideInInspector]
        public bool isDead = false;
        [HideInInspector]
        public bool isSpawning = false;
        private bool isLanded = false; // User controlled, request to land
        private bool isGrounded = false; // Script controlled, positive ground contact
        private bool requestJump = false;
        private bool boosting = false;
        private float boost = 1.0f;
        private Quaternion originalRotation;
        private Rigidbody myRigidbody;
        private HitPointsManager hitpointsManager;
        private FuelManager fuelManager;
        [HideInInspector]
        public Transform onRepairPad;

        void Start()
        {
            if (!photonView.isMine)
            {
                myRigidbody.isKinematic = true;
                return;
            }
            strafeThrust = strafePercent * baseThrust;
            originalRotation = transform.localRotation;
        }

        private void Awake()
        {
            hitpointsManager = GetComponent<HitPointsManager>();
            gameManager = FindObjectOfType<GameManager>();
            myRigidbody = GetComponent<Rigidbody>();
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerMovementManager.LocalPlayerInstance = this.gameObject;
                fuelManager = GetComponent<FuelManager>();
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        public bool GetIsGrounded()
        {
            return isGrounded;
        }

        public void Reset()
        {
            if (photonView.isMine)
            {
                if (myRigidbody != null)
                    myRigidbody.freezeRotation = false;
                
                isLanded = false;
                isGrounded = false;
                requestJump = false;
                timeSinceDead = 0;
                isSpawning = true;
                currentHeight = defaultHeight;
                GetComponent<AudioSource>().Stop();
                gameManager.Respawn(this);
            }
        }

        [PunRPC]
        public void SetPosAndRotation(Vector3 pos, Quaternion rot)
        {
            CheckRespawn();
            if (!photonView.isMine)
                return;
            transform.position = pos;
            transform.rotation = rot;
        }

        void CheckRespawn()
        {
            myMesh.gameObject.SetActive(true);
            GetComponent<AudioSource>().Play();
            GetComponent<Collider>().enabled = true;
            GetComponent<KGFMapIcon>().SetVisibility(true);
            if (isSpawning)
            {
                isSpawning = false;
                myRigidbody.isKinematic = false;
            }

        }

        public void PrepareForRespawn()
        {
            myMesh.gameObject.SetActive(false);
            if (myRigidbody != null)
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
                myRigidbody.isKinematic = false;
                float mx = Input.GetAxis("Mouse X");
                float my = Input.GetAxis("Mouse Y");
                if ((isGrounded || isLanded) && mx + my > 0)
                {
                    TakeOff();
                }
                if (!isGrounded)
                {
                    if (axes == RotationAxes.MouseXAndY)
                    {
                        // Read the mouse input axis
                        rotationX += mx * sensitivityX;
                        rotationY += my * sensitivityY;
                        rotationX = ClampAngle(rotationX, minimumX, maximumX);
                        rotationY = ClampAngle(rotationY, minimumY, maximumY);
                        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
                        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
                    }
                    else if (axes == RotationAxes.MouseX)
                    {
                        rotationX += mx * sensitivityX;
                        rotationX = ClampAngle(rotationX, minimumX, maximumX);
                        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                        transform.localRotation = originalRotation * xQuaternion;
                    }
                    else
                    {
                        rotationY += my * sensitivityY;
                        rotationY = ClampAngle(rotationY, minimumY, maximumY);
                        Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                        transform.localRotation = originalRotation * yQuaternion;
                    }
                }
                inputX = Input.GetAxis("Strafe");
                inputZ = Input.GetAxis("Drive");
                boosting = Input.GetKeyDown(KeyCode.LeftShift);
                if (Time.time >= thrustStamp)
                {
                    if (Input.GetAxisRaw("ChangeThrust") > 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = Mathf.Clamp(thrustMultiplier + 0.01f, 0.1f, 1f);
                    }
                    else if (Input.GetAxisRaw("ChangeThrust") < 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = Mathf.Clamp(thrustMultiplier - 0.01f, 0.1f, 1f);
                    } else if (Input.GetAxisRaw("SetSpeed1") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.1f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed2") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.2f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed3") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.3f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed4") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.4f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed5") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.5f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed6") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.6f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed7") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.7f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed8") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.8f;
                    }
                    else if (Input.GetAxisRaw("SetSpeed9") != 0)
                    {
                        thrustStamp = Time.time + 0.3f;
                        thrustMultiplier = 0.9f;
                    }
                }

                //Fire Pulse
                if (this.gameObject.GetComponent<Unit>().unitType == UnitType.Tank && Time.time >= timestamp && Input.GetAxisRaw("Fire2") != 0)
                {
                    if (fuelManager.TakeFuel(fuelPerPulse))
                    {
                        CmdFirePulseShell();
                        timestamp = Time.time + timeBetweenShots;
                    }
                }

                // Altitude control, before jump to allow cancel of landing attempts by jumping
                if (Input.GetAxisRaw("ChangeAltitude") > 0)
                {
                    currentHeight = Mathf.Min(currentHeight + riseSpeed, maximumHeight);
                }
                else if (Input.GetAxisRaw("ChangeAltitude") < 0 && !isLanded && !isGrounded)
                {
                    currentHeight = Mathf.Max(currentHeight - lowerSpeed, 0f);
                }
                if (currentHeight > 0.001 && (isLanded || isGrounded))
                {
                    TakeOff();
                } else if (currentHeight <= 0.001 && !isLanded)
                {
                    isLanded = true;
                }

                // Tank Jump
                if (Time.time >= jumptimestamp && (Input.GetAxisRaw("Jump") != 0 || Input.GetKeyDown(KeyCode.Keypad0)))
                {
                    if (fuelManager.TakeFuel(fuelPerJump))
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
                    inputX = 0;
                    inputZ = 0;
                    RaycastHit hit;
                    if (!isGrounded && Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, GetComponent<Collider>().bounds.extents.z * 1.15f))
                        Land(hit);
                }

            }
            else
            {
                myRigidbody.freezeRotation = true;
                myRigidbody.isKinematic = true;
            }


        }

        private Vector3 CenteredLowestPoint()
        {
            return new Vector3(transform.position.x, GetComponent<Collider>().bounds.min.y, transform.position.z);
        }

        private void Land(RaycastHit hit)
        {
            if (hit.transform != null)
            {
                Unit u = hit.transform.GetComponent<Unit>();
                if (u != null && u.unitType == UnitType.RepairPad && u.unitTeam == GetComponent<Unit>().unitTeam)
                {
                    Physics.IgnoreCollision(hit.transform.GetComponent<Collider>(), GetComponent<Collider>(), true);
                    onRepairPad = hit.transform;
                }
            }
            /*
             *  TODO: Make this into an "animation" (Lerp)
            Vector3 fwd = transform.forward;
            Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
            transform.rotation = Quaternion.LookRotation(proj, hit.normal);
            */
            transform.Translate(hit.point - CenteredLowestPoint());
            //transform.Translate(Vector3.up * 0.03f); // prevent collision overlap TODO: smoothly move into position
            myRigidbody.freezeRotation = true;
            myRigidbody.isKinematic = true; //do not let physics forces affect this body
            isGrounded = true;
            AudioSource.PlayClipAtPoint(landSource, transform.position);
            GetComponent<AudioSource>().Stop();
        }

        private void TakeOff()
        {
            if (onRepairPad != null)
            {
                Physics.IgnoreCollision(onRepairPad.GetComponent<Collider>(), GetComponent<Collider>(), false);
                onRepairPad = null;
            }
            myRigidbody.isKinematic = false; //let physics forces affect this body again
            myRigidbody.freezeRotation = false;
            isLanded = false;
            isGrounded = false;
            currentHeight = defaultHeight;
            myRigidbody.AddForce(Vector3.up * (takeOffBumpForce * myRigidbody.mass), ForceMode.Impulse);
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
            myRigidbody.AddForce(-transform.forward * pulseShellFiringImpulse, ForceMode.Impulse);
        }

        public void FixedUpdate()
        {
            if (!photonView.isMine || isDead)
                return;


            if (!isLanded && !isGrounded)
            {
                /*
                 *  Start Hover Code
                 *  - This could likely be moved to a separate script for use on other things
                 *  - Provides mass relative upward force needed to maintain {height}
                 *  - TODO: Could handle high velocities a little better
                 */
                //Ray ray = new Ray(transform.position, -Vector3.up);
                //Debug.Log(rotationX + " " + rotationY);
                Ray ray = new Ray(CenteredLowestPoint(), -Vector3.up);
                Ray checkRay = new Ray(transform.position, -Vector3.up); // This double check helps make sure the tank doesn't get stuck in the ground
                RaycastHit hit;
                RaycastHit groundCheck;
                if (Physics.Raycast(ray, out hit, currentHeight) || (Physics.Raycast(checkRay, out groundCheck, currentHeight) && groundCheck.distance != 0 && hit.distance == 0))
                {
                    if (hit.distance < currentHeight)
                    {
                        float ftcGravity = Physics.gravity.y * myRigidbody.mass;
                        float ftcVelocity = myRigidbody.velocity.y * myRigidbody.mass;
                        float multi = (currentHeight - hit.distance) / currentHeight;
                        float force = (ftcGravity + ftcVelocity) * (multi * myRigidbody.mass);
                        myRigidbody.AddForce(new Vector3(0f, -force, 0f));
                    }
                }
                // End Hover Code
            }

            // Tank Jump
            if (requestJump)
            {
                AudioSource.PlayClipAtPoint(jumpSource, transform.position);
                myRigidbody.AddForce(transform.up * jumpForce * myRigidbody.mass, ForceMode.Impulse);
                requestJump = false;
            }

            // Propulsion
            if (isLanded && (inputX != 0f || inputZ != 0f))
                TakeOff();
            boost = 1f;
            if (boosting)
                boost = boostMultiplier;
            float localXVelocity = Mathf.Abs(transform.InverseTransformDirection(myRigidbody.velocity).x);
            float localZVelocity = Mathf.Abs(transform.InverseTransformDirection(myRigidbody.velocity).z);
            float localXLimit = maxVelocityX * boost * thrustMultiplier;
            float localZLimit = maxVelocityZ * boost * thrustMultiplier;
            Vector3 relativeFwd = Vector3.Cross(Vector3.up, transform.right);
            if (localXVelocity > localXLimit)
                inputX = 0;
            if (localZVelocity > localZLimit)
                inputZ = 0;
            Vector3 totalSidewaysForce = transform.right * inputX * strafeThrust * myRigidbody.mass * boost;
            Vector3 totalForwardForce = relativeFwd * inputZ * baseThrust * myRigidbody.mass * boost;
            myRigidbody.AddForce(-totalForwardForce);
            myRigidbody.AddForce(totalSidewaysForce);
            //myRigidbody.AddRelativeForce(new Vector3((x * strafeThrust * myRigidbody.mass) * thrustMultiplier, 0, (z * baseThrust * myRigidbody.mass) * thrustMultiplier));
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
