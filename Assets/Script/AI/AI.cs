using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class AI : MonoBehaviour
{
    internal Animator Am;
    protected AnimatorStateInfo stateinfo;
    internal Transform enemy;
    public delegate IEnumerator Movement();
    protected Action Idle = null;
    public Character Cha;
    internal int AP = 0;
    protected Vector3 Ediv;
    internal int EnemyLayer;
    protected int TileCount;
    internal bool AmTurn = false;
    protected RoundSysytem RS;
    
    
    //動畫
    protected virtual void NoCover()//沒Cover的狀態
    {
        if (!AmTurn && stateinfo.IsName("Idle"))
        {
            if (enemy == null)
            {
                return;
            }
            TileCount = FindDirection(transform.forward);
            transform.forward = Direction(TileCount);
            Ediv = (enemy.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, Ediv);

            if (FoB > 1 / Mathf.Sqrt(2) - 0.1f)   //判斷前後左右
            {
                TileCount = FindDirection(transform.forward);
                if (CurrentTile.AdjCoverList[TileCount] == Tile.Cover.None)
                {
                    if (FoB> 0.95f)
                    {
                        return;
                    }
                    Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                    int i = TileCount;
                    if (LoR.y > 0.0f)
                    {
                        i = (i + 3) % 4;
                    }
                    else
                    {
                        i = (i + 1) % 4;
                    }

                    if (CurrentTile.AdjCoverList[i] == Tile.Cover.None)
                    {
                        return;
                    }
                    else if (CurrentTile.AdjCoverList[i] == Tile.Cover.HalfC)
                    {
                        Am.SetBool("HCover", true);
                        Idle = HalfCover;
                        PreAttakeIdle = PreAtkHalfCover;
                        TileCount = i;
                    }
                    else
                    {
                        LoR = Vector3.Cross(Direction(i), Ediv);
                        if (LoR.y >= 0)
                        {
                            Am.SetBool("Right", true);
                        }
                        else
                        {
                            Am.SetBool("Left", true);
                        }
                        Idle = FullCover;
                        PreAttakeIdle = PreAtkFullCover;
                        Am.SetBool("FCover", true);
                        TileCount = i;
                    }

                }
                else if (CurrentTile.AdjCoverList[TileCount] == Tile.Cover.HalfC)
                {
                    Am.SetBool("HCover", true);
                    Idle = HalfCover;
                    PreAttakeIdle = PreAtkHalfCover;
                }
                else
                {
                    Vector3 LoR = Vector3.Cross(Direction(TileCount), Ediv);
                    if (LoR.y >= 0)
                    {
                        Am.SetBool("Right", true);
                    }
                    else
                    {
                        Am.SetBool("Left", true);
                    }
                    Idle = FullCover;
                    PreAttakeIdle = PreAtkFullCover;
                    Am.SetBool("FCover", true);
                }
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


    protected void HalfCover()
    {
        if (!AmTurn && stateinfo.IsName("Crouch"))//
        {
            if (enemy == null)
            {
                return;
            }
            Ediv = (enemy.position - transform.position).normalized;
            int EDir = FindDirection(Ediv);
            float CFoB = Vector3.Dot(Direction(TileCount), Ediv);

            float FoB = Vector3.Dot(transform.forward, Ediv);

            if (FoB > 1 / Mathf.Sqrt(2) - 0.01f)
            {
                if (CFoB > 0.01f)
                {
                    return;
                }
                else
                {
                    if (CurrentTile.AdjCoverList[EDir] == Tile.Cover.None)
                    {
                        if (FoB > 0.99f)
                        {
                            Idle = NoCover;
                            PreAttakeIdle = PreAtkNoCover;
                            Am.SetBool("HCover", false);
                            TileCount = EDir;
                            return;
                        }
                        Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                        int i = TileCount;
                        if (LoR.y > 0.0f)
                        {
                            i = (i + 3) % 4;
                        }
                        else
                        {
                            i = (i + 1) % 4;
                        }

                        if (CurrentTile.AdjCoverList[i] == Tile.Cover.None)
                        {
                            Idle = NoCover;
                            PreAttakeIdle = PreAtkNoCover;
                            Am.SetBool("HCover", false);
                            TileCount = EDir;
                        }
                        else if (CurrentTile.AdjCoverList[i] == Tile.Cover.HalfC)
                        {
                            TileCount = i;
                        }
                        else
                        {
                            LoR = Vector3.Cross(Direction(i), Ediv);
                            if (LoR.y >= 0)
                            {
                                Am.SetBool("Right", true);
                            }
                            else
                            {
                                Am.SetBool("Left", true);
                            }
                            Idle = FullCover;
                            PreAttakeIdle = PreAtkFullCover;
                            Am.SetBool("FCover", true);
                            TileCount = i;
                            Am.SetBool("HCover", false);

                        }
                    }
                    else if (CurrentTile.AdjCoverList[TileCount] == Tile.Cover.HalfC)
                    {
                        TileCount = EDir;
                        return;
                    }
                    else
                    {
                        Am.SetBool("HCover", false);
                        Am.SetBool("FCover", true);
                        TileCount = EDir;
                        //fcover
                    }
                }
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                if (LoR.y >= 0.0f)
                {
                    Am.SetBool("Right", true);
                    Am.SetBool("Turn", true);
                    AmTurn = true;
                }
                else
                {
                    Am.SetBool("Left", true);
                    Am.SetBool("Turn", true);
                    AmTurn = true;
                }

            }
        }
    }

    protected void FullCover()
    {
        if (stateinfo.IsName("LeftCover")|| stateinfo.IsName("RightCover")|| stateinfo.IsName("Idle"))
        {
            transform.forward = Direction(TileCount);
            if (enemy == null)
            {
                return;
            }
            Ediv = (enemy.position - transform.position).normalized;
            Vector3 CDir = Direction(TileCount);
            float CFoB = Vector3.Dot(CDir, Ediv);
            if (CFoB > 0.1)
            {
                Vector3 CLoR = Vector3.Cross(CDir, Ediv);
                if (CLoR.y > 0)
                {
                    Am.SetBool("Right", true);
                    Am.SetBool("Left", false);
                }
                else
                {
                    Am.SetBool("Right", false);
                    Am.SetBool("Left", true);
                }
            }
            else
            {
                //TileCount = FindDirection(Ediv);
                Am.SetBool("Left", false);
                Am.SetBool("Right", false);
                Am.SetBool("HCover", false);
                Am.SetBool("FCover", false);
                Idle = NoCover;
                PreAttakeIdle = PreAtkNoCover;
            }
        }
    }


    //轉向動畫控制


    public virtual void LeftTurn()
    {
        AmTurn = false;
        Am.SetBool("Left", false);
        Am.SetBool("Turn", false);
    }
    public virtual void RightTurn()
    {
        AmTurn = false;
        Am.SetBool("Right", false);
        Am.SetBool("Turn", false);
    }
    public virtual void BackTurn()
    {
        AmTurn = false;
        Am.SetBool("Back", false);
        Am.SetBool("Turn", false);
    }

    IEnumerator LeftTurn2()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, -90f, 0);
            Am.SetBool("Left", false);
            Am.SetBool("Turn", false);
            AmTurn = false;

        }
        else
            yield return LeftTurn2();
    }
    IEnumerator RightTurn2()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, 90f, 0);
            Am.SetBool("Right", false);
            Am.SetBool("Turn", false);
            AmTurn = false;
        }
        else
            yield return RightTurn2();
    }
    IEnumerator BackTurn2()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, 180f, 0);
            Am.SetBool("Back", false);
            Am.SetBool("Turn", false);
            AmTurn = false;

        }
        else
            yield return BackTurn2();
    }


    public Action AIState;
    protected void NpcAI()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);

        if (!Turn)
        {
            Idle();
        }
        else if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            if (Attack)
            {
                Melee();
            }
            else
            {
                Move2();
            }
            
        }
        else if (NPCPrepera)
        {
            DoActing?.Invoke();
        }
    }
    protected void PlayerAI()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (Moving)
        {
            if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
            {
                Move();
            }
            else if (stateinfo.IsName("RunToMelee") || stateinfo.IsName("RunToStop"))
            {
                Melee();
            }
        }
        else if (Target == null && !Am.GetBool("Run"))
        {
            Idle();
        }
    }




    public void RunToAttack()
    {
        transform.position = AttackPosition;
        Am.SetBool("Run", false);
        Am.SetBool("Aim", true);

    }
    public void ForwardToTileCount()
    {
        transform.forward = Direction(TileCount);
    }




    public void ResetBool()
    {
        Moving = false;
        Attack = false;
        Idle = NoCover;
        PreAttakeIdle = PreAtkNoCover;
        AmTurn = false;
        if (ActionName != null)
        {
            Am.SetBool(ActionName, false);
            ActionName = null;
        }
        Am.SetBool("Turn", false);
        Am.SetBool("Back", false);
        Am.SetBool("FCover", false);
        Am.SetBool("HCover", false);
        Am.SetBool("Left", false);
        Am.SetBool("Right", false);
        Am.SetBool("Aim", false);
        Am.SetBool("Run", false);
        Target = null;
    }


    public void Skip()//跳過回合
    {
        Am.Play("Idle");
        AP = 0;

        ResetBool();
        RemoveVisitedTiles();
        NPCPrepera = false;
        DoActing = null;
        EndTurn();
    }





    //移動

    internal List<Tile> VisitedTiles = new List<Tile>();
    internal Stack<(Tile, MoveWay)> Path = new Stack<(Tile, MoveWay)>();      //tile的資料設定為Stack(後進先出)
    public Tile CurrentTile;      //玩家目前站的tile
    internal bool Turn = false;       //回合判斷
    internal bool Moving = false;
    internal bool Running = false;
    public float MoveSpeed = 4;
    protected float ChaHeight;      //角色高度
    internal bool End = false;
    internal Vector3 Heading;                //移動方向
    internal UISystem UI;
    internal bool Prepera = false;
    public enum MoveWay    //移動方式(簡化
    {
        Run=0,
        Ladder,
        Jump,       
    }

    public void MoveRange() //開始計算移動範圍
    {
        MeleeableList.Clear();
        if (AP == 2)
        {
            Queue<Tile> Process = new Queue<Tile>();
            Process.Enqueue(CurrentTile);
            AddVisited(CurrentTile);
            Process = FindSelectableTiles(Process,1); //第一個AP 的範圍
            UI.DrawMRLine(Process,UI.Blue,1.0f);          //畫線
            Process = FindSelectableTiles(Process,2);   //return的是之後畫線要的格子，格子畫好後又會塞給FindSelectableTiles
            UI.DrawMRLine(Process, UI.Yellow,2.0f);

        }
        else if (AP == 1)
        {
            Queue<Tile> Process = new Queue<Tile>();
            Process.Enqueue(CurrentTile);
            AddVisited(CurrentTile);
            Process = FindSelectableTiles(Process, 1);
            UI.DrawMRLine(Process, UI.Yellow,1.0f);
            Process.Clear();
        }
            CurrentTile.selectable = false;
            ArrangeMeleeList();

    }
    public Queue<Tile> FindSelectableTiles(Queue<Tile> Process, int i)
        { 
        //BFS 寬度優先使用Queue
        Queue<Tile> Process2 = new Queue<Tile>();
        while (Process.Count > 0)
        {
            Tile T = Process.Dequeue();     
            if (T.walkable || T==CurrentTile)       //因為畫線跟找路徑的判斷很麻煩 所以有點混亂
            {
                T.selectable = true;
                foreach (Tile adjT in T.AdjList)
                {
                    if (!adjT.visited)
                    {
                        Vector3 vdiv = adjT.transform.position - T.transform.position;
                        vdiv.y = 0;
                        float TDis = vdiv.magnitude;
                        if (adjT.Cha != null && EnemyLayer != adjT.Cha.EnemyLayer)
                        {
                            MeleeableList.AddLast((adjT.Cha, T));
                        }
                        if (TDis > 0.9f)        //如果斜向移動，則判斷路徑上有沒有東西卡到
                        {
                            if (Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, LayerMask.GetMask("Environment"))
                            || Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, EnemyLayer))
                            {
                                continue;
                            }
                        }
                        adjT.distance = TDis + T.distance;
                        if (!adjT.walkable)
                        {
                            Process2.Enqueue(adjT);
                            AddVisited(adjT);
                            continue;
                        }
                        if (adjT.distance > Cha.Mobility * i) //移動距離不會超過上限
                        {
                            adjT.Parent = T;
                            AddVisited(adjT);
                            Process2.Enqueue(adjT);
                            continue;
                        }
                        adjT.Parent = T;  //visited過的就被設為 parent
                        AddVisited(adjT);
                        Process.Enqueue(adjT);
                    }
                    else if (adjT.Cha != null && EnemyLayer != adjT.Cha.EnemyLayer)
                    {
                        MeleeableList.AddLast((adjT.Cha, T));
                    }
                    
                }
            }
            else
                Process2.Enqueue(T);
        }
        return Process2;
    }


    public void PrepareMove(Tile T)
    {
        if (T.distance > Cha.Mobility)
            AP = 0;
        else
            --AP;
        ResetBool();
        Am.SetBool("Run", true);
        Moving = true;
        RemoveVisitedTiles();//重置Tile狀態
        Am.Play("Run");
        AttakeableList.Clear();
        FindObjectOfType<SoundManager>().Play(Cha.affirmative);
    }

    public Stack<(Tile, MoveWay)> MoveToTile(Tile T)//從T的Parent往回推路徑
    {
        Path.Clear();
        // T.TargetChange();//todo 要更換
        Stack<(Tile, MoveWay)> DLpath = new Stack<(Tile, MoveWay)>();
        Tile Previous = T;
        Tile Current = T;
        while (Current.Parent != null)
        {
            MoveWay MW = MoveWay.Run;
            Vector3 Pdiv;
            Vector3 vdiv = Current.Parent.transform.position - Current.transform.position;
            float Height = vdiv.y;
            if (Mathf.Abs(Height) <= 0.6f)
            {
                Path.Push((Current, MW));
                DLpath.Push((Current, MW));
                Current = Current.Parent;
                while (Current.Parent != null)//如果移動方式是Run則優化路徑
                {       
                    vdiv = Current.Parent.transform.position - Current.transform.position;
                    Height = vdiv.y;
                    if (Mathf.Abs(Height) > 0.6f )
                    {
                        break;
                    }
                    Pdiv = Current.Parent.transform.position - Previous.transform.position;
                    if (Physics.BoxCast(Previous.transform.position + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Pdiv
                            , Quaternion.identity, Pdiv.magnitude))//判斷之前位置跟之後的位置之間有沒有障礙，如果沒有則跳過中間的格子
                    {
                        Path.Push((Current, MW));
                        DLpath.Push((Current, MW));
                        Previous = Current;
                    }
                    Current = Current.Parent;
                }
            } 
            else if (Mathf.Abs(Height) > 2.0f)
            {
                if (Height > 0)
                    MW = MoveWay.Jump;
                else
                    MW = MoveWay.Ladder;
            }
            Path.Push((Current,MW));
            DLpath.Push((Current, MW));
        }
        return DLpath;
       // Moving = true;//
    }

    public void Move()
    {
        if (Moving != true)
        {
            return;
        }
        //if path.Count > 0, move, or remove selectable tiles, disable moving and end the turn. 

        if (Path.Count > 0)
        {
            (Tile T,MoveWay M) = Path.Peek(); 
            Vector3 target = T.transform.position;
            target.y += ChaHeight;
            //switch (M)  用移動方式來決定程式跑法
            //{
            //    case MoveWay.Run:
            //        Run(target);
            //        break;
            //    case MoveWay.Across:
            //        Across();
            //        break;
            //    case MoveWay.Jump:
            //        Jump();
            //        break;
            //    case MoveWay.ClimbDown:
            //        ClimbDown();
            //        break;
            //    case MoveWay.Ladder:
            //        Ladder();
            //        break;
            //    case MoveWay.ClimbUp:
            //        Climbup();
            //        break;
            //}

            if ((transform.position - target).magnitude >= 0.1f)
            {
                Heading = target - transform.position;
                Heading.Normalize();
                //Locomotion
                transform.forward = Heading;
                transform.position += Heading * MoveSpeed * Time.deltaTime;
            }
            else
            {
                //Tile center reached
                if (Path.Count == 1)
                {
                    Am.SetBool("Run", false);
                    Am.Play("Stop");
                    OutCurrentTile();
                    InCurrentTile(Path.Peek().Item1);
                }
                transform.position = target;
                Path.Pop();
            }
        }
        else
        {
            TileCount = FindDirection(transform.forward);
            transform.forward = Direction(TileCount);

            Moving = false;
            StartCoroutine(WaitNextAction());
        }
    }

    internal Vector3 Direction(int tilecount)
    {
        switch (tilecount)
        {
            case -1:
                return  new Vector3();
            case 0:
                return new Vector3(0, 0, 1);
            case 1:
                return new Vector3(-1, 0, 0);
            case 2:
                return new Vector3(0, 0, -1);
            case 3:
                return new Vector3(1, 0, 0);
            default:
                Debug.LogError("No direction");
                return default(Vector3);
        }
    }
    public int FindDirection(Vector3 div)
    {
        if (Mathf.Abs(div.x) > Mathf.Abs(div.z)+0.001f)
        {
            if (div.x > 0)
            {
                return 3;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (div.z > 0)
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }
    }


    public void AddVisited(Tile T)
    {
        VisitedTiles.Add(T);
        T.visited = true;
    }
    public void RemoveVisitedTiles()
    {
        if (VisitedTiles.Count == 0)
        {
            return;
        }
        foreach (Tile tile in VisitedTiles)
        {
            tile.Reset();
        }
        VisitedTiles.Clear();
    }
    public void InCurrentTile(Tile T)
    {
        CurrentTile = T;
        T.walkable = false;
        T.Cha = this;
    }
    public void OutCurrentTile()
    {
        CurrentTile.walkable = true;
        CurrentTile.Cha = null;
    }


    //攻擊
    protected List<AI> Enemies;
    internal LinkedList<(AI, int ,int )> AttakeableList = new LinkedList<(AI, int, int)>();//角色,射擊位置,命中率;
    internal Weapon Gun;
    internal bool PreAttack = false;
    internal bool Attack;
    internal bool ChangeTarget = false;
    internal AI Target;
    protected Vector3 TargetDir;
    public Transform FirePoint;
    public GameObject Bullet;
    public Transform BeAttakePoint;
    internal Vector3 AttackPoint;
    internal bool Miss = false;
    internal Vector3 AttackPosition;
    protected List<AI> AttPredict = new List<AI>();
    internal bool BeAimed = false;
    internal LinkedList<(AI, Tile)> MeleeableList = new LinkedList<(AI, Tile)>();
    internal LinkedList<AI> HealList = new LinkedList<AI>();
    internal LinkedList<AI> ComaList = new LinkedList<AI>();
    public GameObject Rifle;
    public GameObject Knife;
    internal bool Coma = false;
    public void GetTargets(List<AI> enemy)
    {
        Enemies = enemy;
    } 

    public void AttakeableDetect()
    {
        if (AttakeableList.Count>0 || AP == 0)
        {
            return;
        }
        foreach (AI Enemy in Enemies)//改queue ?
        {
            Vector3 Location = CurrentTile.transform.position;
            Vector3 Target = Enemy.CurrentTile.transform.position;
            Vector3 RTDiv = Location - Target;
            int i = FindDirection(RTDiv);
            if (Enemy.CurrentTile.AdjCoverList[i] == Tile.Cover.FullC)//判斷敵人有無FullCover，如有則瞄準障礙物邊邊。
            {
                int j = 0;
                if (Vector3.Cross(Direction(i), RTDiv).y >= 0)
                {
                    j = (i + 3) % 4;
                }
                else
                {
                    j = (i + 1) % 4;
                }
                Target += (Direction(i) + Direction(j)) * 0.3f;
            }
            Vector3 TDiv = Target - Location;
            i = FindDirection(TDiv);
            if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), Target - Location, (Target - Location).magnitude, 1 << 9 ))
            {//確保路徑上沒有障礙物
                AttakeableList.AddLast((Enemy, -1, CalculateAim(Enemy, CurrentTile.transform.position)));
            }
            else if (CurrentTile.AdjCoverList[i] == Tile.Cover.FullC) //如攻擊方有障礙物，則站出去瞄準。
            {
                float LoR = Vector3.Cross(Direction(i), TDiv).y;
                if (LoR != 0)
                {
                    if (LoR > 0)
                    {
                        i = (i + 3) % 4;
                    }
                    else
                    {
                        i = (i + 1) % 4;
                    }
                    Location += Direction(i) * 0.67f;
                    if (CurrentTile.AdjList[i].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9 ))
                        {//確保路徑上沒有障礙物
                            AttakeableList.AddLast((Enemy, i, CalculateAim(Enemy, Location)));//todo
                            continue;
                        }
                    }
                }
                else
                {
                    if (CurrentTile.AdjList[(i + 1) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9 ))
                        {//確保路徑上沒有障礙物
                            AttakeableList.AddLast((Enemy, (i + 1) % 4, CalculateAim(Enemy, Location)));//todo
                            continue;
                        }
                    }
                    if (CurrentTile.AdjList[(i + 3) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            AttakeableList.AddLast((Enemy, (i + 3) % 4, CalculateAim(Enemy, Location)));//todo
                            continue;
                        }
                    }
                }
            }
        }
        for(int i =  0; i < AttakeableList.Count; ++i)//目標順序改成 命中率由大到小
        {
            var Current = AttakeableList.First;
            var Previous = Current;
            Current = Current.Next;
            for (int j =0;j< AttakeableList.Count -1- i; ++j)
            {
                if (Previous.Value.Item3 < Current.Value.Item3)
                {
                    AttakeableList.Remove(Previous);
                    AttakeableList.AddAfter(Current, Previous);
                    Current = Previous.Next;
                }
                else
                {
                    Previous = Current;
                    Current = Current.Next;
                }
            }
        }
    }

    public List<AI> AttackablePredict(Tile T)
    {
        if (AttPredict.Count > 0 )
        {
            return AttPredict;
        }
        foreach (AI Enemy in Enemies)//改queue ?
        {
            Vector3 Location = T.transform.position;
            Vector3 Target = Enemy.CurrentTile.transform.position;
            Vector3 RTDiv = Location - Target;
            int i = FindDirection(RTDiv);
            if (Enemy.CurrentTile.AdjCoverList[i] == Tile.Cover.FullC)//判斷敵人有無FullCover，如有則瞄準障礙物邊邊。
            {
                int j = 0;
                if (Vector3.Cross(Direction(i), RTDiv).y >= 0)
                {
                    j = (i + 3) % 4;
                }
                else
                {
                    j = (i + 1) % 4;
                }
                Target += (Direction(i) + Direction(j)) * 0.3f;
            }
            Vector3 TDiv = Target - Location;
            i = FindDirection(TDiv);
            if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), Target - Location, (Target - Location).magnitude, 1 << 9 ))
            {//確保路徑上沒有障礙物
                AttPredict.Add(Enemy);
            }
            else if (T.AdjCoverList[i] == Tile.Cover.FullC) //如攻擊方有障礙物，則站出去瞄準。
            {
                float LoR = Vector3.Cross(Direction(i), TDiv).y;
                if (LoR != 0)
                {
                    if (LoR > 0)
                    {
                        i = (i + 3) % 4;
                    }
                    else
                    {
                        i = (i + 1) % 4;
                    }
                    Location += Direction(i) * 0.67f;
                    if (T.AdjList[i].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9 ))
                        {//確保路徑上沒有障礙物
                            AttPredict.Add(Enemy);
                            continue;
                        }
                    }
                }
                else
                {
                    if (T.AdjList[(i + 1) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            AttPredict.Add(Enemy);
                            continue;
                        }
                    }
                    if (T.AdjList[(i + 3) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9 ))
                        {//確保路徑上沒有障礙物
                            AttPredict.Add(Enemy);
                            continue;
                        }
                    }
                }
            }
        }
        return AttPredict;

    }

    int CalculateAim(AI Enemy,Vector3 Location)
    {
        Vector3 div = Location - Enemy.CurrentTile.transform.position; 
        div.y = 0;
        Tile.Cover[] cover;
        float dis = div.magnitude;
        float AimAngle;
        if (Enemy.Cha.type == Character.Type.Humanoid) //人形怪才有障礙物Buff
        {
            cover = Enemy.CurrentTile.JudgeCover(div, out AimAngle);
            if (cover[0] == Tile.Cover.FullC)
            {
                return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 40;
            }
            else if (cover[0] == Tile.Cover.HalfC)
            {
                if (cover[1] == Tile.Cover.FullC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 40 + (int)(20f * (45f-AimAngle) / 45f);
                }
                else if(cover[1] == Tile.Cover.HalfC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 20;
                }
                else
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 20 + (int)(10 * (45f - AimAngle)/ 45f);
                }
            }
            else
            {
                if(cover[1] == Tile.Cover.FullC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 40 + (int)(40f * (45f - AimAngle )/ 45f);
                }
                else if(cover[1] == Tile.Cover.HalfC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 20 + (int)(20f * (45f - AimAngle) / 45f);
                }
                else
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)];
                }
            }

        }
        else
        {
            return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)];
        }
        //計算方向判斷對方是有遮蔽物
    //距離影響武器命中率
    }

    protected IEnumerator AIWaitPreAtkChange()
    {
        PreAttack = false;
        yield return new WaitForSecondsRealtime(0.5f);
        NPCPrepera = true;
        PreAttack = true;
    }
    public void ChangePreAttackIdle()
    {
        if (Am.GetBool("FCover"))
        {
            PreAttakeIdle = PreAtkFullCover;
        }
        else if (Am.GetBool("HCover"))
        {
            PreAttakeIdle = PreAtkHalfCover;
        }
        else
        {
            PreAttakeIdle = PreAtkNoCover;
        }
    }
    public void ChangePreAttakeIdle(Vector3 dir)
    {
        Tile.Cover cover = CurrentTile.AdjCoverList[FindDirection(dir)];
        if (cover == Tile.Cover.FullC)
        {
            if (Am.GetBool("FCover") == true)
            {
                TileCount = FindDirection(dir);
                PreAttakeIdle = PreAtkFullCover;
                StartCoroutine(AIWaitPreAtkChange());
                return;
            }
            TileCount = FindDirection(dir);
            if (Vector3.Cross(Direction(TileCount), dir).y >= 0)
            {
                Am.SetBool("Right", true);
            }
            else
            {
                Am.SetBool("Left", true);
            }
            Am.SetBool("FCover", true);

            Am.SetBool("HCover", false);


            PreAttakeIdle = PreAtkFullCover;
            ChangeTarget = true;
            Idle = FullCover;

        }
        else if (cover==Tile.Cover.HalfC)
        {
            if (Am.GetBool("HCover") == true)
            {
                TileCount = FindDirection(dir);
                PreAttakeIdle = PreAtkHalfCover;
                NPCPrepera = true;
                //StartCoroutine(AIWaitPreAtkChange());//todo
                return;
            }
            Am.SetBool("FCover", false);
            Am.SetBool("Left", false);
            Am.SetBool("HCover", true);
            Am.SetBool("Right", false);
            TileCount = FindDirection(dir);
            PreAttakeIdle = PreAtkHalfCover;
            ChangeTarget = true;
            Idle = HalfCover;
            float FoB = Vector3.Dot(transform.forward, TargetDir.normalized);
            Vector3 LoR = Vector3.Cross(transform.forward, TargetDir);
            if (!(FoB > 1 / Mathf.Sqrt(2) - 0.01f))

            {
                if (LoR.y > 0)
                {
                    Am.SetBool("Right", true);
                }
                else
                {
                    Am.SetBool("Left", true);
                }
                Am.SetBool("Turn", true);
            }

        }
        else
        {
            if(!Am.GetBool("HCover")&& !Am.GetBool("FCover")&& !ChangeTarget)
            {
                PreAttakeIdle = PreAtkNoCover;
                Am.SetBool("Aim", true);
                NPCPrepera = true;
                //StartCoroutine(AIWaitPreAtkChange());
                return;
            }
            Am.SetBool("FCover", false);
            Am.SetBool("Left", false);
            Am.SetBool("HCover", false);
            Am.SetBool("Right", false);
            PreAttakeIdle = PreAtkNoCover;
            ChangeTarget = true;
            Idle = NoCover;
            AmTurn = false;

        }
    }



    public void ChaChangeTarget(AI target)//切換攻擊目標
    {
        if (Target == target)
        {
            return;
        }
        PreAttack = true;
        Target = target;

        TargetDir = Target.transform.position - transform.position;
        TargetDir.y = 0;
        if (Am.GetBool("FCover"))
        {
            Idle = FullCover;
            Vector3 LoR = Vector3.Cross(Direction(TileCount), TargetDir);
            if (LoR.y >= 0)
            {
                Am.SetBool("Right", true);
                Am.SetBool("Left", false);
            }
            else
            {
                Am.SetBool("Right", false);
                Am.SetBool("Left", true);
            }
        }
        else if (Am.GetBool("HCover"))
        {
            Idle = HalfCover;
            float FoB = Vector3.Dot(transform.forward, TargetDir.normalized);
            if (FoB > 1 / Mathf.Sqrt(2) - 0.001f)
            {
                return;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, TargetDir);
                if (LoR.y >= 0)
                {
                    Am.SetBool("Right", true);
                    Am.SetBool("Turn", true);
                }
                else
                {
                    Am.SetBool("Left", true);
                    Am.SetBool("Turn", true);
                }
                ChangeTarget = true;
            }
        }
        else
        {
            Idle = NoCover;
            ChangeTarget = true;
            Am.SetBool("Aim", false);
            Vector3 LoR = Vector3.Cross(transform.forward, TargetDir);
            if (LoR.y >= 0)
            {
                Am.SetBool("Right", true);
                Am.SetBool("Turn", true);
            }
            else
            {
                Am.SetBool("Left", true);
                Am.SetBool("Turn", true);
            }
        }
    }

    protected void FaceTarget()//切換攻擊的過程(朝向目標方向)
    {
        TargetDir.y = 0;
        if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.98f)
        {
            ChangeTarget = false;
            Am.SetBool("Right", false);
            Am.SetBool("Left", false);
            Am.SetBool("Turn", false);
            AmTurn = false;
            transform.forward = TargetDir;
        }
        else if (!AmTurn)
        {
            float LoR = Vector3.Cross(transform.forward, TargetDir).y;
            if (LoR > 0)
            {
                Am.SetBool("Right", true);
                Am.SetBool("Turn", true);
            }
            else
            {
                Am.SetBool("Left", true);
                Am.SetBool("Turn", true);
            }
        }

         if ( (stateinfo.IsName("RightTurn") || stateinfo.IsName("LeftTurn"))&& Am.GetBool("Turn"))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.03f);
        }

    }

    public Action PreAttakeIdle;

    public virtual void PreAtkNoCover()
    {
        if (Attack)
        {
            TargetDir = AttackPoint - CurrentTile.transform.position;
            TargetDir.y = 0;
            if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.98f)
            {
                transform.forward = TargetDir;
                AP = 0;
                UI.LRDestory();
                Am.SetBool(ActionName, true);//action name
                RemoveVisitedTiles();
                StartCoroutine(FireWait());

            }else
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
        }
        else if (ChangeTarget)
        {
            FaceTarget();
        }
        else
        {
            if(Vector3.Dot(transform.forward, TargetDir.normalized) > 0.97f)
            {
                ChangePreAttakeIdle(TargetDir);
            }
            //else 
            //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
        }
    }
    public void PreAtkHalfCover()
    {
        if (Attack)
        {
            TargetDir = AttackPoint - CurrentTile.transform.position;
            TargetDir.y = 0;
            if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.96f)
            {
                Am.SetBool("Aim", true);
                transform.forward = TargetDir;
                if (stateinfo.normalizedTime > 0.5f && stateinfo.IsName("Aim"))
                {
                    AP = 0;
                    UI.LRDestory();
                    Am.SetBool(ActionName, true);
                    RemoveVisitedTiles();
                    PreAttack = false;
                    StartCoroutine(FireWait());
                }
            }
            else 
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.08f);
        }
        else if (ChangeTarget)
        {
            stateinfo = Am.GetCurrentAnimatorStateInfo(0);

            float FoB = Vector3.Dot(transform.forward, TargetDir.normalized);
            Vector3 LoR = Vector3.Cross(transform.forward, TargetDir);
            if (FoB > 1 / Mathf.Sqrt(2) - 0.01f&& stateinfo.normalizedTime >= 0.4f)
            {

                ChangeTarget = false;
                ChangePreAttakeIdle(TargetDir);
            }
            else if(stateinfo.normalizedTime>=1.0f)
            {
                Am.SetBool("Turn", false);
                if (LoR.y > 0)
                {
                    Am.SetBool("Right", true);
                }
                else
                {
                    Am.SetBool("Left", true);
                }
                Am.SetBool("Turn", true);

            }

        }
    }
    public void PreAtkFullCover()
    {
        if (Attack&& !ChangeTarget)
        {
            if (stateinfo.IsName("Aim"))
            {
                TargetDir = AttackPoint - AttackPosition;
                TargetDir.y = 0;
                //if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.98f)
                //{
                    transform.forward = TargetDir;
                    AP = 0;
                    UI.LRDestory();
                    Am.SetBool(ActionName, true);
                    RemoveVisitedTiles();
                    StartCoroutine(FW());
                //}
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
            }

            else if(!Am.GetBool("Aim")&& (stateinfo.IsName("LeftToAttack")|| stateinfo.IsName("RightToAttack")))
            {
                if ((transform.position - AttackPosition).magnitude < 0.1f)
                {
                    transform.position = AttackPosition;
                    //Am.SetBool("Turn", true);
                    Am.SetBool("Run", false);
                    Am.SetBool("Aim", true);
                    transform.forward = TargetDir;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, AttackPosition, 0.05f);
                    //transform.position += transform.forward * MoveSpeed * Time.deltaTime*0.1f;
                }
            }
        }
        else if (ChangeTarget)
        {
            Vector3 LoR = Vector3.Cross(Direction(TileCount), TargetDir.normalized);
            if (LoR.y >= 0)
            {
                transform.forward = Direction(TileCount);
                Am.SetBool("Right", true);
                Am.SetBool("Left", false);
                ChangeTarget = false;
                ChangePreAttakeIdle(TargetDir);
            }
            else
            {
                transform.forward = Direction(TileCount);
                Am.SetBool("Right", false);
                Am.SetBool("Left", true);
                ChangeTarget = false;
                ChangePreAttakeIdle(TargetDir);
            }
        }
    }
    public void PreAtkFCoverAfterAttack()
    {
        if ((transform.position - AttackPosition).magnitude < 0.05f)
        {
            transform.position = CurrentTile.transform.position + Vector3.up * ChaHeight;
            Am.SetBool("Back", false);
            transform.forward = Direction(TileCount);
            PreAttack = false;
        }
        else if (stateinfo.IsName("LeftToCover") || stateinfo.IsName("RightToCover"))
        {
            transform.position = Vector3.Lerp(transform.position, AttackPosition, 0.05f);
            //transform.position += -transform.forward * MoveSpeed * Time.deltaTime*0.2f;
        }
    }





    public void Fire((AI target, int location, int aim) FireTarget)
    {
        Gun.bullet -= 1;
        Debug.Log(FireTarget.aim);
        int i =  Random.Range(0, 101);
        if (FireTarget.aim < i)//Miss
        {
            Miss = true;
            RaycastHit RH;
            Vector3 ShotPoint = CurrentTile.transform.position + new Vector3(0, 1.34f, 0) + Direction(FireTarget.location);
            int j = 0;
            Vector3 RandPoint;
            while (true)
            {
                if (j > 10)
                {
                    AttackPoint = FireTarget.Item1.BeAttakePoint.position +Vector3.up*0.3f+ Vector3.Cross((FireTarget.Item1.BeAttakePoint.position - ShotPoint), Vector3.up).normalized * 0.3f; ;
                    break;
                }
                RandPoint = FireTarget.Item1.BeAttakePoint.position
                            + new Vector3(Random.Range(-0.67f, 0.67f), Random.Range(-0.2f, 0.2f), Random.Range(-0.67f, 0.67f));
                if (Physics.SphereCast(ShotPoint, 0.05f, RandPoint - ShotPoint, out RH, 30f))
                {
                    if (RH.collider.tag != "Human" && RH.collider.tag != "Alien")
                    {
                        AttackPoint = RandPoint;
                        break;
                    }
                }
                else
                {
                    AttackPoint = RandPoint;
                    break;
                }
                ++j;
            }
        }
        else
        {
            Miss = false;
            AttackPoint = Target.BeAttakePoint.position;
            Target.BeDamaged(Gun.Damage[Random.Range(0, Gun.DamageRange)]);
        }
        if (Am.GetBool("FCover"))
        {
            if(FireTarget.location == -1)
            {
                Am.SetBool("Aim", true);
                AttackPosition = CurrentTile.transform.position;
                FW = FullCoverFireWait2;
            }
            else
            {
                Am.SetBool("Run", true);
                Vector3 dir = Direction(FireTarget.location);
                AttackPosition = transform.position + dir*0.67f;
                FW = FullCoverFireWait;
            }
        }
        else if (Am.GetBool("HCover"))
        {
            Am.SetBool("Aim", true);
        }
        else
        {
            ;
        }
        Attack = true;
        ActionName = "Fire";
    }

    public ParticleSystem FireLight;
    public void FireBullet()
    {
        GameObject B = Instantiate(Bullet,FirePoint.transform.position,Quaternion.identity);
        if (Miss)
        {
            TargetDir = AttackPoint - FirePoint.position;
            B.GetComponent<bullet>().SetAttackPoint(FirePoint.position, AttackPoint);
        }
        else
        {
            TargetDir = Target.BeAttakePoint.position - FirePoint.position;
            B.GetComponent<bullet>().SetAttackPoint(FirePoint.position, Target.BeAttakePoint.position);
            B.GetComponent<bullet>().Hit = true;
        }
        B.transform.forward = TargetDir;
        if (FireLight != null)
        {
            FireLight.Play();
        }
        Attack = false;
        FindObjectOfType<SoundManager>().Play(Gun.weapon1);
    }

    public delegate IEnumerator FWait();
    public FWait FW;
    public IEnumerator FireWait()//攻擊後緩衝時間給下回合
    {
        PreAttack = false;
        yield return new WaitUntil(() => Attack == false);
        yield return new WaitForSeconds(1f);
        EndTurn();
        ResetBool();
    }
    public IEnumerator FullCoverFireWait()
    {
        //yield return new WaitUntil(() => stateinfo.normalizedTime >= 1.0f);
        PreAttakeIdle = PreAtkFCoverAfterAttack;
        Am.SetBool("Back", true);
        //yield return new WaitUntil(() => stateinfo.IsName("LeftToCover") || stateinfo.IsName("RightToCover"));

        Vector3 dir = AttackPosition - CurrentTile.transform.position;
        dir.y = 0;
        //transform.forward = Direction(TileCount);
        AttackPosition = CurrentTile.transform.position;
        yield return new WaitUntil(() => PreAttack == false);
        transform.forward = Direction(TileCount);
        Am.SetBool(ActionName, false);
        Am.SetBool("Aim", false);
        ActionName = null;
        EndTurn();

    }
    public IEnumerator FullCoverFireWait2()
    {
        PreAttack = false;
        yield return new WaitUntil(() => stateinfo.IsName(ActionName));
        Am.SetBool(ActionName, false);
        ActionName = null;

        yield return new WaitForSeconds(1f);
        Am.SetBool("Aim", false);
        EndTurn();
        PreAttack = false;
        //ResetBool();
    }



    protected void ArrangeMeleeList()
    {
        if (MeleeableList.Count == 0) { return; }
        AI First, Previous = null;

        while (true)
        {
            var Current = MeleeableList.First;
            First = Current.Value.Item1;
            if (Previous == First)
            {
                break;
            }
                var tmp = Current.Next;
            while (tmp != null)
            {

                
                if (Previous!=null&&tmp.Value.Item1 == Previous)
                {
                    break;
                }
                if (tmp.Value.Item1 != First)
                {
                    MeleeableList.Remove(tmp);
                    MeleeableList.AddFirst(tmp);
                }
                else
                {
                    Current = tmp;
                }
                tmp = Current.Next;
            }
            Previous = First;
        }
    }
    public virtual void PreMelee(LinkedListNode<(AI, Tile)> MeleeTarget)
    {
        MoveToTile(MeleeTarget.Value.Item2);
        ResetBool();
        if (Path.Count != 0)
        {
            Am.SetBool("Run", true);
        }
        Idle = NoCover;
        PreAttakeIdle = PreAtkNoCover;
        Moving = true;
        RemoveVisitedTiles();//重置Tile狀態
        Attack = true;
        Target = MeleeTarget.Value.Item1;
        Am.SetBool("Melee",true);        
        UI.LRDestory();        
    }


    public void PutRifle()
    {
        Rifle.SetActive(false);
    }
    public void GrabRifle()
    {
        Rifle.SetActive(true);
    }
    public void PutKnife()
    {
        BeAttakePoint.Find("BackKnife").gameObject.SetActive(true);
        Knife.SetActive(false);
    }
    public void GrabKnife()
    {
        FindObjectOfType<SoundManager>().Play(Gun.preAttack);
        BeAttakePoint.Find("BackKnife").gameObject.SetActive(false);
        Knife.SetActive(true);
    }

    public virtual void Melee()
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

            if ((transform.position - target).magnitude >= 0.1f)
            {
                Heading = target - transform.position;
                Heading.Normalize();
                //Locomotion
                transform.forward = Heading;
                transform.position += Heading * 5f * Time.deltaTime;
            }
            else
            {
                //Tile center reached
                if (Path.Count == 1)
                {

                    Am.SetBool("Run", false);
                    Am.Play("RunToStop");
                    OutCurrentTile();
                    InCurrentTile(Path.Peek().Item1);
                }
                transform.position = target;
                Path.Pop();
            }
        }
        else
        {
            Miss = false;
            Moving = false;
            Attack = false;

            StartCoroutine(FaceMeleeTarget());
        }
    }

    public IEnumerator FaceMeleeTarget()
    {
        yield return new WaitUntil(() => stateinfo.IsName("Armed"));
        Vector3 TargetDir = Target.transform.position - transform.position;
        TargetDir.y = 0;
        transform.forward = TargetDir;
    }
    protected void EndMelee()
    {
        Am.SetBool("Melee", false);
        ResetBool();
        EndTurn();
    }
    public void Meleeing()
    {
        Target.BeDamaged(4);
        Target.Hurt(transform.forward);
        FindObjectOfType<SoundManager>().Play(Gun.weapon2);
    }

    public void PreHeal(AI Cha)
    {
        foreach(var Skill in Skills)
        {
            if (Skill.Name == "Heal")
            {
                Skill.EnterCD();
                AP -= Skill.AP;
                break;
            }
        }

        ResetBool();
        if (Cha == this)
        {
            Am.SetTrigger("SelfHeal");
            Target = this;            
        }
        else
        {
            Am.Play("Heal");
            Target = Cha;
            transform.forward = Target.transform.position - transform.position;
        }
        RemoveVisitedTiles();

    }

    public void Heal()
    {
        FindObjectOfType<SoundManager>().Play(Cha.xskill);
        Target.Cha.HP += 3;
        Destroy(Instantiate<GameObject>(Resources.Load<GameObject>("HealEffect"), Target.transform.position, Quaternion.identity), 2.0f);
        if (Target.Cha.HP > Target.Cha.MaxHP)
        {
            Target.Cha.HP = Target.Cha.MaxHP;
        }
        UI.status("heal", this);
        UI.HpControl(Target, Target.Cha.HP);
        ResetBool();
        StartCoroutine(WaitNextAction());
    }




    public void PreCooperation(AI Cha)
    {
        foreach (var Skill in Skills)
        {
            if (Skill.Name == "Cooperation")
            {
                Skill.EnterCD();
                AP -= Skill.AP;
                break;
            }
        }
        ResetBool();
        Am.Play("Point");
        RemoveVisitedTiles();//重置Tile狀態
        Target = Cha;
        transform.forward = Target.transform.position - transform.position;
        FindObjectOfType<SoundManager>().Play(Gun.note);
    }
    public void Cooperation()
    {
        Target.AP = 1;
        UI.TurnCha = Target;
        UI.PlayerStartTurn();
    }

    public virtual void EndTurn()
    {
        Target = null;
        NPC_Prefire = false;
        if (AttakeTarget.Item1 != null)
        {
            AttakeTarget.Item1.BeAimed = false;
            AttakeTarget = (null, 0, 0);
        }
        if (AttakeableList.Count > 0)
        {
            foreach ((AI ai, _, _) in AttakeableList)
            {
                ai.NotBeAim();
            }
        }
        AttakeableList.Clear();

        Turn = false;
        lock (EndCheck.GetInstance())
        {
            EndCheck.GetInstance().ChaEnd = true;
        }
        UI.CheckEvent();
    }

             
    
    public virtual void BeDamaged(int damage)
    {
        Cha.HP -= damage;//todo ?
        UI.demage = damage;
        Debug.Log(damage);
    }
    protected virtual void AIDeath()
    {
        OutCurrentTile();
        RS.DeathKick(this);
        TimeLine.Instance.Moved = false;
        UI.DeathKick(this);
        Destroy(GetComponent<EPOOutline.Outlinable>());
        Destroy(Cha);
        if (MindControlAI != null&&!MindControlAI.Am.GetBool("Death"))
        {
            UI.TurnRun = () => { StartCoroutine(MindControlAI.RecoverMind(this)); UI.TurnRun = null; };
        }
        else
        {
            Destroy(this);
        }

    }

    public virtual void Hurt(Vector3 dir)
    {
        if (Am.GetBool("Death") || stateinfo.IsName("Hurt"))
        {
            return;
        }
        dir.y = 0;
        if (Cha.HP <= 0)
        {
            Am.SetBool("Death", true);
            transform.forward = -dir;
            UI.HpControl(this, Cha.HP);
            Am.Play("Death");
            FindObjectOfType<SoundManager>().Play(Cha.die);
            AIDeath();
        }
        else
        {
            transform.forward = -dir;
            UI.HpControl(this, Cha.HP);
            ResetBool();
            Am.Play("Hurt");
            Idle = NoCover;
            PreAttakeIdle = PreAtkNoCover;
            FindObjectOfType<SoundManager>().Play(Cha.takeHit);
        }
    }
    public void Hurt2(Vector3 dir)
    {
        dir.y = 0;
        if (Cha.HP <= 0)
        {
            transform.forward = -dir;
            UI.HpControl(this, Cha.HP);
            Am.Play("Twitch");
            Coma = true;
            Am.SetBool("Death",true);
            AIDeath();
        }
        else
        {
            transform.forward = -dir;
            UI.HpControl(this, Cha.HP);
            ResetBool();
            Coma = true;
            Am.Play("Twitch");
            Idle = NoCover;
            PreAttakeIdle = PreAtkNoCover;
        }
    }



    public void BeAim(AI Attacker)
    {
        BeAimed = true;
        enemy = Attacker.transform;
    }
    public void NotBeAim()
    {
        BeAimed = false;
    }

    public void Reload()
    {
        Gun.bullet = Gun.MaxBullet;
        AP -= 1;
        Am.SetTrigger("Reload");
        FindObjectOfType<SoundManager>().Play(Cha.reload);
        FindObjectOfType<SoundManager>().Play(Gun.vReload);
        RemoveVisitedTiles();
        UI.LRDestory();
        StartCoroutine( WaitNextAction());
    }
    protected IEnumerator WaitNextAction()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (AP != 0)
        {
            UI.PlayerStartTurn();
        }
        else
        {
            EndTurn();
            PreAttack = false;
        }
    }


    public void PreBomb()
    {
        --AP;
        Am.SetTrigger("Bomb");
        AmTurn = true;
        transform.forward = Vector3.right;
        TileCount = FindDirection(Vector3.right);
        RemoveVisitedTiles();
        UI.LRDestory();
        FindObjectOfType<SoundManager>().Play(Cha.detonate);
    }
    public void ForwardToBomb()
    {
        transform.forward = Vector3.right;
    }
    public void Bomb()
    {
        AmTurn = false;
        UI.Bomb_button();
        StartCoroutine(WaitNextAction());
    }


    public void Leave()
    {
        RemoveVisitedTiles();
        OutCurrentTile();
        UI.Escape = true;
        RS.DeathKick(this);
        TimeLine.Instance.Moved = false;
        UI.DeathKick(this);
        EndTurn();
        Destroy(gameObject);
    }

    public void BeComa()
    {
        Instantiate<GameObject>(Resources.Load<GameObject>("ComaEffect"),transform.position+ new Vector3(0,1.34f,0),Quaternion.identity).transform.SetParent(transform);  
    }

    public void WakeUp()
    {
        Coma = false;
        Am.SetTrigger("Wake");
        Destroy(transform.Find("ComaEffect(Clone)").gameObject);
        ResetBool();
    }


    public IEnumerator WaitEndturn()
    {
        yield return new WaitForSeconds(2f);
        AP = 0;
        EndTurn();//todo UI跳過回合
    }

    public void PreWake(AI Cha)
    {
        ResetBool();
        AmTurn = true;
        Am.SetTrigger("Wake");
        Target = Cha;
        transform.forward = Target.transform.position - transform.position;
        RemoveVisitedTiles(); 
    }
     public void Wake()
    {
        AmTurn = false;
        Target.WakeUp();
        StartCoroutine(WaitNextAction());
    }

    public Grenade Grenade;
    private Tile ExTile;

    public void PreGrenade(Tile T)
    {
        foreach (var Skill in Skills)
        {
            if (Skill.Name == "Grenade")
            {
                Skills.Remove(Skill);
                Skill.EnterCD();
                AP -= Skill.AP;
                break;
            }
        }
        Destroy(GetComponent<ThrowGrenade>());
        Grenade.gameObject.SetActive(true);
        ResetBool();
        ExTile = T;
        AmTurn = true;
        Am.SetTrigger("Grenade");
        RemoveVisitedTiles();
        transform.forward = T.transform.position - transform.position;
    }
    public void ThrowGrenade()
    {
        AmTurn = false;
        Grenade.transform.SetParent(null);
        Grenade.enabled = true;
        Grenade.TargetTile = ExTile;
        Debug.Log("Hello");
    }











    /////AI

    public virtual void FindSelectableTiles(int ap)
    {
        if (VisitedTiles.Count > 0)
            return;
        //BFS 寬度優先使用Queue
        BestPoint = 0;
        Queue<Tile> Process = new Queue<Tile>();
        CalPointAction(CurrentTile);

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
                    AddVisited(adjT);
                    adjT.Parent = T;  //visited過的就被設為 parent
                    if (adjT.distance > Cha.Mobility * ap) //移動距離不會超過上限
                    {
                        continue;
                    }


                    CalPointAction(adjT);
                    Process.Enqueue(adjT);
                }
            }
        }
    }


    protected virtual void PreMove()
    {
        MoveToTile(BestT);
        ResetBool();
        Am.SetBool("Run", true);
        Idle = NoCover;
        PreAttakeIdle = PreAtkNoCover;
        Moving = true;
        RemoveVisitedTiles();//重置Tile狀態
        OutCurrentTile();
        InCurrentTile(BestT);
        DoActing = null;
        Am.Play("Run");
    }

    public virtual void Move2()
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

            if ((transform.position - target).magnitude >= 0.1f)
            {
                Heading = target - transform.position;
                Heading.Normalize();
                transform.forward = Heading;
                transform.position += Heading * MoveSpeed * Time.deltaTime;
            }
            else
            {
                if (Path.Count == 1)
                {
                    Am.SetBool("Run", false);
                    Am.Play("Stop");
                }
                transform.position = target;
                Path.Pop();
            }
        }
        else
        {
            TileCount = FindDirection(transform.forward);
            transform.forward = Direction(TileCount);
            //RemoveVisitedTiles();//重置Tile狀態
            //CurrentTile.Reset();
            //CurrentTile.walkable = true;
            //CurrentTile = TargetTile;
            Moving = false;
            if (Acting2 != null)
            {
                DoActing = Acting2.GetAction();
                ActionName = Acting2.Name;
                Acting2.EnterCD();
                Acting2 = null;
                //StartCoroutine(MoveWait());
            }
            else
            {
                PreAttack = false;
                NPCPrepera = false;
                ResetBool();
                AP = 0;
                RS.EndChecked = true;
                StartCoroutine(WaitNextAction());
            }
        }
    }

    protected string ActionName;
    protected Tile BestT;
    public Action DoActing;
    protected ISkill Acting;
    protected ISkill Acting2;
    protected (AI, int, int) AttakeTarget;
    protected float BestPoint;
    internal bool NPCPrepera = false;
    internal bool NPC_Prefire;
    internal List<ISkill> Skills=new List<ISkill>();
    internal bool Ult=false;

    protected virtual bool CalPointAction(Tile T)
    {
        float Point = 0;
        Vector3 Location = T.transform.position;
        (AI, int, int) aim=(null,0,0);
        float MinDis = 99;
        if (Enemies == null)
        {
            Enemies = RoundSysytem.GetInstance().Humans;
        }
        foreach (AI enemy in Enemies)//有障礙物則加分
        {
            float a;
            Vector3 Edir = enemy.transform.position - Location;
            if (T.AdjCoverList[FindDirection(Edir)] == Tile.Cover.FullC)
            {
                a = 2f / Edir.magnitude;
                if (a>2)
                    Point += 2;
                else
                {
                    Point += a;
                }
            }
            else if (T.AdjCoverList[FindDirection(Edir)] == Tile.Cover.HalfC)
            {
                a = 1f / Edir.magnitude;
                if (a > 1)
                    Point += 1;
                else
                {
                    Point += a;
                }


                Point += 2f;// Edir.magnitude;
            }
            if (MinDis > Edir.magnitude)
            {
                MinDis = Edir.magnitude;
            }
        }
        if (MinDis < 3f)
        {
            Point = 1.5f;
        }
        else if (MinDis < 6f)
        {
            Point += 3f;
        }
        else if (MinDis < 9f)
        {
            Point += 2f;
        }else if (MinDis < 12f)
        {
            Point += 1f;
        }else if (MinDis <15f)
        {
            Point -= 2f;
        } 
         ///可用能力巡一遍，選擇得分高的能力 再拿出來加分
        ISkill Sec = null ;
        ISkill Sec2 = null;
        float SecPoint = 0;
        if (Skills != null&&!Ult) 
        {
            if (T.distance <= Cha.Mobility)
            {
                if (T.distance != 0)
                {
                    aim = AttakeableDetect(T);
                    foreach (var skill in Skills)
                    {
                        if (skill.CheckUseable(aim.Item1))
                        {
                            float TmpPoint = skill.Point;
                            TmpPoint += aim.Item3 * skill.AimPoint;
                            if (SecPoint < TmpPoint)
                            {
                                Sec2 = skill;
                                SecPoint = TmpPoint;
                            }
                        }
                    }
                }
                else
                {
                    aim = AttakeableDetect(T);

                    foreach (var skill in Skills)
                    {
                        if (skill.CheckUseable(aim.Item1))
                        {
                            float TmpPoint = skill.Point;
                            TmpPoint += aim.Item3 * skill.AimPoint;
                            if (2 - skill.AP > 0)
                            {
                                foreach (var skill2 in Skills)
                                {
                                    if (skill2.CheckUseable(aim.Item1))
                                    {
                                        if (skill == skill2) { continue; }
                                        TmpPoint += skill.Point;
                                        TmpPoint += aim.Item3 * skill.AimPoint;
                                        if (SecPoint < TmpPoint)
                                        {
                                            Sec = skill;
                                            Sec2 = skill2;
                                            SecPoint = TmpPoint;
                                        }
                                    }
                                }
                            }
                            else if (SecPoint < TmpPoint)
                            {
                                Sec = skill;
                                SecPoint = TmpPoint;
                            }
                        }
                    }
                }
            }
        }

        //todo特殊能力 先確認CD 如果可以用 在計算命中 得分會比普通射擊高一些
        Point += SecPoint;
        if (Point > BestPoint)
        {
            BestT = T;
            BestPoint = Point;
            AttakeTarget = aim;
            if (Sec != null)
            {
                Acting = Sec;
            }
            else
            {
                Acting = null;
            }
            if (Sec2 != null)
            {
                Acting2 = Sec2;
            }
            else
            {
                Acting2 = null;
            }
        }
        return true;
    }

    public (AI, int, int) AttakeableDetect(Tile T)//回傳目標，射擊位置，命中率
    {
        (AI, int, int) BestAim = (null, -1, 0);
        foreach (AI Enemy in Enemies)//改queue ?
        {
            Vector3 Location = T.transform.position;
            Vector3 Target = Enemy.CurrentTile.transform.position;
            Vector3 RTDiv = Location - Target;
            int i = FindDirection(RTDiv);
            if (Enemy.CurrentTile.AdjCoverList[i] == Tile.Cover.FullC)//判斷敵人有無FullCover，如有則瞄準障礙物邊邊。
            {
                int j = 0;
                if (Vector3.Cross(Direction(i), RTDiv).y >= 0)
                {
                    j = (i + 3) % 4;
                }
                else
                {
                    j = (i + 1) % 4;
                }
                Target += (Direction(i) + Direction(j)) * 0.3f;
            }
            Vector3 TDiv = Target - Location;
            i = FindDirection(TDiv);
            if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
            {//確保路徑上沒有障礙物
                int j = CalculateAim(Enemy, T.transform.position);
                if (j > BestAim.Item3)
                {
                    BestAim = (Enemy, -1, j);
                }
            }
            else if (T.AdjCoverList[i] == Tile.Cover.FullC) //如攻擊方有障礙物，則站出去瞄準。
            {
                float LoR = Vector3.Cross(Direction(i), TDiv).y;
                if (LoR != 0)
                {
                    if (LoR > 0)
                    {
                        i = (i + 3) % 4;
                    }
                    else
                    {
                        i = (i + 1) % 4;
                    }
                    Location += Direction(i) * 0.67f;
                    if (T.AdjList[i].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            int j = CalculateAim(Enemy, Location);
                            if (j > BestAim.Item3)
                            {
                                BestAim = (Enemy, i, j);
                            }
                        }
                    }
                }
                else
                {
                    if (T.AdjList[(i + 1) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            int j = CalculateAim(Enemy, Location);
                            if (j > BestAim.Item3)
                            {
                                BestAim = (Enemy, (i + 1) % 4, j);
                            }
                        }
                    }
                    if (T.AdjList[(i + 3) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            int j = CalculateAim(Enemy, Location);
                            if (j > BestAim.Item3)
                            {
                                BestAim = (Enemy, (i + 3) % 4, j);
                            }
                        }
                    }
                }
            }
        }
        return BestAim;
    }
    private int FriendLayer()
    {
        if (EnemyLayer == 1 << 10)
        {
            return 1 << 11;
        }
        else
        {
            return 1 << 10;
        }
    }
    public void PreFire()
    {
        Target = AttakeTarget.Item1;
        TargetDir = Target.transform.position - transform.position;
        TargetDir.y = 0;
        ChangeTarget = true;
        ChangePreAttakeIdle(TargetDir);
        PreAttack = true;
        NPCPrepera = false;
        DoActing = Fire;//method.invoke
    }

    public void Fire()
    {
        Debug.Log(AttakeTarget.Item3);
        int i = Random.Range(0, 100);
        NPC_Prefire = true;
        UI.MoveCam.att_cam_bool = true;
        if (AttakeTarget.Item3 < i)//Miss
        {
            Miss = true;
            RaycastHit RH;
            Vector3 ShotPoint = CurrentTile.transform.position + new Vector3(0, 1.34f, 0) + Direction(AttakeTarget.Item2)+TargetDir.normalized*0.67f;
            Vector3 RandPoint;
            int j = 0;
            while (true)
            {
                if (j>10)
                {
                    Debug.Log("Miss");
                    AttackPoint = AttakeTarget.Item1.BeAttakePoint.position + Vector3.up * 0.3f + Vector3.Cross((AttakeTarget.Item1.BeAttakePoint.position -ShotPoint),Vector3.up).normalized* 0.3f;
                    break;
                }
                RandPoint = AttakeTarget.Item1.BeAttakePoint.position
                    + new Vector3(Random.Range(-0.67f, 0.67f), Random.Range(-0.2f, 0.2f), Random.Range(-0.67f, 0.67f));
                if (Physics.SphereCast(ShotPoint, 0.05f, RandPoint-ShotPoint, out RH, 30f))
                {
                    if (RH.collider.tag != "Human" && RH.collider.tag != "Alien")
                    {
                        AttackPoint = RandPoint;
                        break;
                    }
                }
                else
                {
                    AttackPoint = RandPoint;
                    break;
                }
                ++j;
            }
        }
        else
        {
            Miss = false;
            AttackPoint = AttakeTarget.Item1.BeAttakePoint.position;
            Target.BeDamaged(Gun.Damage[Random.Range(0, Gun.DamageRange)]);
        }


        if (Am.GetBool("FCover"))
        {
            if (AttakeTarget.Item2 == -1)
            {
                Am.SetBool("Aim", true);
                AttackPosition = CurrentTile.transform.position;
                FW = FullCoverFireWait2;
            }
            else
            {
                Am.SetBool("Run", true);

                Vector3 dir = Direction(AttakeTarget.Item2);

                AttackPosition = transform.position + dir * 0.67f;

                FW = FullCoverFireWait;
            }
        }
        else if (Am.GetBool("HCover"))
        {
            Am.SetBool("Aim", true);
        }
        else
        {
            ;
        }
        Attack = true;
        DoActing = null;
        NPCPrepera = false;
        AttakeTarget.Item1.BeAim(this);
    }


    public virtual void ConfirmAction()
    {
        if (BestT != CurrentTile)
        {
            DoActing = PreMove;
            NPCPrepera = true;
        }
        else if (Acting != null )
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




    public void AIReload()
    {
        ResetBool();
        Gun.bullet = Gun.MaxBullet;
        AP -= 1;
        Am.SetTrigger("Reload");
        //Am.set
        if (Acting2 != null)
        {
            DoActing = Acting2.GetAction();
            ActionName = Acting2.Name;
            Acting2.EnterCD();
            Acting2 = null;
        }
        else
        {
            
            PreAttack = false;
            NPCPrepera = false;
            EndTurn();
        }
    }


    public virtual void PreMindControl ()
    {
        Target = AttakeTarget.Item1;
        TargetDir = Target.transform.position - transform.position;
        TargetDir.y = 0;
        ChangeTarget = true;
        ChangePreAttakeIdle(TargetDir);
        PreAttack = true;
        NPCPrepera = false;
        DoActing = MindControl;
        FindObjectOfType<SoundManager>().Play(Gun.weapon2);
    }



    protected AI MindControlAI;


    public void MindControl()
    {
        NPC_Prefire = true;
        UI.MoveCam.att_cam_bool = true;

        Miss = false;
        AttackPoint = AttakeTarget.Item1.BeAttakePoint.position;


        if (Am.GetBool("FCover"))
        {
            if (AttakeTarget.Item2 == -1)
            {
                Am.SetBool("Aim", true);
                AttackPosition = CurrentTile.transform.position;
                FW = FullCoverFireWait2;
            }
            else
            {
                Am.SetBool("Run", true);

                Vector3 dir = Direction(AttakeTarget.Item2);

                AttackPosition = transform.position + dir * 0.67f;

                FW = FullCoverFireWait;
            }
        }
        else if (Am.GetBool("HCover"))
        {
            Am.SetBool("Aim", true);
        }
        else
        {
            ;
        }
        MindControlAI = AttakeTarget.Item1;
        Attack = true;
        DoActing = null;
        NPCPrepera = false;
        AttakeTarget.Item1.BeAim(this);
        
    }
    public void CreatMindC()
    {
        RS.EndChecked = false;
        GameObject GO = Instantiate<GameObject>(Resources.Load<GameObject>("MindControl"));
        GO.transform.position = FirePoint.position;
        GO.transform.SetParent(FirePoint);
        StartCoroutine(ShotMindC(GO));        
    }
    public IEnumerator ShotMindC(GameObject go)
    {
        yield return new WaitForSeconds(50f / 60f);
        go.transform.SetParent(null);
        TargetDir = Target.BeAttakePoint.position - FirePoint.position;
        go.GetComponent<bullet>().enabled = true;
        go.GetComponent<bullet>().SetAttackPoint(FirePoint.position, Target.BeAttakePoint.position);
        go.GetComponent<bullet>().Hit = true;
        float sec = TargetDir.magnitude / 10f;
        go.transform.forward = TargetDir;
        Attack = false;

        StartCoroutine(Target.BeMindControl(sec));
    }

    public IEnumerator BeMindControl(float Sec)
    {
        UI.MoveCam.cam_dis = 20.0f;//一開始預設攝影機距離為20公尺
        UI.per_but = false; //我方切換子彈預設為關
        UI.MoveCam.att_cam_bool = false;
        yield return new WaitForSeconds(Sec);
        UI.MoveCam.ChaTurn(this);
        Am.Play("Agony");
        GameObject go = Instantiate<GameObject>(Resources.Load<GameObject>("MindCing"));
        Transform head = BeAttakePoint.GetChild(0).Find("Head");
        go.transform.position = head.position;
        go.transform.SetParent(head);
        Cha.camp = Character.Camp.Alien;
        Enemies.Add(this);
        Enemies = RS.Humans;
        Enemies.Remove(this);
        EnemyLayer = 1 << 11;
        AIState = NpcAI;
        yield return new WaitForSeconds(1f);
        UI.ChangeLogo(this);
        UI.DestroyHPBar(this);
        UI.CreateHP_Bar(this, Cha.MaxHP, Cha.HP);
        yield return new WaitForSeconds(0.5f);
        UI.CheckEvent();
        RS.EndChecked = true;
        FindObjectOfType<SoundManager>().Play(Cha.MindControlled);
    }

    public IEnumerator RecoverMind(AI enemy)
    {
        RS.EndChecked = false;
        yield return new WaitForSeconds(1f);
        UI.MoveCam.ChaTurn(this);
        Transform MCing = BeAttakePoint.GetChild(0).Find("Head").Find("MindCing(Clone)");
        Destroy(MCing.gameObject);
        Cha.camp = Character.Camp.Human;
        Enemies.Add(this);
        Enemies = RS.Aliens;
        Enemies.Remove(this);
        EnemyLayer = 1 << 10;
        AIState = PlayerAI;
        UI.ChangeLogo(this);
        UI.DestroyHPBar(this);
        UI.CreateHP_Bar(this, Cha.MaxHP, Cha.HP);
        RS.EndChecked = true;
        Destroy(enemy);
    }


    public virtual void PreMelee2()
    {
        MoveToTile(BestT);
        Am.SetBool("Run", true);
        ResetBool();
        Idle = NoCover;
        PreAttakeIdle = PreAtkNoCover;
        Moving = true;
        RemoveVisitedTiles();//重置Tile狀態
        OutCurrentTile();
        InCurrentTile(BestT);
        DoActing = null;
        Am.Play("Run");
        Attack = true;

    }

    public void status(string s)
    {
        Debug.Log("-------------------------------: " + s);
        if (s == "Demage")
        {
            if (Miss)
            {
                UI.status("Miss",this);
            }
            else if (Miss == false)
            {
                UI.status("Demage", this);
            }
            else
                UI.status("Demage",this);
        }
        else
            UI.status(s,this);
    }
    public void CountCD()
    {
        foreach (var skill in Skills)
        {
            skill.CountCD();
        }
    }


}

