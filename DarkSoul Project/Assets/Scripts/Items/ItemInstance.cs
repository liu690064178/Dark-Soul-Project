using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA
{
    public class ItemInstance : MonoBehaviour
    {
        public Weapon instance;

        public void Start()
        {
            Random.InitState(100);


            for (int i = 0; i < 100; i++)
            {
                Debug.Log(Random.Range(1,100));
            }
        }

        

    }

}
