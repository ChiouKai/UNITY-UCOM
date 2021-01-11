using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoScript : MonoBehaviour
{
    public Vector2 InsertLocation, LeaveLocation;
    RectTransform RT;
    private void Start()
    {
        RT = GetComponent<RectTransform>();
        InsertLocation = RT.anchoredPosition + new Vector2(-200, 0);
        LogoUpdate = LogoInsert;
    }


    public Action LogoUpdate;

    public void LogoInsert()
    {
        if ((RT.anchoredPosition- InsertLocation).magnitude > 0.5f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, InsertLocation, 0.06f);
        }
        else
        {
            TimeLine.Instance.Moved = true;
            RT.anchoredPosition = InsertLocation;
            LeaveLocation = RT.anchoredPosition + new Vector2(200, 0);
            LogoUpdate = null;
        }

    }

    public void LogoUP()
    {
        if ((RT.anchoredPosition - InsertLocation).magnitude > 0.5f)
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
        if ((RT.anchoredPosition - LeaveLocation).magnitude > 0.5f)
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
        if ((RT.anchoredPosition - LeaveLocation).magnitude > 0.5f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, LeaveLocation, 0.06f);
        }
        else
        {
            TimeLine.Instance.Moved = true;
            TimeLine.Instance.LogoList.Remove(this);
            Destroy(gameObject);
        }
    }
}
