using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class DamageCollider : MonoBehaviour {
        
        StateManager states;
        
        public void Init(StateManager st)
        {
            states = st;
          
        
          

        }
        
        
        private void OnTriggerEnter(Collider other)
        {         
            EnemyStates eStates = other.transform.root.GetComponentInParent<EnemyStates>();
            if (eStates == null)
                return;
        //    Weapon w = states.inventoryManager.GetCurrentWeapon(states.attacksLeftHand);
            eStates.DoDamage(states.currentAction);
        }
    }

}
