using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public string preAttack;
    public string weapon1;
    public string weapon2;
    public int[] Damage = { 4, 5 };
    public int DamageRange = 2;
    public int[] atkRange = new int[]{0, 21, 20, 19, 17, 16, 14, 11, 9, 7, 5, 4, 3, 2, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
    public int MaxBullet = 4;
    public int bullet=4;
}