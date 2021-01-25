﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grenade : MonoBehaviour
{
    public Vector3 FirstPos;
    private float Gravity = 9.8f;
    public Tile TargetTile;
    private float VelocityH;
    private float VelocityV;
    Vector3 dir;
    public ParticleSystem Trail;
    // Start is called before the first frame update
    void Start()
    {
        FirstPos = transform.position;
        float H = FirstPos.y;
        dir =  TargetTile.transform.position- FirstPos;
        dir.y = 0;
        float Time = Mathf.Sqrt(2f*(H + dir.magnitude) / Gravity);
        VelocityV =  VelocityH = dir.magnitude / Time;
        Trail.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * (dir.normalized * VelocityH + Vector3.up * (VelocityV - 1f / 2f * Gravity * Time.deltaTime));
        VelocityV -= Gravity * Time.deltaTime;
        if ((transform.position - TargetTile.transform.position).magnitude < 0.5f)
        {
            FindObjectOfType<SoundManager>().Play("Grenade");
            Explosion();
            UISystem.getInstance().AfterGrenade(TargetTile);
            Destroy(gameObject);
        }
    }


    private void Explosion()
    {
        AI cha;
        if (TargetTile.Cha != null)
        {
            cha = TargetTile.Cha;
            cha.BeDamaged(4);
            cha.Hurt(-TargetTile.Cha.transform.forward);
            Vector3 vScreenPos = Camera.main.WorldToScreenPoint(cha.BeAttakePoint.transform.position);
            vScreenPos += Vector3.right * Random.Range(100, 200) + Vector3.up * 100f;
            GameObject go = Instantiate(UISystem.getInstance().status_UI[0]) as GameObject;
            go.transform.GetChild(2).GetComponent<Text>().text = "4";
            go.transform.position = vScreenPos;

            go.transform.SetParent(UISystem.getInstance().HPCanvas.transform);
            Destroy(go, 2f);
        }
        for(int i = 0; i < 8; ++i)
        {
            if (TargetTile.AdjList[i].Cha != null)
            {
                cha = TargetTile.AdjList[i].Cha;
                cha.BeDamaged(4);
                cha.Hurt(TargetTile.AdjList[i].transform.position - TargetTile.transform.position);
                Vector3 vScreenPos = Camera.main.WorldToScreenPoint(cha.BeAttakePoint.transform.position);
                vScreenPos += Vector3.right * Random.Range(100, 200) + Vector3.up * 100f;
                GameObject go = Instantiate(UISystem.getInstance().status_UI[0]) as GameObject;
                go.transform.GetChild(2).GetComponent<Text>().text = "4";
                go.transform.position = vScreenPos;

                go.transform.SetParent(UISystem.getInstance().HPCanvas.transform);
                Destroy(go, 2f);

            }
        }
        Instantiate<GameObject>(Resources.Load<GameObject>("Explosion"),TargetTile.transform.position,Quaternion.identity);
    }

}
