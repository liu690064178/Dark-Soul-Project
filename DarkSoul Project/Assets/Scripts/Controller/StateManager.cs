using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Experimental.PlayerLoop;

namespace SA
{
    public class StateManager : MonoBehaviour
    {

        [Header("Init")]
        public GameObject activeModel;


        [Header("Stats")] 
        public Attributes attributes;

        public CharacterStats characterStats;
        
        
        [Header("inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb, b, a, x, y;
        public bool twoHanded;
        public bool rollInput;
        public bool itemInput;

        [Header("Stats")]
        public float moveSpeed = 2f;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5f;
        public float toGround = 0.5f;
        public float rollSpeed = 1f;
        public float parryOffset = 1.4f;
        public float backStabOffset = 1.4f;

        [Header("States")]
        public bool onGround;
        public bool run;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        float _actionDelay;
        public bool isTwoHanded;
        public bool usingItem;
        public bool canBeParried;
        public bool parryIsOn;
        public bool isBlocking;
        public bool isLeftHand;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;
     //   public EnemyStates parryTarget;
        

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;



        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        [HideInInspector]
        public Action currentAction;
        
        public void Init()
        {

            //AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync("");

            //request.assetBundle.LoadAsset<GameObject>("");
            
           
            

            SetUpAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4f;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init(this);

            a_hook = activeModel.GetComponent<AnimatorHook>();
            if(a_hook==null)
                a_hook = activeModel.AddComponent<AnimatorHook>();
            
            a_hook.Init(this);

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            //待理解
            gameObject.layer = 8;
            
            ignoreLayers = ~(1 << 9);
            anim.SetBool(StaticStrings.onGround, true);
        }

        void SetUpAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }
            if (anim == null)
                anim = activeModel.GetComponent<Animator>();
            anim.applyRootMotion = false;
        }


        public void FixedTick(float d)
        {
            delta = d;

            isBlocking = false;
            usingItem = anim.GetBool(StaticStrings.interacting);
            
            DetectAction();
            DetectItemAction();
            inventoryManager.rightHandWeapon.instance.weaponModel.SetActive(!usingItem);
            
            anim.SetBool(StaticStrings.blocking,isBlocking);
            anim.SetBool(StaticStrings.isLeft,isLeftHand);
            
            if (inAction)
            {
                anim.applyRootMotion = true;
                _actionDelay += delta;
                if (_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;

                }
            }
            canMove = anim.GetBool(StaticStrings.canMove);
            if (!canMove)
                return;

            // a_hook.rm_multi = 1;
            a_hook.CloseRoll();

            HandleRolls();

            anim.applyRootMotion = false;

            //inAction = !anim.GetBool("canMove");
            //canMove = anim.GetBool("canMove");
            //if (!canMove)
            //{
            //    rigid.velocity = Vector3.zero;
            //    return;

            //}




            rigid.drag = (moveAmount > 0 || onGround) ? 0 : 4;


            float targetSpeed = moveSpeed;

            if (usingItem)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0f, 0.45f);
            }

            if (run)
                targetSpeed = runSpeed;
            if (onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;

            //自由模式和锁定目标的移动
            Vector3 targetDir = (lockOn == false ?
                                    moveDir :
                                     (lockOnTransform!=null?
                                        lockOnTransform.position - transform.position:
                                      moveDir));
            targetDir.y = 0f;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            anim.SetBool(StaticStrings.lockOn, lockOn);

            if (lockOn)
            {
                HandleLockOnAnimations(moveDir);
            }
            else
            {
                HandleMovementAnimations();

            }


        }

        public void DetectItemAction()
        {

            if (!canMove||usingItem)
                return;
            if (!itemInput)
                return;


            ItemAction slot =actionManager.consumableItem;

            string targetAnim = slot.targetAnim;
            if (string.IsNullOrEmpty(targetAnim))
                return;
          //  inventoryManager.curWeapon.weaponModel.SetActive(false);
            usingItem = true;
            anim.Play(targetAnim);
        }

