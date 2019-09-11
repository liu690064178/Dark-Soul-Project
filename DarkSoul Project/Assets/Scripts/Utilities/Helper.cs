using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{

    public class Helper : MonoBehaviour {

        Animator anim;

        [Range(-1, 1)]
        public float vertical;
        [Range(-1, 1)]
        public float horizontal;

      //  public string animName;
        public bool playAnim;
        public string[] oh_attacks;

        public bool twoHanded;
        public string[] th_attacks;

        public bool enableRM;
        public bool useItem;
        public bool interacting;
        public bool lockon;


	    // Use this for initialization
	    void Start () {
            anim = GetComponent<Animator>();
	    }
	
	    // Update is called once per frame
	    void Update () {


            enableRM = !anim.GetBool("canMove");
            anim.applyRootMotion = enableRM;
            interacting = anim.GetBool("interacting");

            if (!lockon)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }
            anim.SetBool("lockon", lockon);

            if (enableRM)
                return;

            if (useItem)
            {
                twoHanded = false;
                anim.Play("use_item");
                useItem = false;
            }
            if (interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, 0f, 0.5f);
            }

            anim.SetBool("two_handed", twoHanded);

            if (playAnim)
            {
                string targetAnim;
                if (!twoHanded)
                {
                    targetAnim = oh_attacks[Random.Range(0, oh_attacks.Length)];
                    
                }

                else
                {
                    targetAnim = th_attacks[Random.Range(0, th_attacks.Length)];

                }
                if (vertical > 0.5f)
                     targetAnim = "oh_attack_3";

                vertical = 0;
                anim.CrossFade(targetAnim,0.2f);

                playAnim = false;
            }
            anim.SetFloat("vertical", vertical);
            anim.SetFloat("horizontal", horizontal);
            
	    }
    }
}

