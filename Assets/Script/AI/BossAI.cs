using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : AI
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

    public GameObject Shield;





    private void Charge()
    {
        Cha.Energy += 3;
        if (Cha.Energy > Cha.MaxEnergy)
        {
            Cha.Energy = Cha.MaxEnergy;
        }
        //am.settriger
        NPCPrepera = false;

    }//ui ConfirmAction2()

    public void AfterCharge()
    {
        //UI
        //confirmaction2
    }
    



















    public override void ConfirmAction()
    {
        DoActing = Charge;
    }
    private void ConfirmAction2()
    {
        if (BestT != CurrentTile)
        {
            DoActing = PreMove;
        }
        else if (Acting != null)
        {
            DoActing = Acting.GetAction();
            ActionName = Acting.Name;
            Acting.EnterCD();
            Acting = null;
        }
        //if (Acting2 == PreFire)//Target!=null
        //{
        //ChangePreAttakeIdle();
        //}//else if (Acting2==reload)
        NPCPrepera = true;
    }










}
