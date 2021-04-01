using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        RS = RoundSysytem.GetInstance();
        Enemies = RS.Humans;
        Skills.AddRange(GetComponents<ISkill>());
        AIState = NpcAI;
        UI = UISystem.getInstance();
        SM = FindObjectOfType<SoundManager>();
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
            AI tmpEnemy = null;
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
        if (!Turn && enemy.Moving || ChangEnemy)
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

    public GameObject Shield;



    public void PreCharge()
    {
        ResetBool();
        Am.SetTrigger("Charge");
        NPCPrepera = false;
        DoActing = null;
    }

    public void ChargeLogo()
    {
        UI.status(1, "充能!", this);
    }
    public void Charge()
    {
        int tmp = (Cha.MaxEnergy - Cha.Energy) / 2;
        if (tmp < 3)
        {
            tmp = 3;
        }
        Cha.Energy += tmp;
        if (Cha.Energy > Cha.MaxEnergy)
        {
            Cha.Energy = Cha.MaxEnergy;
        }
        if (!Shield.activeSelf)
        {
            UI.status(1, "電能護盾!", this);
            Shield.SetActive(true);
        }
        UI.HpControl(this, Cha.HP);

        StartCoroutine(AfterCharge());
    }

    public IEnumerator AfterCharge()
    {
        yield return new WaitForSeconds(1f);
        ConfirmAction2();
    }
    



















    public override void ConfirmAction()
    {
        if (Ult)
        {
            ResetBool();
            Ult = false;
            Am.SetTrigger("Ultimate2");
        }
        else if (Cha.Energy == Cha.MaxEnergy)
        {
            ConfirmAction2();
        }
        else
        {
            DoActing = PreCharge;
        }
        NPCPrepera = true;

    }
    private void ConfirmAction2()
    {
        if (BestT != CurrentTile)
        {
            DoActing = PreMove;
            NPCPrepera = true;
        }
        else if (Acting != null)
        {
            DoActing = Acting.GetAction();
            ActionName = Acting.Name;
            Acting.EnterCD();
            Acting = null;
            NPCPrepera = true;
        }
        else
        {
            RemoveVisitedTiles();
            NPCPrepera = false;
            DoActing = null;
            RS.EndChecked = true;
            EndTurn();
        }
    }



    public override void BeDamaged(int damage)
    {
        Cha.Energy -= damage;
        if (Cha.Energy <= 0)
        {
            Cha.HP += Cha.Energy;
            Cha.Energy = 0;
        }
        Damage = damage;
        hurt = true;
    }

    public override bool Hurt(Vector3 dir)
    {
        if (!hurt)
        {
            return false;
        }
        dir.y = 0;
        UI.HpControl(this, Cha.HP);
        UI.status(2, Damage.ToString(), this);
        hurt = false;
        if (Cha.HP <= 0)
        {
            transform.forward = -dir;
            SM.Play(Cha.die);
            AIDeath();
        }
        else
        {
            if (Cha.Energy == 0)
            {
                Shield.SetActive(false);
                ResetBool();
                transform.forward = -dir;
                Am.Play("Hurt");
            }
        }
        return true;
    }

    public GameObject Chain;
    public GameObject Fall;
    public GameObject effect;
    bool Hit = false;
    public void LightingSound()
    {
        SM.Play("att_thunder1");
    }
    public void Lightning()
    {

        GameObject go = Instantiate(Bullet, FirePoint.position, Quaternion.identity);
        GameObject go2;
        LineRenderer chain = Instantiate<GameObject>(Chain).GetComponent<LineRenderer>();
        chain.positionCount = 2;
        chain.SetPosition(0, FirePoint.position);
        Cha.Energy -= 1;
        UI.status(1, "消耗 1 能量!", this);
        UI.HpControl(this, Cha.HP);
        if (Miss)
        {
            UI.status(1,"未命中!", Target);
            chain.SetPosition(1, AttackPoint);
            go2 = Instantiate(Bullet, AttackPoint, Quaternion.identity);
        }
        else
        {
            if(Target.Cha.HP>2)
                Hit = true;
            Vector3 dir = Target.transform.position - transform.position;
            dir.y = 0;
            Target.Hurt2(dir);
            chain.SetPosition(1, Target.BeAttakePoint.position);
            go2 = Instantiate(Bullet, Target.BeAttakePoint.position, Quaternion.identity);
        }
        Attack = false;

        Destroy(chain.gameObject, 0.4f);
        Destroy(go, 0.4f);
        Destroy(go2, 0.4f);
    }

    public override void PreMindControl()
    {
        ResetBool();
        
        Vector3 pos = new Vector3();
        foreach(AI target in Enemies)
        {
            pos += target.transform.position;
        }
        pos = pos / Enemies.Count;
        TargetDir = pos - transform.position;
        TargetDir.y = 0;
        Ult = true;
        Am.SetTrigger("Ultimate");
    }
    public void UltFaceTarget()
    {
        transform.forward = TargetDir;
    }


    List<Tile> UltimateTile = new List<Tile>();
    List<AI> CamAIList = new List<AI>();
    public  GameObject CamTarget;
    public void UseUltimate()
    {
        for (int i = 0; i < 4; ++i)
        {
            LineRenderer chain = Instantiate<GameObject>(Chain).GetComponent<LineRenderer>();
            chain.positionCount = 2;
            chain.SetPosition(0, transform.position + Vector3.up * 10f);
            chain.SetPosition(1, transform.position + Direction(i) * 0.335f);
            Destroy(chain.gameObject, 2f);
        }
        Cha.Energy -= 3;
        UI.HpControl(this, Cha.HP);
        UI.status(1, "消耗 3 能量!", this);
        Destroy(Instantiate<GameObject>(effect, transform), 2f);
        StartCoroutine(PreUltimate());
    }
    IEnumerator PreUltimate()
    {
        yield return new WaitForSeconds(1f);
        foreach(AI target in Enemies)
        {
            Tile T = target.CurrentTile;
            UltimateTile.Add(T);
            T.DangerPos();
            UI.JoinActionTile(T);
            for(int i = 0; i < 8; ++i)
            {
                T.AdjList[i].DangerPos();
                UI.JoinActionTile(T.AdjList[i]);
            }
            CamAIList.Add((Instantiate<GameObject>(CamTarget,T.transform.position,Quaternion.identity).GetComponent<AI>()));
            UI.MoveCam.ChaTurn(target);
            yield return new WaitForSeconds(1f);
        }
        EndTurn();
    }
    public void PreUltimated()
    {
        StartCoroutine(Ultimated());
    }

    public IEnumerator Ultimated()
    {

        for(int i = 0; i < UltimateTile.Count; ++i)
        {
            UI.LeaveActionTile(UltimateTile[i]);
            UltimateTile[i].Recover();
            UI.MoveCam.ChaTurn(CamAIList[i]);
            AI cha;
            Destroy(Instantiate<GameObject>(Fall, UltimateTile[i].transform.position,Quaternion.identity), 2f);
            SM.Play("att_thunder2");
            if (UltimateTile[i].Cha != null && UltimateTile[i].Cha.name != "Boss")
            {
                cha = UltimateTile[i].Cha;
                cha.BeDamaged(4);
                cha.Hurt(-UltimateTile[i].Cha.transform.forward);
            }

            for (int j = 0; j < 8; ++j)
            {
                UI.LeaveActionTile(UltimateTile[i].AdjList[j]);
                UltimateTile[i].AdjList[j].Recover();
                if (UltimateTile[i].AdjList[j].Cha != null && UltimateTile[i].AdjList[j].Cha.name != "Boss")
                {
                    cha = UltimateTile[i].AdjList[j].Cha;
                    cha.BeDamaged(4);
                    cha.Hurt(UltimateTile[i].AdjList[j].transform.position - UltimateTile[i].transform.forward);
                }
                UI.LeaveActionTile(UltimateTile[i].AdjList[j]);
                UltimateTile[i].AdjList[j].Recover();
            }

            yield return new WaitForSeconds(1f);
        }


        foreach(var cam in CamAIList)
        {
            Destroy(cam.gameObject);
        }
        CamAIList.Clear();
        ConfirmAction();
        UI.MoveCam.ChaTurn(this);
    }

    public override void EndTurn()
    {
        if (Cha.Energy > 6 && Hit)
        {
            UI.MoveCam.att_cam_bool = false;
            UI.MoveCam.ChaTurn(this);
            PreMindControl();
            Hit = false;
            return;
        }
        base.EndTurn();
    }
}
