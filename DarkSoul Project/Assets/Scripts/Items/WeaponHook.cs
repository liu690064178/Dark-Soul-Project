using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class WeaponHook : MonoBehaviour {

        public GameObject[] damageCollier;

        public void OpenDamageColliders()
        {
            for (var i = 0; i < damageCollier.Length; i++)
            {
                damageCollier[i].SetActive(true);
            }
        }
        
        public void CloseDamageColliders()
        {
            for (var i = 0; i < damageCollier.Length; i++)
            {
                damageCollier[i].SetActive(false);
            }
        }

        public void InitDamageCollider(StateManager states)
        {
            for (var i = 0; i < damageCollier.Length; i++)
            {
                damageCollier[i].GetComponent<DamageCollider>().Init(states);
            }
        }
    }

}
