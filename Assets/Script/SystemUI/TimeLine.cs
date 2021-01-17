using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLine : MonoBehaviour
{

    Dictionary<AI, LogoScript> LogoDic;
    public LinkedList<LogoScript> LogoList;
    public static TimeLine Instance;
    public bool Moved;
    public Action acting;
    private void Start()
    {
        Moved = false;
        Instance = this;
        LogoDic = new Dictionary<AI, LogoScript>();
        LogoList = new LinkedList<LogoScript>();
    }

    private void LateUpdate()
    {
        foreach (LogoScript logo in LogoList)
        {
            logo.LogoUpdate?.Invoke();
        }
        acting?.Invoke();
    }





    public void NewLogo(AI Cha, GameObject Logo,int Count)
    {
        LogoScript logo = Instantiate<GameObject>(Logo).GetComponent<LogoScript>();
        logo.transform.SetParent(transform,false);
        logo.GetComponent<RectTransform>().anchoredPosition =  new Vector2( 140 , -50-110 * Count);
        LogoList.AddLast(logo);
        LogoDic.Add(Cha, logo);
    }
    public void NewComeLogo(AI Cha, GameObject Logo, int Count)
    {
        LogoScript logo = Instantiate<GameObject>(Logo).GetComponent<LogoScript>();
        logo.transform.SetParent(transform, false);
        logo.GetComponent<RectTransform>().anchoredPosition = new Vector2(140, -50 - 110 * Count);
        var Current = LogoList.First;
        for(int i = 0; i < Count; ++i)
        {
            Current = Current.Next;
        }
        LogoList.AddBefore(Current, logo);
        LogoDic.Add(Cha, logo);
        while (Current != null)
        {
            logo = Current.Value;
            logo.InsertLocation += new Vector2(0, -110);
            logo.LogoUpdate = logo.LogoMove;
            Current = Current.Next;
        }


    }

    public void TEndLogo(AI Cha, int Count)
    {
        LogoScript logo;
        if(LogoDic.TryGetValue(Cha, out logo))
        {
            logo.InsertLocation = new Vector2(-60, -50 - 110 * Count);
            logo.LogoUpdate = logo.LogoLeave;
        }
        else
        {
            Debug.LogError("LogoError");
        }
        var Current = LogoList.First;
        LogoList.RemoveFirst();
        LogoList.AddLast(Current);
        Current = LogoList.First;
        for (int i =0; i <Count; ++i)
        {
            logo = Current.Value;
            logo.InsertLocation += new Vector2(0, 110);
            logo.LogoUpdate = logo.LogoMove;
            Current = Current.Next;
        }

    }
    public void DestoryLogo(AI Cha)
    {
        LogoScript logo;
        if (LogoDic.TryGetValue(Cha, out logo))
        {
            logo.LogoUpdate = logo.LogoDeath;
            var Previous = LogoList.Find(logo);
            var Current = Previous.Next;
            while (Current != null)
            {
                logo = Current.Value;
                logo.InsertLocation += new Vector2(0, 110);
                logo.LogoUpdate = logo.LogoMove;
                Current = Current.Next;
            }
        }
        else
        {
            Debug.LogError("LogoError");
        }
    }
    public void ChangeLogo(AI Cha)
    {
        LogoScript logo;
        if (LogoDic.TryGetValue(Cha, out logo))
        {
            LogoScript newlogo;
            LogoDic.Remove(Cha);
            var tem = LogoList.First;
            int i = 0;
            while (tem.Value != logo)
            {
                ++i;
                tem = tem.Next;
            }
            if (Cha.Cha.camp == Character.Camp.Alien)
            {
                newlogo = Instantiate<GameObject>(Resources.Load<GameObject>("Enemy"+Cha.name+"Logo")).GetComponent<LogoScript>();
            }
            else
            {
                newlogo = Instantiate<GameObject>(Resources.Load<GameObject>(Cha.name+"Logo")).GetComponent<LogoScript>();
            }
            newlogo.transform.SetParent(transform, false);
            newlogo.GetComponent<RectTransform>().anchoredPosition = new Vector2(140, -50 - 110 * i);
            LogoList.AddBefore(tem, newlogo);
            LogoList.Remove(tem);
            Destroy(logo.gameObject);
            LogoDic.Add(Cha, newlogo);
            RoundSysytem.GetInstance().EndChecked = true;
        }
    }
}
