using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SA
{
    public class EnemyTarget : MonoBehaviour {

        [Header("被锁定下标")]
        public int index;
        public List<Transform> targets = new List<Transform>();
        public List<HumanBodyBones> h_bones = new List<HumanBodyBones>();
        Animator anim;

        public EnemyStates eStates;

        public void Init(EnemyStates eSt)
        {
          //  anim = GetComponent<Animator>();
          eStates = eSt;
            anim = eStates.anim;
            if (anim.isHuman == false)
            {
                return;
            }
            for (int i = 0; i < h_bones.Count; i++)
            {
                targets.Add(anim.GetBoneTransform(h_bones[i])); 
            }
            EnemyManager.singleton.enemyTargets.Add(this);
        }

        public Transform GetTarget(bool negative = false)
        {
            if (targets.Count == 0)
                return transform;

          

            if(negative == false)
            {

                if(index < targets.Count - 1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
            }
            else
            {
                if (index < 0)
                {
                    index = targets.Count - 1;
                }
                else
                {
                    index--;
                }
            }
            index = Mathf.Clamp(index, 0, targets.Count);
            return targets[index];

        }
	
    }

}

