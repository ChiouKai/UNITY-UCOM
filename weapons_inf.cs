using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class weapons_inf
{
    /*紀錄武器種類 攻擊距離 攻擊力
     
            距離      攻擊力     準確度     子彈數
     步槍    30        30        50        30
     狙擊    60        70         70        5
     霰彈    10        80         60        7
     小槍    20        20         50        7
     小刀    1         30         100      無限
     
     */
    public GameObject gun;
    public float dir;
    public float atk;
    public float acc;
    public int bullet;
}
public class nowtarget
{
    public GameObject nt;
    public string name;
}