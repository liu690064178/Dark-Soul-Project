using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class InventoryManager : MonoBehaviour {

        public ItemInstance rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public ItemInstance leftHandWeapon;

        public GameObject parryCollider;
        
        StateManager states;

        public void Init(StateManager st)
        {
	        states = st;

            if(rightHandWeapon!=null)
			    EquipWeapon(rightHandWeapon.instance,false);

            if(leftHandWeapon!=null)
			    EquipWeapon(leftHandWeapon.instance,true);

            hasLeftHandWeapon = (leftHandWeapon != null);

			InitAllDamageCollider(st);
			CloseAllDamageCollider();

			ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
			pr.InitPlayer(st);
			
			CloseParryCollider();
        }
 
        public void EquipWeapon(Weapon w, bool isLeft = false)
        {
	        string targetIdle = w.oh_idle;
	        targetIdle += isLeft ? "_l" : "_r";
	        states.anim.SetBool(StaticStrings.mirror,isLeft);
	        states.anim.Play("changeWeapon");
	        states.anim.Play(targetIdle);

	        UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
	        uiSlot.UpdateSlot((isLeft)?UI.QSlotType.lh:UI.QSlotType.rh,w.icon);
	        
	        
	        
        }

        public Weapon GetCurrentWeapon(bool isLeft)
        {
	        if (isLeft)
	        {
		        return
		        leftHandWeapon.instance;
	        }
	        else
	        {
		        return
		        rightHandWeapon.instance;
	        }
        }


        public void EquipRightWeapon()
        {
	        string targetIdle = rightHandWeapon.instance.oh_idle;
	        targetIdle += "_r";
	        
	        states.anim.SetBool("mirror",false);
	        states.anim.Play("changeWeapon");
	        states.anim.Play(targetIdle);
        }

        public void EquipLeftWeapon()
        {
	        if (!hasLeftHandWeapon)
		        return;
	        string targetIdle = rightHandWeapon.instance.oh_idle;
	        targetIdle += "_l";
	        states.anim.SetBool("mirror",true);
	        states.anim.Play("changeWeapon");
	        states.anim.Play(targetIdle);
        }
        public void OpenAllDamageColliders()
        {
	        if(rightHandWeapon!=null)
		        rightHandWeapon.instance.w_hook.OpenDamageColliders();
	        
	        if(leftHandWeapon.instance.w_hook!=null)
		        leftHandWeapon.instance.w_hook.OpenDamageColliders();
        }
        
        public void CloseAllDamageCollider()
        {
	        if(rightHandWeapon!=null)
				 rightHandWeapon.instance.w_hook.CloseDamageColliders();
	        
	        if(leftHandWeapon.instance.w_hook!=null)
		        leftHandWeapon.instance.w_hook.CloseDamageColliders();

        }

        public void InitAllDamageCollider(StateManager states)
        {
	        if(rightHandWeapon!=null)
		        rightHandWeapon.instance.w_hook.InitDamageCollider(states);
	        
	        if(leftHandWeapon.instance.w_hook!=null)
		        leftHandWeapon.instance.w_hook.InitDamageCollider(states);

        }

        public void CloseParryCollider()
        {
	        parryCollider.SetActive(false);
        }
        public void OpenParryCollider()
        {
	        parryCollider.SetActive(true);
        }
        
        // Use this for initialization
        void Start () {
		
	    }
	
	    // Update is called once per frame
	    void Update () {
		
	    }
    }

    [System.Serializable]
    public class Weapon
    {
	    public string weaponId;
	    public Sprite icon;
	    
	    
	    public string oh_idle;
	    public string th_idle;
	    
        public List<Action> acitons;
       // public List<Action> leftHand_acitons;
        public List<Action> two_handedActions;
        //public WeaponStats parryStats;
        //public WeaponStats backstabStats;

        public float parryMultiplier;
        public float backstabMultiplier;

        public bool LeftHandMirror;
        public GameObject weaponModel;
        public WeaponHook w_hook;

        public Action GetAction(List<Action> l, ActionInput inp)
        {
	        for (int i = 0; i < l.Count; i++)
	        {
		        if (l[i].input == inp)
		        {
			        return l[i];
		        }
	        }

	        return null;
        }
    }

}

