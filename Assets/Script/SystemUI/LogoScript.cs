using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LogoScript : MonoBehaviour
{
    public Vector2 InsertLocation, LeaveLocation;
    RectTransform RT;
    TimeLine TL;
    private void Start()
    {
        RT = GetComponent<RectTransform>();
        InsertLocation = RT.anchoredPosition + new Vector2(-200, 0);
        LogoUpdate = LogoInsert;
        TL = TimeLine.Instance;
    }


    public Action LogoUpdate;

    public void LogoInsert()
    {
        if ((RT.anchoredPosition- InsertLocation).magnitude > 1f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, InsertLocation, 0.06f);
        }
        else
        {
            TL.Moved = true;
            RT.anchoredPosition = InsertLocation;
            LeaveLocation = RT.anchoredPosition + new Vector2(200, 0);
            LogoUpdate = null;
        }

    }

    public void LogoMove()
    {
        if ((RT.anchoredPosition - InsertLocation).magnitude >1f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, InsertLocation, 0.03f);
        }
        else
        {
            RT.anchoredPosition = InsertLocation;
            LeaveLocation = RT.anchoredPosition + new Vector2(200, 0);
            LogoUpdate = null;
        }
    }

    public void LogoLeave()
    {
        if ((RT.anchoredPosition - LeaveLocation).magnitude > 1f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, LeaveLocation, 0.06f);
        }
        else
        {
            RT.anchoredPosition = InsertLocation + new Vector2(200, 0);
            LogoUpdate = LogoInsert;
        }
    }
    public void LogoDeath()
    {
        if ((RT.anchoredPosition - LeaveLocation).magnitude > 1f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, LeaveLocation, 0.06f);
        }
        else
        {
            TL.Moved = true;
            TL.acting = () => { TimeLine.Instance.LogoList.Remove(this); Destroy(this.gameObject); TimeLine.Instance.acting = null; };
        }
    }
}