        public void DetectAction()
        {
            if (!canMove||usingItem||isBlocking)
                return;


            if (!rb && !rt && !lt && !lb)
                return;

           Action slot=  actionManager.GetActionSlot(this);
            if (slot == null)
                return;

            switch (slot.type)
            {
                case Actiontype.attack:
                    AttackAction(slot);
                    break;
                case Actiontype.block:
                    BlockAction(slot) ;
                    break;
                case Actiontype.spells:
                    break;
                case Actiontype.parry:
                    ParryAction(slot);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        void AttackAction(Action slot)
        {
            

          //  attacksLeftHand = actionManager.IsLeftHandSlot(slot);
            
            if (CheckForParry(slot))
                return;

            if (CheckForBackStab(slot))
                return;
            
            string targetAnim = string.Empty;
            targetAnim = slot.targetAnim;

            //if (rb)
            //    targetAnim = "oh_attack_1";
            //if (rt)
            //    targetAnim = "oh_attack_2";
            //if (lt)
            //    targetAnim = "oh_attack_3";
            //if (lb)
            //    targetAnim = "th_attack_1";
            if (string.IsNullOrEmpty(targetAnim))
                return;

            currentAction = slot;
            
            inAction = true;
            canMove = false;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }
            
            anim.SetFloat(StaticStrings.animSpeed,targetSpeed);
            anim.SetBool(StaticStrings.mirror,slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);
            //   rigid.velocity = Vector3.zero;
            //   Debug.Log("===============");
        }

        private bool CheckForParry(Action slot)
        {
            //            if (parryTarget == null)
            //                return false;

            if (!slot.canBeParried)
                return false;

            EnemyStates parryTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            
              //  Debug.DrawRay(origin,rayDir,Color.red);
            if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers))
            {
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

                
             if (parryTarget == null)
                return false;
             if (parryTarget.parriedBy == null)
                 return false;
            
         //   Debug.Log("G");
            
//            float dis = Vector3.Distance(parryTarget.transform.position, transform.position);
//            if (dis > 3)
//            {
//                return false;
//            }
            
            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);
            //格挡成果后必须面都面
            if (angle < 60)
            {
                Vector3 targetPosition = -dir*parryOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;

                if (dir == Vector3.zero)
                    dir = -parryTarget.transform.forward;
                
                Quaternion eRotation = Quaternion.LookRotation(-dir);
                Quaternion outRot = Quaternion.LookRotation(dir);

                parryTarget.transform.rotation = eRotation;
                transform.rotation = outRot;

                parryTarget.IsGettingParried(slot);

                inAction = true;
                canMove = false;
                //          anim.Play(targetAnim);
                anim.SetBool(StaticStrings.mirror,slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
               
                lockOnTarget = null;
                
                return true;
            }
            
            return false;
        }

        private bool CheckForBackStab(Action slot)
        {

            if (!slot.canBackStab)
                return false;
            
            EnemyStates backtab = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            
            //  Debug.DrawRay(origin,rayDir,Color.red);
            if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers))
            {
                backtab = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (backtab == null)
                return false;
            Vector3 dir = transform.position- backtab.transform.position ;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(backtab.transform.forward, dir);
        
            if (angle > 150)
            {
                
                Vector3 targetPosition = dir*backStabOffset;
                targetPosition += backtab.transform.position;
                transform.position = targetPosition;

             
                
           //     Quaternion eRotation = Quaternion.LookRotation(dir);
            //    Quaternion outRot = Quaternion.LookRotation(dir);

                backtab.transform.rotation = transform.rotation;
                //  transform.rotation = outRot;

                backtab.IsGettingBackstabbed(slot);

                inAction = true;
                canMove = false;
                //          anim.Play(targetAnim);
                anim.SetBool(StaticStrings.mirror,slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                lockOnTarget = null;
                return true;
            }

            return false;
        } 
        
        void BlockAction(Action slot)
        {
            isBlocking = true;
            isLeftHand = slot.mirror;
        }
        
        void ParryAction(Action slot)
        {
            string targetAnim = string.Empty;
            targetAnim = slot.targetAnim;

          
            if (string.IsNullOrEmpty(targetAnim))
                return;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }
            
            anim.SetFloat(StaticStrings.animSpeed,targetSpeed);
            
            canBeParried = slot.canBeParried;
            inAction = true;
            canMove = false;
            //          anim.Play(targetAnim);
            anim.SetBool(StaticStrings.mirror,slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);
            //   rigid.velocity = Vector3.zero;
            //   Debug.Log("===============");
        }
        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool(StaticStrings.onGround, onGround);
        }

        void HandleRolls()
        {
            if (!rollInput||usingItem)
                return;
            

            float v = vertical;
            float h = horizontal;
            v = (moveAmount > 0.3f ? 1 : 0);
            h = 0;

            //if (lockOn == false)
            //{
            //    v = (moveAmount > 0.3f ? 1 : 0);
            //    h = 0;
            //}
            //else
            //{
            //    if (Mathf.Abs(v) < 0.3f)
            //        v = 0;
            //    if (Mathf.Abs(h) < 0.3f)
            //        h = 0;
            //}

            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;

                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.InitForRoll();
                a_hook.rm_multi = rollSpeed;
                //Debug.Log("----------");

            }
            else
            {
                a_hook.rm_multi = 1.3f;

            }


            anim.SetFloat(StaticStrings.vertical, v);
            anim.SetFloat(StaticStrings.horizontal, h);

            inAction = true;
            canMove = false;

            anim.CrossFade(StaticStrings.Rolls, 0.2f);

        }

        void HandleMovementAnimations()
        {
            anim.SetBool(StaticStrings.run, run);
            anim.SetFloat(StaticStrings.vertical, moveAmount, 0.4f, delta); ;
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;
            anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
            anim.SetFloat(StaticStrings.horizontal, h, 0.2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;
            RaycastHit hit;
            Debug.DrawRay(origin, dir);
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }
            return r;
        }

        public void HandleTwoHanded()
        {
            anim.SetBool(StaticStrings.two_hand, isTwoHanded);
            if (isTwoHanded)
                actionManager.UpdateActionsTwoHanded();
            else
                actionManager.UpdateActionsOneHanded();
        }
        public void IsGettingParried(){
            
        }
    }
    
   

}


