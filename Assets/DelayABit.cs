using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayABit : MonoBehaviour
{
    public GameObject obj1, obj2;

    // Start is called before the first frame update
    void Start()
    {
        obj1.SetActive(true);
        obj2.SetActive(false);
        StartCoroutine(Delayed());
    }

    public IEnumerator Delayed()
    {
        yield return new WaitForSeconds(15f);
        obj1.SetActive(false);
        obj2.SetActive(true);
    }
}
