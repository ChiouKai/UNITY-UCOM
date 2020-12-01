using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Character :MonoBehaviour
{
    public int HP = 5;
    public float Mobility= 8.0f;//移動距離
    public int Speed = 5;

    public enum group
    {
        Human = 0,
        Alien = 1
    }
    public enum Type
    {
        Humanoid = 0,
        Monster = 1,
        Robet = 2
    }

}
