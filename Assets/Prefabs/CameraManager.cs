using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.Wulfram3 {
    public class CameraManager : Photon.PunBehaviour {
        private Vector3 thirdPersonPos = new Vector3(0, 1.5f, -3.5f);
        private Vector3 firstPersonPos = new Vector3(0, 1.2f, -0.5f);//new Vector3(0, 0.33f, -0.3f);

        //public Transform firstPersonT;
        //public Transform thirdPersonT;

        public float transitionTime = 1.0f;

        private Camera cam;
        private Vector3 currentPos;
        private Vector3 targetPos;
        private float transitionStartTime;
        private bool transitionComplete = true;
        private bool needsReset = false;

        // Use this for initialization
        void Start() {
            if (photonView.isMine) {
                targetPos = firstPersonPos;
                currentPos = targetPos;
                //Debug.Log(currentPos);
                cam = Camera.main;
                cam.transform.SetParent(transform);
                cam.transform.localPosition = currentPos;
                cam.transform.rotation = Quaternion.identity;
            }
        }

		public void Detach(){
			cam.transform.SetParent(null);

		}

        public void SetFirstPersonPosition(Transform t)
        {
            firstPersonPos = transform.InverseTransformPoint(t.position);
            needsReset = true;
        }

        public void SetThirdPersonPosition(Transform t)
        {
            thirdPersonPos = transform.InverseTransformPoint(t.position);
            needsReset = true;
        }

        // Update is called once per frame
        void Update() {
            if (photonView.isMine) {
                if (needsReset)
                {
                    needsReset = false;
                    transitionComplete = false;
                    targetPos = firstPersonPos;
                    transitionStartTime = Time.time;
                }
                if (Input.GetKeyDown(KeyCode.F2) && !Cursor.visible) {
                    SwapTargetpos();
                    transitionStartTime = Time.time;
                    transitionComplete = false;
                }
                if (!transitionComplete) {
                    float fracComplete = (Time.time - transitionStartTime) / transitionTime;
                    if (fracComplete >= 1.0f) {
                        transitionComplete = true;
                        currentPos = targetPos;
                    } else {
                        currentPos = Vector3.Slerp(currentPos, targetPos, fracComplete);
                    }
                    cam.transform.localPosition = currentPos;
                }

            }
        }

        private void SwapTargetpos() {
            if (targetPos.Equals(thirdPersonPos)) {
                targetPos = firstPersonPos;
            } else {
                targetPos = thirdPersonPos;
            }
        }
    }
}
