using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Character :MonoBehaviour
{
    public int MaxHP = 5;
    public int HP = 5;
    public float Mobility= 8.0f;//移動距離
    public int Speed = 5;
    public int BasicAim = 70;
    string Weapon;
    public int Energy;
    public int MaxEnergy ;
    public enum Camp
    {
        Human = 0,
        Alien = 1
    }
    public Camp camp;

    public enum Type
    {
        Humanoid = 0,
        Monster = 1,
        Robet = 2
    }
    public Type type;
}
