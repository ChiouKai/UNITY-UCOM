﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Reload : MonoBehaviour, ISkill
{
    public int AP { get; private set; }

    public int CD { get; private set; }

    public int CDCount { get; private set; }

    public float AimPoint { get; private set; }
    public int Point { get; private set; }

    public AI ai { get; private set; }

    public string Name { get; private set; }


    private void Start()
    {
        AP = 1;
        CD = 0;
        CDCount = 0;
        AimPoint = 0f;
        Point = 2;
        ai = GetComponent<AI>();
        Name = "Reload";
    }

    public string CheckUseable()
    {
        if (ai.Gun.bullet == ai.Gun.MaxBullet)
        {
            return null;
        }
        return Name;
    }

    public void EnterCD()
    {
        CDCount = CD;
    }

    public Action GetAction()
    {
        if (ai.Cha.camp == Character.Camp.Alien)
        {
            return ai.AIReload;
        }
        else
        {
            return UISystem.getInstance().PreReload;
        }
    }
}