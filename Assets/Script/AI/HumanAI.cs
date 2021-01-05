﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HumanAI : AI
{
    // Start is called before the first frame update
    void Start()
    {
        CurrentTile.walkable = false;
        Vector3 CTP = CurrentTile.transform.position;
        ChaHeight = transform.position.y - CTP.y;
        CTP.y = transform.position.y;
        transform.position = CTP;
        Cha = GetComponent<Character>();
        Am = GetComponent<Animator>();
        EnemyLayer = 1 << 10;
        Gun = GetComponent<Weapon>();
        TileCount = FindDirection(transform.forward);
        Idle = NoCover;
        UI = UISystem.getInstance();
        Enemies = RoundSysytem.GetInstance().Aliens;
    }

    // Update is called once per frame
    void Update()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);

        if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            Move();
        }
        else if (Target==null&&!Am.GetBool("Run"))
        {
            Idle();
        }
       
    }
    private void LateUpdate()
    {
        
        if (PreAttack)
        {
            PreAttakeIdle();
        }
    }
    private void FixedUpdate()
    {
        float MinDis = 99f;
        foreach (AI EnCha in Enemies)
        {
            float dis = (EnCha.transform.position - transform.position).magnitude;
            if (dis < MinDis)
            {
                MinDis = dis;
                enemy = EnCha.transform;
            }
        }
    }


    public override void LeftTurn()
    {
        AmTurn = false;
        Am.SetBool("Left", false);
        Am.SetBool("Turn", false);
        //transform.Rotate(0, -90f, 0);
    }
    public override void RightTurn()
    {
        AmTurn = false;
        Am.SetBool("Right", false);
        Am.SetBool("Turn", false);
        //transform.Rotate(0, 90f, 0);
    }
    public override void BackTurn()
    {
        AmTurn = false;
        Am.SetBool("Back", false);
        Am.SetBool("Turn", false);
        //transform.Rotate(0, 180f, 0);
    }





}