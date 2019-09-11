
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace SA
{

    public class ActionManager : MonoBehaviour
    {

        public List<Action> actionSlots = new List<Action>();

        public ItemAction consumableItem;

        StateManager states;
        public void Init(StateManager st)
        {
            states = st;
            UpdateActionsOneHanded();
         //   UpdateActionsTwoHanded();
         
            

        }



        
        
        

        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots();
            
            DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb,ActionInput.rb);
            DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt,ActionInput.rt);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb,ActionInput.lb,true);
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt,ActionInput.lt,true);               
            }
            else
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.lb,ActionInput.lb);
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.lt,ActionInput.lt);
            }
            /*
            if (states.inventoryManager.hasLeftHandWeapon)
            {
                UpdateActionsWithLeftHand();
                return;
            }
            
            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.acitons.Count; i++)
            {
                Action a = GetAction(w.acitons[i].input);
                a.targetAnim = w.acitons[i].targetAnim;
            }*/
        }

        public void DeepCopyAction(Weapon w,ActionInput inp,ActionInput assign,bool isLeftHand = false)
        {
            Action a = GetAction(assign);
            Action w_a = w.GetAction(w.acitons,inp);

            if (w_a == null)
                return;

            a.targetAnim =w_a.targetAnim;
            a.type = w_a.type;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackStab = w_a.canBackStab;
            a.damageAnim = w_a.damageAnim;
            a.overriderDamageAnim = w_a.overriderDamageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;
      //      a.AssignPointer(w);

            if (isLeftHand)
            { 
                a.mirror = true;
            }
            DeepCopyWeaponStats(w_a.weaponStats,a.weaponStats);
        }

        public void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {
            to.pysical = from.pysical;
            to.slash = from.slash;
            to.strike = from.strike;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.lighting = from.lighting;
            to.dark = from.dark;
            to.fire = from.fire;
        }
        
        
        public void UpdateActionsWithLeftHand()
        {
            DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb,ActionInput.rb);
            DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt,ActionInput.rt);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb,ActionInput.lb,true);
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt,ActionInput.lt,true);               
            }
            else
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.lb,ActionInput.lb);
                DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.lt,ActionInput.lt);
            }
            /*
            Weapon r_w = states.inventoryManager.rightHandWeapon;
            Weapon l_w = states.inventoryManager.leftHandWeapon;

            Action rb = GetAction(ActionInput.rb);
            Action rt = GetAction(ActionInput.rt);

            Action w_rb = r_w.GetAction(r_w.acitons, ActionInput.rb); 
            rb.targetAnim =w_rb.targetAnim;
            rb.type = w_rb.type;
            rb.canBeParried = w_rb.canBeParried;
            rb.changeSpeed = w_rb.changeSpeed;
            rb.animSpeed = w_rb.animSpeed;
            
            Action w_rt = r_w.GetAction(r_w.acitons, ActionInput.rt); 
            rt.targetAnim =w_rt.targetAnim;
            rt.type = w_rt.type;
            rt.canBeParried = w_rt.canBeParried;
            rt.changeSpeed = w_rt.changeSpeed;
            rt.animSpeed = w_rt.animSpeed;
           
            
            Action lb = GetAction(ActionInput.lb);
            Action lt = GetAction(ActionInput.lt);
            
            Action w_lb = l_w.GetAction(l_w.acitons, ActionInput.rb); 
            lb.targetAnim = w_lb.targetAnim;
            lb.type = w_lb.type;
            lb.canBeParried = w_lb.canBeParried;
            lb.changeSpeed = w_lb.changeSpeed;
            lb.animSpeed = w_lb.animSpeed;
            
            Action w_lt = l_w.GetAction(l_w.acitons, ActionInput.rt); 
            lt.targetAnim = w_lt.targetAnim;
            lt.type = w_lt.type;
            lt.canBeParried = w_lt.canBeParried;
            lt.changeSpeed = w_lt.changeSpeed;
            lt.animSpeed = w_lt.animSpeed;

            if (l_w.LeftHandMirror)
            {
                lb.mirror = true;
                lt.mirror = true;
                
            }*/

        }
        
        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon.instance;
            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = GetAction(w.two_handedActions[i].input);
                a.targetAnim = w.two_handedActions[i].targetAnim;
                a.type = w.two_handedActions[i].type;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
                a.targetAnim = null;
                a.mirror = false;
                a.type = Actiontype.attack;
            }
            
        }

        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);

            }
        }

        public ActionInput GetActionInput(StateManager st)
        {

            if (st.rb)
                return ActionInput.rb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lb)
                return ActionInput.lb;
            if (st.lt)
                return ActionInput.lt;
            return ActionInput.rb;
        }

        public Action GetAction(ActionInput inp)
        {

            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return GetAction(a_input);
        }

        public bool IsLeftHandSlot(Action slot)
        {
            return (slot.input == ActionInput.lb || slot.input == ActionInput.lt);
        }


    }

    public enum ActionInput
    {
        rb, lb, rt, lt
    }


    public enum Actiontype
    {
        attack,block,spells,parry
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public Actiontype type;
        public string targetAnim;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackStab = false;

        [HideInInspector]
        public float parryMultiplier;
        [HideInInspector]
        public float backstabMultiplier;

        public bool overriderDamageAnim;
        public string damageAnim;

        public WeaponStats weaponStats;
      //  [HideInInspector]
        //Weapon weaponPointer;

        // public void AssignPointer(Weapon w)
        // {
        //     weaponPointer = w;
        // }

        // public Weapon GetWeaponPointer()
        // {
        //     return weaponPointer;
        // } 
    }
    [System.Serializable]
    public class ItemAction
    {
        public string targetAnim;
        public string item_id;
    }


}

