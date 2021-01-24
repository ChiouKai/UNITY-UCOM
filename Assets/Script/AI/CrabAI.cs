using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAI : AI
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
        RS = RoundSysytem.GetInstance();
        Enemies = RS.Humans;
        Skills.AddRange(GetComponents<ISkill>());
        Idle = NoCover;
        UI = UISystem.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);

        if (!Turn)
        {
            Idle();
        }
        else if (Attack)
        {
            Melee();
        }
        else if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            Move2();
        }
        else if (NPCPrepera)
        {
            DoActing?.Invoke();
        }
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
        if (AmTurn && stateinfo.IsName("Turn"))
        {
            Ediv = (enemy.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Ediv), 0.05f);
            float FoB = Vector3.Dot(transform.forward, Ediv);
            if (FoB > 0.99f)
            {
                transform.forward = Ediv;
                Am.SetBool("Turn", false);
                AmTurn = false;
            }
        }
    }



    protected override void NoCover()
    {
        if (!AmTurn && stateinfo.IsName("Idle"))
        {
            Ediv = (enemy.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, Ediv);

            if (FoB > 1 / Mathf.Sqrt(2))   //判斷前後左右
            {
                return;
            }
            else
            {
                Am.SetBool("Turn", true);
                AmTurn = true;
            }
        }
    }


    public override void FindSelectableTiles(int ap)
    {
        if (VisitedTiles.Count > 0)
            return;
        //BFS 寬度優先使用Queue
        BestPoint = 0;
        Queue<Tile> Process = new Queue<Tile>();
        if (CalPointAction(CurrentTile))
        {
            return;
        }

        Process.Enqueue(CurrentTile);
        AddVisited(CurrentTile);

        while (Process.Count > 0)
        {
            Tile T = Process.Dequeue();
            foreach (Tile adjT in T.AdjList)
            {
                if (!adjT.visited && adjT.walkable)
                {
                    Vector3 vdiv = adjT.transform.position - T.transform.position;
                    vdiv.y = 0;
                    float TDis = vdiv.magnitude;
                    if (TDis > 0.9f)        //如果斜向移動，則判斷路徑上有沒有東西卡到
                    {
                        if (Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, LayerMask.GetMask("Environment"))
                        || Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, EnemyLayer))
                        {
                            continue;
                        }
                    }
                    adjT.distance = TDis + T.distance;
                    adjT.Parent = T;  //visited過的就被設為 parent
                    AddVisited(adjT);

                    if (adjT.distance > Cha.Mobility * ap) //移動距離不會超過上限
                    {
                        continue;
                    }

                    if (CalPointAction(adjT))
                    {
                        return;
                    }
                    Process.Enqueue(adjT);
                }
            }
        }
    }



    protected override bool CalPointAction(Tile T)
    {
        float Point = 0;
        Vector3 Location = T.transform.position;

        float MinDis = 99;
        if (Enemies == null)
        {
            Enemies = RoundSysytem.GetInstance().Humans;
        }
        foreach (AI enemy in Enemies)
        {
            Vector3 Edir = enemy.transform.position - Location;

            if (MinDis > Edir.magnitude)
            {
                MinDis = Edir.magnitude;
            }
        }
        float i = 32f / (MinDis + 1f);

            Point += i;
        //可用能力巡一遍，選擇得分高的能力 再拿出來加分
        float SecPoint = 0;
        for (int j = 0; j < 4; ++j)
        {
            if (T.AdjList[j].Cha != null && EnemyLayer != T.AdjList[j].Cha.EnemyLayer)
            {
                Target = T.AdjList[j].Cha;
                SecPoint += 10;
                break;
            }
        }

        //todo特殊能力 先確認CD 如果可以用 在計算命中 得分會比普通射擊高一些
        Point += SecPoint;
        if (Point > BestPoint)
        {
            BestT = T;
            BestPoint = Point;
            if (Target != null)
            {
                return true;
            }
        }
        return false;
    }

    public override void ConfirmAction()
    {
        if (Target != null)
        {
            DoActing = PreMelee2;
        }
        else
        {
            DoActing = PreMove;
        }

        NPCPrepera = true;
    }

    protected override void PreMove()
    {
        MoveToTile(BestT);
        Am.SetBool("Run", true);
        AmTurn = false;
        Am.SetBool("Turn", false);
        Moving = true;
        RemoveVisitedTiles();//重置Tile狀態
        OutCurrentTile();
        InCurrentTile(BestT);
        DoActing = null;
    }

    public override void Move2()
    {
        //if path.Count > 0, move, or remove selectable tiles, disable moving and end the turn. 
        if (Moving != true)
        {
            return;
        }
        if (Path.Count > 0)
        {
            (Tile T, MoveWay M) = Path.Peek();
            Vector3 target = T.transform.position;
            target.y += ChaHeight;

            if ((transform.position - target).magnitude >= 0.05f)
            {
                Heading = target - transform.position;
                Heading.Normalize();
                transform.forward = Heading;
                transform.position += Heading * MoveSpeed * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                Path.Pop();
            }
        }
        else
        {

            Am.SetBool("Run", false);
            Moving = false;
            PreAttack = false;
            NPCPrepera = false;
            AP = 0;
            RS.EndChecked = true;
            StartCoroutine(WaitNextAction());
        }
    }








    public override void PreMelee2()
    {
        MoveToTile(BestT);
        if (Path.Count != 0)
        {
            Am.SetBool("Run", true);

        }
        Moving = true;
        AmTurn = false;
        Am.SetBool("Turn", false);

        RemoveVisitedTiles();//重置Tile狀態
        OutCurrentTile();
        InCurrentTile(BestT);
        DoActing = null;
        Attack = true;
    }

    public override void Melee()
    {
        if (Moving != true)
        {
            return;
        }
        //if path.Count > 0, move, or remove selectable tiles, disable moving and end the turn. 

        if (Path.Count > 0)
        {
            (Tile T, MoveWay M) = Path.Peek();
            Vector3 target = T.transform.position;
            target.y += ChaHeight;
            if ((transform.position - target).magnitude >= 0.05f)
            {
                Heading = target - transform.position;
                Heading.Normalize();
                transform.forward = Heading;
                transform.position += Heading * MoveSpeed * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                Path.Pop();
            }
        }
        else
        {

            Am.SetBool("Run", false);
            Vector3 TargetDir = Target.transform.position - transform.position;
            TargetDir.y = 0;
            transform.forward = TargetDir;
            Moving = false;
            Attack = false;
            Am.SetTrigger("Melee");
            AP = 0;
            StartCoroutine(WaitMelee2());            
        }
    }

    protected IEnumerator WaitMelee2()
    {

        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<SoundManager>().Play(Cha.affirmative);
        Target.BeDamaged(3);
        Target.Hurt(transform.forward);
        UI.status("Demage", this);
        yield return new WaitForSeconds(1f);//
        RS.EndChecked = true;
        StartCoroutine(WaitNextAction());
    }

    public override void Hurt(Vector3 dir)
    {
        dir.y = 0;
        if (Cha.HP <= 0)
        {
            transform.forward = -dir;
            UI.HpControl(this, Cha.HP);
            AIDeath();
        }
        else
        {
            transform.forward = -dir;
            UI.HpControl(this, Cha.HP);
            Am.SetBool("Turn", false);
            AmTurn = false;
            Am.Play("Hurt");
            FindObjectOfType<SoundManager>().Play(Cha.takeHit);
        }
    }
    protected override void AIDeath()
    {
        OutCurrentTile();
        Am.Play("Death");
        FindObjectOfType<SoundManager>().Play(Cha.die);
        RS.DeathKick(this);
        TimeLine.Instance.Moved = false;
        UI.DeathKick(this);
        Destroy(GetComponent<EPOOutline.Outlinable>());
        Destroy(Cha);
        //Idle = DeathIdle;
        FindObjectOfType<SoundManager>().Play(Cha.die);
    }
    private void DeathIdle()
    {

        transform.localScale = transform.localScale * 0.9f;
        if (transform.localScale.x<0.3)
        {
            Destroy(this);
        }

    }
}
