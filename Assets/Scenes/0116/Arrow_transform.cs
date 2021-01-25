using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_transform : MonoBehaviour
{
    float time_ud;
    float y = 0.01f;
    // Update is called once per frame
    void FixedUpdate()
    {
        time_ud += Time.deltaTime;
        if (time_ud > 0.5f)
        {
            y = -y;
            time_ud = 0;
        }
        if (Time.timeScale != 0)
        {
            this.transform.position += new Vector3(0, y, 0);
            transform.Rotate(0.5f, 0, 0);
        }
    }
}
