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
        InsertLocation = RT.anchoredPosition + new Vector2(-100, 0);
        LogoUpdate = LogoInsert;
    }


    public Action LogoUpdate;

    public void LogoInsert()
    {
        if ((RT.anchoredPosition- InsertLocation).magnitude > 0.01f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, InsertLocation, 0.05f);
        }
        else
        {
            TimeLine.Instance.Moved = true;
            RT.anchoredPosition = InsertLocation;
            LeaveLocation = RT.anchoredPosition + new Vector2(120, 0);
            LogoUpdate = null;
        }

    }

    public void LogoUP()
    {
        if ((RT.anchoredPosition - InsertLocation).magnitude > 0.01f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, InsertLocation, 0.03f);
        }
        else
        {
            RT.anchoredPosition = InsertLocation;
            LeaveLocation = RT.anchoredPosition + new Vector2(120, 0);
            LogoUpdate = null;
        }
    }

    public void LogoLeave()
    {
        if ((RT.anchoredPosition - LeaveLocation).magnitude > 0.01f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, LeaveLocation, 0.05f);
        }
        else
        {
            RT.anchoredPosition = InsertLocation + new Vector2(120, 0);
            LogoUpdate = LogoInsert;
        }
    }
    public void LogoDeath()
    {
        if ((RT.anchoredPosition - LeaveLocation).magnitude > 0.1f)
        {
            RT.anchoredPosition = Vector2.Lerp(RT.anchoredPosition, LeaveLocation, 0.05f);
        }
        else
        {
            TimeLine.Instance.LogoList.Remove(this);
            Destroy(gameObject);
        }
    }
}
