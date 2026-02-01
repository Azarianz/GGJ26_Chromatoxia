using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyAfterSeconds : MonoBehaviour
{   [SerializeField] float lifetime = 5f;

        void Start()
        {
            Destroy(gameObject, lifetime);
        }
    

}