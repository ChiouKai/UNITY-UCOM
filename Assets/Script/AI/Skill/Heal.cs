using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour ,ISkill
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
        Name = "Heal";
        ai = GetComponent<AI>();
        AP = 2;
        CD = 3;
        CDCount = 0;
        AimPoint = 0;
        Point = 2;
        type = 0;
    }

    public bool CheckUseable(AI Target)
    {
        if (CDCount > 0)
        {
            return false;
        }
        ai.HealList.Clear();
        if (ai.Cha.HP < ai.Cha.MaxHP)
        {
            ai.HealList.AddLast(ai);
        }
        foreach (Tile T in ai.CurrentTile.AdjList)
        {
            AI Cha= T.Cha;
            if (Cha != null&& Cha.Cha.camp==Character.Camp.Human&& Cha.Cha.type==Character.Type.Humanoid&& Cha.Cha.HP!= Cha.Cha.MaxHP)
            {
                ai.HealList.AddLast(T.Cha);
            }
        }
        if (ai.Cha.HP != ai.Cha.MaxHP)
        {
            ai.HealList.AddLast(ai);
        }
        if (ai.HealList.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EnterCD()
    {
        CDCount = CD;
    }
    public Action GetAction()
    {
        if (ai.Cha.camp == Character.Camp.Alien)
        {
            return ai.PreFire;
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
