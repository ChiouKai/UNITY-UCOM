using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAi : AI
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
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);

        if (!Turn)
        {
            NoCover();
        }
        else if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            if (Attack)
            {
                //melee
            }
            else 
            {
                Move2();
            }

        }
        else if (NPCPreaera)
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




    protected override void NoCover()
    {
        if (!AmTurn && stateinfo.IsName("Idle"))
        {
            transform.forward = Direction(TileCount);
            Ediv = (enemy.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, Ediv);

            if (FoB > 1 / Mathf.Sqrt(2) - 0.1f)   //判斷前後左右
            {
                return;
            }
            else if (FoB < -1 / Mathf.Sqrt(2))
            {
                Am.SetBool("Back", true);
                Am.SetBool("Turn", true);
                AmTurn = true;
                TileCount = (TileCount + 2) % 4;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                if (LoR.y > 0.0f)
                {
                    Am.SetBool("Right", true);
                    Am.SetBool("Turn", true);
                    AmTurn = true;
                    TileCount = (TileCount + 3) % 4;
                }
                else
                {
                    Am.SetBool("Left", true);
                    Am.SetBool("Turn", true);
                    AmTurn = true;
                    TileCount = (TileCount + 1) % 4;
                }
            }
        }
    }


    public override void FindSelectableTiles()
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
                    if (adjT.distance > Cha.Mobility * 2) //移動距離不會超過上限
                    {
                        continue;
                    }
                    adjT.Parent = T;  //visited過的就被設為 parent
                    AddVisited(adjT);
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
        
        foreach (AI enemy in Enemies)
        {
            Vector3 Edir = enemy.transform.position - Location;
            
            if (MinDis > Edir.magnitude)
            {
                MinDis = Edir.magnitude;
            }
        }
        int i = 16 / ((int)MinDis + 1);
        if (i > 4)
        {
            Point += 8;
        }
        else
        {
            Point += i;
        }
        //可用能力巡一遍，選擇得分高的能力 再拿出來加分
        float SecPoint = 0;
        for (i = 0; i < 4; ++i) 
        {
            if (T.AdjList[i].Cha != null && EnemyLayer != T.AdjList[i].Cha.EnemyLayer) 
            {
                Target = T.AdjList[i].Cha;
                SecPoint += 3;
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
            DoActing = PreMelee;
        }
        else 
        {
            DoActing = PreMove;
        }

        NPCPreaera = true;
    }





}
