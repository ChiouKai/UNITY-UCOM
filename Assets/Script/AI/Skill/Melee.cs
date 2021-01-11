using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour , ISkill
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
        Name = "Melee";
        ai = GetComponent<AI>();
        AP = 2;
        CD = 1;
        CDCount = 0;
        AimPoint = 0;
        Point = 3;
    }

    public bool CheckUseable(AI Target)
    {
        if (Target == null)
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
            return ai.PreMelee;
        }
        else
        {
            return UISystem.getInstance().PreMelee;
        }
    }

    public void CountCD()
    {
        if (CDCount == 0)
            return;
        --CDCount;
    }
}
