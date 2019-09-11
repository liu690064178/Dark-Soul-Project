using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace SA
{

    public class CameraManager : MonoBehaviour {

        public bool lockOn;
        public float followSpeed = 9f;
        public float mouseSpeed = 2f;
        public float controllerSpeed = 7f;


        public Transform target;
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        


        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public  Transform camTrans;
        StateManager states;

        float turnSmoothing = 0.1f;
        public float minAngle = -35f;
        public float maxAngle = 35f;

        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;

        public float lookAngle;
        public float tiltAngle;

        bool usedRightAxis;

        private bool changeTargetLeft;
        private bool changeTargetRight;
        
        public static CameraManager singleton;
        private void Awake()
        {
            singleton = this;
        }
        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;

            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            //Debug.Log(h);

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");
           

            float targetSpeed = mouseSpeed;

            changeTargetLeft = Input.GetKeyUp(KeyCode.V);
            changeTargetRight = Input.GetKeyUp(KeyCode.B);
            
            if (lockOnTarget != null)
            {

                if(lockOnTransform == null)
                {
                    lockOnTransform = lockOnTarget.GetTarget();
                    states.lockOnTransform = lockOnTransform;
                }
             //   if (Mathf.Abs(c_h) > 0.6f)
                if(Input.GetKeyUp(KeyCode.Tab))
                {
                    if (!usedRightAxis)
                    {
                        lockOnTransform = lockOnTarget.GetTarget((c_h>0));
                        states.lockOnTransform = lockOnTransform;
                        
                        usedRightAxis = true;
                    }

                    if (changeTargetLeft || changeTargetRight)
                    {
                        lockOnTransform = lockOnTarget.GetTarget(changeTargetLeft);
                        states.lockOnTransform = lockOnTransform;
                    
                    }
                }
            }  
            if (usedRightAxis)
            {
                if (Mathf.Abs(c_h) < 0.6f)
                {
                    usedRightAxis = false;
                }
                
            }
            

            if(c_h != 0 || c_v != 0) 
            {
                h = c_h;
                v = -c_v;
                targetSpeed = controllerSpeed;
            }
            FlollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);

        }

        void FlollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d,float v,float h,float targetSpeed)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

        

            if (lockOn && lockOnTarget!=null)
            {
                Vector3 targetDir = lockOnTransform.position - transform.position;
                targetDir.Normalize();
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion lookTarget = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookTarget, d * 9f);
                lookAngle = transform.eulerAngles.y;

                return;
            }
         

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);
        }
    }

}
