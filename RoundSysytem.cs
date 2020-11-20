using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundSysytem 
{
    private static RoundSysytem m_insatnce;
    public static RoundSysytem GetInstance()
    {
        if (m_insatnce != null)
        {
            return m_insatnce;
        }
        else{
            m_insatnce = new RoundSysytem();
            return m_insatnce;
        }

    }

    //GameObject[] Humans;
    //GameObject[] Aliens;
    public bool EndSignal = false;
    List<(GameObject Cha, int Speed)> Sequence;
    public void RoundPrepare(GameObject[] humans,GameObject[] aliens) 
    {
        
        //Humans = humans;
        //Aliens = aliens;
        foreach (GameObject human in humans)
        {
            int Count = 0;
            (GameObject Chr,int speed)obj = (human, human.GetComponent<Character>().Speed);
            if (Sequence == null)
            {
                Sequence = new List<(GameObject, int)>();
                Sequence.Add(obj);
                continue;
            }
            foreach((_ ,int LSpeed) in Sequence)
            {
                if (obj.Item2 > LSpeed)
                {
                    Sequence.Insert(Count,obj);
                }
                ++Count;
            }

        }
        foreach (GameObject alien in aliens)
        {
            int Count = 0;
            (GameObject Chr, int speed) obj = (alien, alien.GetComponent<Character>().Speed);
            if (Sequence == null)
            {
                Sequence = new List<(GameObject, int)>();
                Sequence.Add(obj);
                continue;
            }
            foreach ((_, int LSpeed) in Sequence)
            {
                if (obj.Item2 > LSpeed)
                {
                    Sequence.Insert(Count, obj);
                }
                ++Count;
            }

        }
    }
    public void RoundStart()
    {
        while (true)
        {
            GameObject[] ChaSeq= new GameObject[Sequence.Count];
            //右邊順序動畫
            for (int Count = 0; Count < Sequence.Count; ++Count)//載入角色順序
            {
                ChaSeq[Count] = Sequence[Count].Cha;
            }
            for(int Count =0; Count< ChaSeq.Length; ++Count)//開始行動
            {
                //ChaSeq[Count].searchenemy();
                //ChaSeq[Count].CalMove();
                ChaSeq[Count].GetComponent<Character>().AP = 2;
                while (EndSignal != true)
                {
                    System.Threading.Thread.Sleep(1);
                }
                //while(動畫還沒結束)
                //System.Threading.Thread.Sleep(1);
            }
            //事件?增援?newcome

        }
    }
    private void NewCome()
    {

    }
}
