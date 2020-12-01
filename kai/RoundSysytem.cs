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

    public bool EndSignal = false;
    LinkedList<(AI Cha, int Speed)> Sequence;
    //Dictionary<AI ,int> ChaSpeed;
    //List<(AI Cha, int Speed)> Order;


    public void RoundPrepare(AI[] humans,AI[] aliens) 
    {
        Sequence = new LinkedList<(AI, int)>();
        Sequence.AddFirst((null, 99));
        foreach (AI human in humans)
        {
            
            (AI Cha,int speed) obj = (human, human.GetComponent<Character>().Speed);

            InsertCha(obj);
        }
        foreach (AI alien in aliens)
        {
            (AI Cha, int speed) obj = (alien, alien.GetComponent<Character>().Speed);

            InsertCha(obj);
        }
    }
    public void RoundStart()
    {
        LinkedListNode<(AI Cha, int Speed)> Current = Sequence.First.Next;
        while (true)
        {
            //wait UI 右邊順序動畫 
            Current.Value.Cha.Turn = true;
            Current.Value.Cha.AP = 2;

            while (Current.Value.Cha.Turn!= false)
            {
                System.Threading.Thread.Sleep(1);
            }
            Current = Current.Next;
            if (Current == null) //回合結束
            {
                Current = Sequence.First.Next;
                //事件?增援?newcome
            }

        }
    }

    void InsertCha((AI Chr, int speed) obj)
    {
        LinkedListNode<(AI Cha, int Speed)> current = Sequence.Last;

        for (int Count = Sequence.Count; Count > 0; --Count)
        {
            if (obj.speed < current.Value.Speed)
            {
                Sequence.AddAfter(current, obj);
                break;
            }
            else
                current = current.Previous;
        }
    }
    private void DeathKick(AI cha)
    {
        LinkedListNode<(AI Cha, int Speed)> current = Sequence.Last;
        lock (Sequence)
        {
            while (true)
            {
                if (current.Value.Cha == cha)
                {
                    Sequence.Remove(current);
                    break;
                }
                current = current.Previous;
            }
        }
    }
    private void NewCome()
    {

    }
}
