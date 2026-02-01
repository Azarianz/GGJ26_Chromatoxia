using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeItAppear : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        StartCoroutine(ShowThenDelete());
    }

    IEnumerator ShowThenDelete()
    {
        yield return new WaitForSeconds(1f);   // wait before showing
        target.SetActive(true);

        yield return new WaitForSeconds(5f);   // stay visible
        Destroy(gameObject);
    }
}
