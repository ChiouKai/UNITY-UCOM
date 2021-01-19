using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    Move_Camera MoveCam;
    public bool EndSignal = false;
    LinkedList<(AI Cha, int Speed)> Sequence;
    public List<AI> Humans;
    public List<AI> Aliens;
    UISystem UI;
    public bool EndChecked = true;
    public List<Action> EventList;

    public void RoundPrepare(List<AI> humans, List<AI> aliens, Move_Camera MC, UISystem ui) //遊戲開始前 抓取每個單位資料
    {
        Humans = humans;
        Aliens = aliens;
        UI = ui;
        MoveCam = MC;
        UI.per_but = false; //我方切換子彈預設為關
        MoveCam.att_cam_bool = false;
        UI.toggle[0].SetActive(true);
        UI.toggle[1].SetActive(false);
        UI.toggle[2].SetActive(false);
        UI.explosion.SetActive(false);
        for (int i = 0; i < UI.toggle.Length; i++)
        {
            UI.toggle[i].transform.GetChild(1).GetComponent<Text>().color = Color.white;
            UI.toggle[i].transform.GetChild(0).GetComponent<Image>().sprite = UI.mission_Images[0];
        }
        Sequence = new LinkedList<(AI, int)>();
        Sequence.AddFirst((UI.GetComponent<AI>(), 99));
        foreach (AI human in Humans)
        {
            human.GetTargets(Aliens);
            (AI Cha,int speed) obj = (human, human.GetComponent<Character>().Speed);

            InsertCha(obj);
        }
        foreach (AI alien in aliens)
        {
            alien.GetTargets(Humans);
            (AI Cha, int speed) obj = (alien, alien.GetComponent<Character>().Speed);

            InsertCha(obj);
        }
        UI.Sequence = Sequence;
        UI.UIAnima = UI.PrepareTimeLine;
        EventList = new List<Action>();
    }
    public void RoundStart()
    {
        LinkedListNode<(AI Cha, int Speed)> Current = Sequence.First;
        Sequence.RemoveFirst();
        Sequence.AddLast(Current);
        Current = Sequence.First;
        AI TurnCha;
        TurnCha = Current.Value.Cha;
        UI.TurnCha = TurnCha;
        while (true)
        {
            //wait UI 右邊順序動畫
            while(UI.TurnRun != null)
            {
                System.Threading.Thread.Sleep(1);
            }
            lock (UI.TurnCha)
            {
                UI.TurnCha = TurnCha;
            }
            if (TurnCha.Cha.camp == 0)
            {
                TurnCha.AP = 2;

                TurnCha.Turn = true;
                TurnCha.CountCD();
                UI.TurnRun = UI.PlayerStartTurn;
            }
            else
            {
                TurnCha.Turn = true;
                UI.TurnRun = UI.EnemyStartTurn;
            }


            while (TurnCha.Turn!= false|| EndChecked!= true|| TimeLine.Instance.Moved != true)
            {
                System.Threading.Thread.Sleep(2);
            }

            UI.Count = InsertCha(Current);
            UI.TurnRun = UI.ChaTurnEnd;
            UI.RunUI = UI.CloseActionUI;
            System.Threading.Thread.Sleep(1500);
            Current = Sequence.First;

            if (Current.Value.Speed == 99) //回合結束
            {
                UI.RunUI = UI.CleanGarbage;
                if (UI.Bomb_start)
                {
                    UI.Bomb_Round++;
                }
                NewEvent();//條件觸發則增加Event;
                while (TimeLine.Instance.Moved != true)
                {
                    System.Threading.Thread.Sleep(1);
                }
                DoEventList();
                while (TimeLine.Instance.Moved != true|| EndChecked != true)
                {
                    System.Threading.Thread.Sleep(1);
                }
                Sequence.RemoveFirst();
                Sequence.AddLast(Current);
                Current = Sequence.First;

                UI.TurnRun = UI.TurnEnd;

                
                //事件?增援?newcome
            }
            TurnCha = Current.Value.Cha;
        }
    }

    
    public void DoEventList()
    {
        foreach(var Event in EventList)
        {
            Event?.Invoke();
            while (TimeLine.Instance.Moved != true || EndChecked != true)
            {
                System.Threading.Thread.Sleep(1);
            }
        }
        EventList.Clear();
    }
    public void testevent()
    {
        Action Event = () => 
        { 
            EndChecked = false;
            UI.TurnRun = UI.NewCome;
            Event = null;
        };
        EventList.Add(Event);
    }
    public void NewEvent()
    {
        if (Aliens.Count < 2)
        {
            Action Event = () =>
            {
                EndChecked = false;
                UI.TurnRun = UI.NewCome;
                Event = null;
            };
            EventList.Add(Event);
        }
        if (UI.Bomb_Round == 3) 
        {
            Action Event = () =>
            {
                EndChecked = false;
                UI.RunUI = UI.StartLeave;
                Event = null;
            };
            EventList.Add(Event);
        }
    }



    void InsertCha((AI Chr, int speed) obj)//回合前排順序
    {
        LinkedListNode<(AI Cha, int Speed)> current = Sequence.Last;

        for (int Count = Sequence.Count; Count > 0; --Count)
        {
            if (obj.speed <= current.Value.Speed)
            {
                Sequence.AddAfter(current, obj);
                break;
            }
            else
                current = current.Previous;
        }
    }
    int InsertCha(LinkedListNode<(AI Cha, int Speed)> obj)//加到下回合的順序 //順便給UI知道順位
    {
        Sequence.RemoveFirst();
        LinkedListNode<(AI Cha, int Speed)> current = Sequence.Last;
        int Count;
        for (Count = Sequence.Count; Count > 0; --Count)
        {
            if (obj.Value.Speed <= current.Value.Speed)
            {
                Sequence.AddAfter(current, obj);
                break;
            }
            else
                current = current.Previous;
        }
        return Count;
    }


    public void DeathKick(AI cha)//死亡剔除名單
    {
        LinkedListNode<(AI Cha, int Speed)> Current = Sequence.Find((cha, cha.Cha.Speed));
        lock (Sequence)
        {
            Sequence.Remove(Current);
        }
        if (cha.Cha.camp == Character.Camp.Alien)
        {
            Aliens.Remove(cha);
        }
        else
        {
            Humans.Remove(cha);
        }

    }
    public int NewCome(AI Cha)
    {
        var obj = (Cha, Cha.Cha.Speed);
        LinkedListNode<(AI Cha, int Speed)> current = Sequence.Last;
        int Count = Sequence.Count;
        for (; Count > 0; --Count)
        {
            if (obj.Speed <= current.Value.Speed)
            {
                Sequence.AddAfter(current, obj);
                break;
            }
            else
                current = current.Previous;
        }
        return Count; 
    }






}
