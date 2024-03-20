using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayABit : MonoBehaviour
{
    public GameObject obj1, obj2, obj3, obj4;
    public float delay = 15f;

    // Start is called before the first frame update
    void Start()
    {
        obj1.SetActive(true);
        obj2.SetActive(false);
        obj3.SetActive(false);
        obj4.SetActive(false);
        StartCoroutine(Delayed());
    }

    public IEnumerator Delayed()
    {
        yield return new WaitForSeconds(delay);
        obj1.SetActive(false);
        obj2.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            obj2.SetActive(!obj2.active);
            obj3.SetActive(!obj3.active);
            obj4.SetActive(!obj4.active);
        }
    }
}
