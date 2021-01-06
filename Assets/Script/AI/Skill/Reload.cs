using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Reload : MonoBehaviour, ISkill
{
    public int AP { get; private set; }

    public int CD { get; private set; }

    public int CDCount { get; private set; }

    public int Point { get; private set; }

    public AI ai { get; private set; }

    public string Name { get; private set; }


    private void Start()
    {
        AP = 1;
        CD = 0;
        CDCount = 0;
        Point = 2;
        ai = GetComponent<AI>();
        Name = "Reload";
    }

    public string Func()
    {
        if (ai.Gun.bullet == ai.Gun.MaxBullet)
        {
            return null;
        }
        return Name;
    }
}
