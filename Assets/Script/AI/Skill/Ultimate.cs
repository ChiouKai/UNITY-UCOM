using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate : MonoBehaviour , ISkill
{
    public int AP { get; private set; }

    public int CD { get; private set; }

    public int CDCount { get; private set; }

    public float AimPoint { get; private set; }
    public int Point { get; private set; }

    public AI ai { get; private set; }

    public string Name { get; private set; }
    public int type { get; private set; }

    private void Start()
    {
        Name = "Ultimate";
        ai = GetComponent<AI>();
        AP = 1;
        CD = 0;
        CDCount = 0;
        AimPoint = 0;
        Point = 10;
        type = 0;
    }

    public bool CheckUseable(AI Target)
    {
        if (!ai.Ult)
        {
            return false;
        }
        return true;

    }

    public void EnterCD()
    {
        CDCount = CD;
    }
    public Action GetAction()
    {
        if (ai.Cha.camp == Character.Camp.Alien)
        {
            return ai.PreMindControl;
        }
        else
        {
            return null;
        }
    }

    public void CountCD()
    {
        if (CDCount == 0)
            return;
        --CDCount;
    }
}
