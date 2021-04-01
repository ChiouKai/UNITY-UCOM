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
        RS = RoundSysytem.GetInstance();
        Enemies = RS.Humans;
        Skills.AddRange(GetComponents<ISkill>());
        AIState = NpcAI;
        UI = UISystem.getInstance();
        SM= FindObjectOfType<SoundManager>(); 

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
            AI tmpEnemy=null;
            foreach (AI EnCha in Enemies)
            {
                float dis = (EnCha.transform.position - transform.position).magnitude;
                if (dis < MinDis)
                {
                    MinDis = dis;
                    tmpEnemy = EnCha;
                }
            }
            if (tmpEnemy != enemy)
            {
                enemy = tmpEnemy;
                ChangEnemy = true;
            }
        }
        if (!Turn&&enemy.Moving|| ChangEnemy)
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









}