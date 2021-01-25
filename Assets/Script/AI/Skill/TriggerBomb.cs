using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBomb : MonoBehaviour , ISkill
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
        Name = "Bomb";
        ai = GetComponent<AI>();
        AP = 0;
        CD = 0;
        CDCount = 0;
        AimPoint = 0;
        Point = -2;
        type = 1;
    }

    public bool CheckUseable(AI Target)
    {
        if (ai.CurrentTile!=UISystem.getInstance().BombSite|| UISystem.getInstance().Bomb_start)
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
        return UISystem.getInstance().PreBomb;
    }

    public void CountCD()
    {
        if (CDCount == 0)
            return;
        --CDCount;
    }
}
