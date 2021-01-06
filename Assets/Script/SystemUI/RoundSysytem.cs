﻿using System;
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
    Move_Camera MoveCam;
    public bool EndSignal = false;
    LinkedList<(AI Cha, int Speed)> Sequence;
    public List<AI> Humans;
    public List<AI> Aliens;
    UISystem UI;
    public bool DeathChecked = true;

    public void RoundPrepare(List<AI> humans, List<AI> aliens, Move_Camera MC, UISystem ui) //遊戲開始前 抓取每個單位資料
    {
        Humans = humans;
        Aliens = aliens;
        UI = ui;
        MoveCam = MC;
        MoveCam.cam_dis = 20.0f;//一開始預設攝影機距離為20公尺
        UI.per_but = false; //我方切換子彈預設為關
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
    }
    public void RoundStart()
    {
        LinkedListNode<(AI Cha, int Speed)> Current = Sequence.First;
        Sequence.RemoveFirst();
        Sequence.AddLast(Current);
        Current = Sequence.First;
        while (true)
        {
            //wait UI 右邊順序動畫
            AI TurnCha = Current.Value.Cha;
            while(UI.TurnRun != null)
            {
                System.Threading.Thread.Sleep(1);
            }
            UI.TurnCha = TurnCha;
            
            if (TurnCha.Cha.camp == 0)
            {
                UI.TurnRun = UI.PlayerStartTurn;
                
                TurnCha.Turn = true;
                TurnCha.AP = 2;
            }
            else
            {
                TurnCha.Turn = true;
                UI.TurnRun = UI.EnemyStartTurn;
            }


            MoveCam.ChaTurn(TurnCha);
            MoveCam.cam_dis = 20.0f;//一開始預設攝影機距離為20公尺
            UI.per_but = false; //我方切換子彈預設為關
            TurnCha.NPC_Prefire = false;
            MoveCam.att_cam_bool = false;

            while (TurnCha.Turn!= false|| DeathChecked!= true|| TimeLine.Instance.Moved != true)
            {
                System.Threading.Thread.Sleep(1);
            }

            UI.Count = InsertCha(Current);
            UI.TurnRun = UI.ChaTurnEnd;
            UI.RunUI = UI.CloseActionUI;
            TimeLine.Instance.Moved = false;
            System.Threading.Thread.Sleep(1500);
            Current = Sequence.First;
            if (Current.Value.Speed == 99) //回合結束
            {
                Sequence.RemoveFirst();
                Sequence.AddLast(Current);
                Current = Sequence.First;

                while (TimeLine.Instance.Moved != true)
                {
                    System.Threading.Thread.Sleep(1);
                }
                UI.TurnRun = UI.TurnEnd;

                TimeLine.Instance.Moved = false;

                
                //事件?增援?newcome
            }

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
        DeathChecked = false;
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
    private void NewCome((AI Cha ,int speed) obj)
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






}
