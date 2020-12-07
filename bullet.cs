using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    float life_time;
    
    // Update is called once per frame
    void Update()
    {
        life_time += Time.deltaTime;

        this.transform.position += transform.forward * Time.deltaTime * 20.0f;
        if (life_time > 1)
        {
            GameObject.Find("ObjectPool").GetComponent<ObjectPool>().Recovery(gameObject);
            life_time = 0;
        }
    }
}
