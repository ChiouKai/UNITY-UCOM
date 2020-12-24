using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLine : MonoBehaviour
{

    Dictionary<AI, LogoScript> LogoDic;
    public LinkedList<LogoScript> LogoList;
    public static TimeLine Instance;
    public bool Moved;
    private void Start()
    {
        Moved = false;
        Instance = this;
        LogoDic = new Dictionary<AI, LogoScript>();
        LogoList = new LinkedList<LogoScript>();
    }

    private void Update()
    {
        foreach (LogoScript logo in LogoList)
        {
            logo.LogoUpdate?.Invoke();
        }
    }





    public void NewLogo(AI Cha, GameObject Logo,int Count)
    {
        LogoScript logo = Instantiate<GameObject>(Logo).GetComponent<LogoScript>();
        logo.transform.SetParent(transform,false);
        logo.GetComponent<RectTransform>().anchoredPosition =  new Vector2( 75 , -50 - 110 * Count);
        LogoList.AddLast(logo);
        LogoDic.Add(Cha, logo);
    }


    public void TEndLogo(AI Cha, int Count)
    {
        LogoScript logo;
        if(LogoDic.TryGetValue(Cha, out logo))
        {
            logo.InsertLocation = new Vector2( -25, -50 - 110 * Count);
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
            logo.LogoUpdate = logo.LogoUP;
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
                logo.LogoUpdate = logo.LogoUP;
                Current = Current.Next;
            }
        }
        else
        {
            Debug.LogError("LogoError");
        }
    }

}
