﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindControl : MonoBehaviour,ISkill
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
        Name = "MindControl";
        ai = GetComponent<AI>();
        AP = 2;
        CD = 3;
        CDCount = 0;
        AimPoint = 0f;
        Point = 6;
    }

    public string CheckUseable()
    {
        CDCount--;
        if (CDCount>0)
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
            return ai.MindControl;
        }
        else
        {
            return null;//
        }

    }
}