using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour,ISkill
{
    public int AP { get; private set; }

    public int CD  { get; private set; }

    public int CDCount { get; private set ; }

    public int Point { get;  private set; }

    public AI ai { get; private set; }

    public string Name { get; private set; }

    private void Start()
    {
        Name = "Fire";
        ai = GetComponent<AI>();
        AP = 2;
        CD = 1;
        CDCount = 0;
        //Point = ;
    }

    public string Func()
    {
        if (ai.Gun.bullet == 0||ai.AttakeableList.Count==0)
        {
            return null;
        }
        return Name;

    }
}
