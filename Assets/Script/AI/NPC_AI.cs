using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_AI : AI
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
        EnemyLayer = 1 << 11;
        Gun = GetComponent<Weapon>();
        TileCount = FindDirection(transform.forward);
        Idle = NoCover;
        Enemies = RoundSysytem.GetInstance().Humans;
        Skills = GetComponents<ISkill>();
        AIState = NpcAI;
        UI = UISystem.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        AIState();
    }
    private void FixedUpdate()
    {
        if (!Turn && !BeAimed)
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
            Ediv = (enemy.position - transform.position).normalized;
        }
    }
    private void LateUpdate()
    {
        if (PreAttack)
        {
            PreAttakeIdle();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(enemy.position + Vector3.up * 1.6f, 0.2f);
    }











}