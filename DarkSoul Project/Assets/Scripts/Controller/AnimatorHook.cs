using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SA
{

    public class AnimatorHook : MonoBehaviour {

        Animator anim;
        StateManager states;
        private EnemyStates eStates;
        private Rigidbody rigid;

        public float rm_multi;
        public bool rolling;
        public AnimationCurve roll_curve;
    
        float roll_t;
        private float delta;
        

        public void Init(StateManager st,EnemyStates eSt = null)
        {
            states = st;
            eStates = eSt;
            if (eSt != null)
            {
                anim = eSt.anim;
                rigid = eSt.rigid;
                delta = eStates.delta;
            }

            if (st != null)
            {
                anim = st.anim;
                rigid = st.rigid;
                roll_curve = states.roll_curve;
                delta = states.delta;
            }   
          
        }
        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0;
        } 
        public void CloseRoll()
        {
            if (!rolling)
                return;
            rm_multi = 1;
            roll_t = 0;
            rolling = false;
        }

        private void OnAnimatorMove()
        {
            if (states == null && eStates ==null)
                return;

            if (rigid == null)
                return;

            if (states != null)
            {
                if (states.canMove )
                    return;
                delta = states.delta;
            }


            if (eStates != null)
            {
                if (eStates.canMove)
                    return;

                delta = eStates.delta;
            }
              
            
            rigid.drag = 0;
        
            if (rm_multi == 0)
                rm_multi = 1;

            if (rolling == false)
            {
                Vector3 delta2 = anim.deltaPosition;
                delta2.y = 0;
                Vector3 v = (delta2 * rm_multi) / delta;
                rigid.velocity = v;
            }
            else
            {
                //待理解
                roll_t += delta/0.6f;
                if (roll_t > 1)
                {
                    roll_t = 1;
                }
                float zValue = roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_multi);// states.delta;
                rigid.velocity = v2;
            }
        }



        public void OpenDamageColliders()
        {
            if (states)
            {
            states.inventoryManager.OpenAllDamageColliders();
                
            }
            OpenParryFlag();
        }

        public void CloseDamageColliders()
        {
            if (states)
            {
            states.inventoryManager.CloseAllDamageCollider();
                
            }
            CloseParryFlag();
        }

        public void CloseParryCollider()
        {
            if (states == null)
                return; 
            states.inventoryManager.CloseParryCollider();
        }
        
        public void OpenParryCollider()
        {
            if (states == null)
                return;
            states.inventoryManager.OpenParryCollider();
        }

        public void OpenParryFlag()
        {
            if (states)
            {
                states.parryIsOn = true;
            }

            if (eStates)
            {
                eStates.parryIsOn = true;
            }
        }
         
        public void CloseParryFlag()
        {
            if (states)
            {
                states.parryIsOn = false;
            }

            if (eStates)
            {
                eStates.parryIsOn = false;
            }
        }
        
        
        public void LateTick()
        {

        }
    }
}

