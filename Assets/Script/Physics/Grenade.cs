using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Vector3 FirstPos;
    private float Gravity = 9.8f;
    public Vector3 TargetPos;
    private float Velocity;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        //transform.p
        float H = FirstPos.y;
        dir = FirstPos - TargetPos;
        dir.y = 0;
        float Time = Mathf.Sqrt(2f*(H + dir.magnitude) / Gravity);
        Velocity = dir.magnitude / Time;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir.normalized;
    }
}
