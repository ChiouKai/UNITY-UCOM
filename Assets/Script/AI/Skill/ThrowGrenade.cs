using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour,ISkill
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
        Name = "Grenade";
        ai = GetComponent<AI>();
        AP = 2;
        CD = 0;
        CDCount = 0;
        AimPoint = 0f;
        Point = 0;
        type = 0;
    }

    public bool CheckUseable(AI Target)
    {
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
            return null;
        }
        else
        {
            return UISystem.getInstance().PreGrenade;
        }
    }

    public void CountCD()
    {
        if (CDCount == 0)
            return;
        --CDCount;
    }
}
