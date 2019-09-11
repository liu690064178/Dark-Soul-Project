using UnityEngine;
using UnityEngine.Experimental.UIElements;


namespace SA
{
    public class InputHandler : MonoBehaviour {

        float vertical;
        float horizontal;
        float delta;

       
        bool b_input;
        bool a_input;
        bool x_input;
        bool y_input;

        bool rb_input;
        float rt_axis;
        bool rt_input;

        bool lb_input;
        float lt_axis;
        bool lt_input;

        bool leftAxis_down;
        bool rightAxis_down;

        float b_timer;
        float rt_timer;
        float lt_timer;


        StateManager states;
        CameraManager camManager;

	    // Use this for initialization
	    void Start () {
            
            UI.QuickSlot.singleton.Init();
            
            states = GetComponent<StateManager>();
            states.Init();
            camManager = CameraManager.singleton;
            camManager.Init(states);
	    }

        // Update is called once per frame
        void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);
            ResetInputNStates();
        }

        private void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            states.FixedTick(delta);
            camManager.Tick(delta);

           

        }




        void GetInput()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);
            b_input = Input.GetButton(StaticStrings.B);
            x_input = Input.GetButton(StaticStrings.X);
            y_input = Input.GetButtonUp(StaticStrings.Y);
            a_input = Input.GetButton(StaticStrings.A);
            rt_input = Input.GetButton(StaticStrings.RT);
            rt_axis = Input.GetAxis(StaticStrings.RT);

            if (rt_axis != 0)
                rt_input = true;

           // Debug.Log("rt_axis:"+rt_axis);
            //Debug.Log("rt_input"+rt_input);

            lt_input = Input.GetButton(StaticStrings.LT);
            lt_axis = Input.GetAxis(StaticStrings.LT);
            if (lt_axis != 0)
                lt_input = true;

            rb_input = Input.GetButton(StaticStrings.RB);
            lb_input = Input.GetButton(StaticStrings.LB);

            //Debug.Log("lt_axis:" + lt_axis);
            //Debug.Log("lt_input:" + lt_input);


            //  rt_input = Input.GetButton("RT");

            rightAxis_down = Input.GetButtonUp(StaticStrings.L)||Input.GetKeyUp(KeyCode.T);


            if (b_input)
            {
                b_timer += delta;
            }

        }

        void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;
            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            //  states.rollInput = b_input;

            if (x_input)
                b_input = false;

            if (b_input && b_timer>0.5f)
            {
                
                   states.run = (states.moveAmount>0);
            }
            if (!b_input && b_timer > 0f && b_timer < 0.5f)
            {
                states.rollInput = true;
            }


            //  states.b = b_input;
            states.itemInput = x_input;
            states.rt = rt_input;
            states.lt = lt_input;
            states.rb = rb_input;
            states.lb = lb_input;

            if (y_input)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }

            if (states.lockOnTarget != null)
            {
                if (states.lockOnTarget.eStates.isDead)
                {
                    states.lockOn = false;
                    states.lockOnTarget = null;
                    states.lockOnTransform = null;
                    camManager.lockOn = false;
                    camManager.lockOnTarget = null;
                }

            }
            else
            {
                states.lockOn = false;
                states.lockOnTarget = null;
                states.lockOnTransform = null;
                camManager.lockOn = false;
                camManager.lockOnTarget = null;

          
            }

            if (rightAxis_down)
            {
                states.lockOn = !states.lockOn;
//                if (states.lockOnTarget == null)
//                {
//                    states.lockOn = false;
//                    
//                }

                states.lockOnTarget = EnemyManager.singleton.GetEnemy(transform.position);

                if (states.lockOnTarget == null)
                {
                    states.lockOn = false;
                }
            
                 camManager.lockOnTarget = states.lockOnTarget;
                 states.lockOnTransform = states.lockOnTarget.GetTarget();
                 states.lockOnTransform = camManager.lockOnTransform;
                 camManager.lockOnTarget = states.lockOnTarget;
                 camManager.lockOn = states.lockOn;
                
            }
        }

        void ResetInputNStates()
        {
            if (b_input == false)
            {
                b_timer = 0;
            }
            if (states.rollInput)
            {
                states.rollInput = false;
            }
            if (states.run)
            {
                states.run = false;
            }
        }
    }

}


