using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : MonoBehaviour ,ISkill
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
        Name = "Leave";
        ai = GetComponent<AI>();
        AP = 0;
        CD = 0;
        CDCount = 0;
        AimPoint = 0;
        Point = 0;
    }

    public bool CheckUseable(AI Target)
    {
        if (UISystem.getInstance().Bomb_start&&(ai.CurrentTile==UISystem.getInstance().LeaveTile[0]|| ai.CurrentTile == UISystem.getInstance().LeaveTile[1]))
        {
            return true;
        }
        return false;

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
            return UISystem.getInstance().PreFire;
        }
    }

    public void CountCD()
    {
        if (CDCount == 0)
            return;
        --CDCount;
    }
}
