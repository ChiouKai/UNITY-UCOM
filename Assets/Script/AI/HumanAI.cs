using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HumanAI : AI
{
    
    // Start is called before the first frame update
    void Start()
    {
        InCurrentTile(CurrentTile);
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
        Skills.AddRange(GetComponents<ISkill>());
        AIState = PlayerAI;
    }

    // Update is called once per frame
    void Update()
    {
        AIState();
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
        if (!BeAimed)
        {
            float MinDis = 99f;
            if (Enemies.Count > 0)
            {
                foreach (AI EnCha in Enemies)//
                {
                    float dis = (EnCha.transform.position - transform.position).magnitude;
                    if (dis < MinDis)
                    {
                        MinDis = dis;
                        enemy = EnCha.transform;
                    }
                }
            }
            else
            {
                enemy = null;
            }
        }
    }

}
