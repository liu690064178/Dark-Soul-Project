using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour
{


    public float m_velocity;
    public float m_acceleration;
    public float maxVelocity;
    public bool isPlayer;

    public Transform[] ways;

    public int nextPoint;

    Quaternion target;
    Vector3 dir;
    NavMeshAgent nav;

    


    private void Awake()
    {

        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;

        for (int i = 0; i < ways.Length; i++)
        {
            ways[i].position = new Vector3(ways[i].position.x, transform.position.y, ways[i].position.z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isPlayer)
        {
            nav.enabled = true;
            nav.SetDestination(ways[nextPoint].position);

        }

        //dir = ways[nextPoint].position - transform.position;
        //dir.y = transform.position.y;
        //target = Quaternion.LookRotation(dir);
    }

    // Update is called once per frame
    void Update()
    {

        if (isPlayer)
        {


            //  transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime + 0.5f);

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.Space))
            {
                m_velocity = m_velocity + m_acceleration * -2 * Time.deltaTime;
                m_velocity = Mathf.Clamp(m_velocity, 0, maxVelocity);
            }
            else
            {
                m_velocity = m_velocity+ m_acceleration*v * Time.deltaTime;
                m_velocity = Mathf.Clamp(m_velocity, 0, maxVelocity);

            }



            transform.Translate(Vector3.forward * m_velocity * Time.deltaTime);
            transform.Rotate(Vector3.up * h * 100 * Time.deltaTime);

            return;
        }

      




        if (Vector3.Distance(transform.position, ways[nextPoint].position) < 0.5f)
        {
            nextPoint++;
            nextPoint %= ways.Length;
            dir = ways[nextPoint].position - transform.position;

            
                nav.SetDestination(ways[nextPoint].position);


            dir.y = transform.position.y;
            target =  Quaternion.LookRotation(dir);

        }
    }
}
