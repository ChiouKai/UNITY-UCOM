using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    /*紀錄武器種類 攻擊距離 攻擊力
     
            距離      攻擊力     準確度     子彈數
     步槍    30        30        50        30
     狙擊    60        70         70        5
     霰彈    10        80         60        7
     小槍    20        20         50        7
     小刀    1         30         100      無限
     
     */

    public int[] Damage = { 4, 5 };
    public int[] atkRange = new int[]{0, 21, 20, 19, 17, 16, 14, 11, 9, 7, 5, 4, 3, 2, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
    public int bullet=4;
}