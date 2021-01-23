using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Vector3 FirstPos;
    private float Gravity = 9.8f;
    public Tile TargetTile;
    private float Velocity;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        FirstPos = transform.position;
        float H = FirstPos.y;
        dir =  TargetTile.transform.position- FirstPos;
        dir.y = 0;
        float Time = Mathf.Sqrt(2f*(H + dir.magnitude) / Gravity);
        Velocity = dir.magnitude / Time;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * (dir.normalized * Velocity + Vector3.up * (Velocity -1f / 2f * Gravity * Time.deltaTime));
        if ((transform.position - TargetTile.transform.position).magnitude < 0.5f)
        {
            Explosion();
            Destroy(gameObject);
        }
    }


    private void Explosion()
    {
        AI cha;
        if (TargetTile.Cha != null)
        {
            cha = TargetTile.Cha;
            cha.BeDamaged(3);
            cha.Hurt(-TargetTile.Cha.transform.forward);
        }
        for(int i = 0; i < 8; ++i)
        {
            if (TargetTile.AdjList[i].Cha != null)
            {
                cha = TargetTile.AdjList[i].Cha;
                cha.BeDamaged(3);
                cha.Hurt(TargetTile.transform.position - TargetTile.AdjList[i].transform.position);
            }
        }
        Instantiate<GameObject>(Resources.Load<GameObject>("Explsion"),TargetTile.transform.position,Quaternion.identity);
    }

}
