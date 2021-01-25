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



    public void PreCharge()
    {
        ResetBool();
        Am.SetTrigger("Charge");
        NPCPrepera = false;
        DoActing = null;
    }


    public void Charge()
    {
        Cha.Energy += 3;
        if (Cha.Energy > Cha.MaxEnergy)
        {
            Cha.Energy = Cha.MaxEnergy;
        }
        if (!Shield.activeSelf)
        {
            Shield.SetActive(true);
        }
        UI.HpControl(this, Cha.HP);

        StartCoroutine(AfterCharge());
    }//ui ConfirmAction2()

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
            Shield.SetActive(false);
            Cha.Energy = 0;
        }
        UI.demage = damage;
    }

    public override void Hurt(Vector3 dir)
    {
        dir.y = 0;
        if (Cha.HP <= 0)
        {
            transform.forward = -dir;
            UI.HpControl(this, Cha.HP);
            Am.Play("Death");
            AIDeath();
        }
        else
        {

            UI.HpControl(this, Cha.HP);
            if (Cha.Energy == 0)
            {
                ResetBool();
                transform.forward = -dir;
                Am.Play("Hurt");
            }
        }
    }

    public GameObject Chain;
    public GameObject Fall;
    bool Hit = false;
    public void Lightning()
    {
        GameObject go = Instantiate(Bullet, FirePoint.position, Quaternion.identity);
        GameObject go2;
        LineRenderer chain = Instantiate<GameObject>(Chain).GetComponent<LineRenderer>();
        chain.positionCount = 2;
        chain.SetPosition(0, FirePoint.position);
        if (Miss)
        {
            UI.status("Miss", this);
            chain.SetPosition(1, AttackPoint);
            go2 = Instantiate(Bullet, AttackPoint, Quaternion.identity);
        }
        else
        {
            Hit = true;
            UI.status("coma", this);
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
        Cha.Energy -= 3;
        UI.HpControl(this, Cha.HP);
        //instiate
        StartCoroutine(Ultimating());
    }
    IEnumerator Ultimating()
    {
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
            CamAIList.Add((Instantiate<GameObject>(CamTarget).GetComponent<AI>()));
            UI.MoveCam.ChaTurn(target);
            yield return new WaitForSeconds(1f);
        }

    }
    IEnumerator Ultimatied()
    {
        for(int i = 0; i < UltimateTile.Count; ++i)
        {
            UI.LeaveActionTile(UltimateTile[i]);
            UltimateTile[i].Recover();
            AI cha;
            if (UltimateTile[i].Cha != null && UltimateTile[i].Cha.name != "Boss")
            {
                cha = UltimateTile[i].Cha;
                cha.BeDamaged(4);
                cha.Hurt(-UltimateTile[i].Cha.transform.forward);
                Vector3 vScreenPos = Camera.main.WorldToScreenPoint(cha.BeAttakePoint.transform.position);
                vScreenPos += Vector3.right * Random.Range(100, 200) + Vector3.up * 100f;
                GameObject go = Instantiate(UISystem.getInstance().status_UI[0]) as GameObject;
                go.transform.GetChild(2).GetComponent<Text>().text = "4";
                go.transform.position = vScreenPos;
                go.transform.SetParent(UISystem.getInstance().HPCanvas.transform);
                Destroy(go, 2f);

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
                    Vector3 vScreenPos = Camera.main.WorldToScreenPoint(cha.BeAttakePoint.transform.position);
                    vScreenPos += Vector3.right * Random.Range(100, 200) + Vector3.up * 100f;
                    GameObject go = Instantiate(UISystem.getInstance().status_UI[0]) as GameObject;
                    go.transform.GetChild(2).GetComponent<Text>().text = "4";
                    go.transform.position = vScreenPos;
                    go.transform.SetParent(UISystem.getInstance().HPCanvas.transform);
                    Destroy(go, 2f);
                }
            }
            //UI.MoveCam.ChaTurn();
            yield return new WaitForSeconds(1f);


        }





    }

    public override void EndTurn()
    {
        if (Cha.Energy > 7 && Hit)
        {
            PreMindControl();
            Hit = false;
            return;
        }
        base.EndTurn();
    }
}
