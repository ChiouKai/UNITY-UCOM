using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCheck 
{
    static public EndCheck Instance;

    public static EndCheck GetInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }
        else
        {
            Instance = new EndCheck();
            return Instance;
        }
    }
    private EndCheck()
    {
        ChaEnd = false;
    }
    public bool ChaEnd;



}
