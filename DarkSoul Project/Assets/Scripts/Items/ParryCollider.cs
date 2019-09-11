using UnityEngine;

namespace SA
{
    public class ParryCollider:MonoBehaviour
    {
        private StateManager states;
        private EnemyStates eStates;

        
        public float maxTimer = 0.6f;
        private float timer;
        public void InitPlayer(StateManager st)
        {
            states = st;
        }


        void Update()
        {
            if (states)
            {
                timer += states.delta;
                if (timer > maxTimer)
                {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }

            if (eStates)
            {
                timer += eStates.delta;
                if (timer > maxTimer)
                {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }
        }
        

        public void InitEnemy(EnemyStates st)
        {
            eStates = st;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (states)
            {
                EnemyStates e_st = other.transform.root.GetComponentInParent<EnemyStates>();
                if (e_st == null)
                    return;
                if (!e_st.canBeParried||e_st.isInvicible)
                {
                    return;
                }
    
                if (e_st != null)
                {
                    e_st.CheckForParry(transform.root, states);
                }
                
            }

            if (eStates)
            {
                //check for player
            }
            
            
        }
    }
}