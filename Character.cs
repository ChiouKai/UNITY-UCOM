using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character :MonoBehaviour
{
    public int HP = 5;
    public float Mobility= 8.0f;
    public int Speed = 5;
    public int AP = 0;
    enum group
    {
        Human = 0,
        Alien = 1
    }
    enum Type
    {
        Humanoid = 0,
        Monster = 1,
        Robet = 2
    }

}
