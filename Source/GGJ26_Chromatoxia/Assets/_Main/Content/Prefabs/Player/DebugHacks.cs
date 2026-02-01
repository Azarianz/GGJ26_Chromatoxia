using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHacks : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("F2");
            GameManager.I.Win("");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("F2");
            GameManager.I.Lose("");
        }
    }
}
