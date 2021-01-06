using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



public class AI : MonoBehaviour
{
    public Animator Am;
    protected AnimatorStateInfo stateinfo;
    public Transform enemy;
    public delegate IEnumerator Movement();
    protected Action Idle = null;
    public Character Cha;
    public int AP = 0;
    protected Vector3 Ediv;
    protected int EnemyLayer;
    protected int TileCount;
    public bool AmTurn = false;
    
    
    //動畫
    protected void NoCover()//沒Cover的狀態
    {
        if (!AmTurn && stateinfo.IsName("Idle"))
        {         
            transform.forward = Direction(TileCount);
            Ediv = (enemy.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, Ediv);

            if (FoB > 1 / Mathf.Sqrt(2) - 0.1f)   //判斷前後左右
            {
                TileCount = FindDirection(transform.forward);
                if (CurrentTile.AdjCoverList[TileCount] == Tile.Cover.None)
                {
                    if (FoB> 0.99f)
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
                        Am.SetBool("FCover", true);
                        TileCount = i;
                    }

                }
                else if (CurrentTile.AdjCoverList[TileCount] == Tile.Cover.HalfC)
                {
                    Am.SetBool("HCover", true);
                    Idle = HalfCover;
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
            Ediv = (enemy.position - transform.position).normalized;
            Vector3 CDir = Direction(TileCount);
            float CFoB = Vector3.Dot(CDir, Ediv);
            if (CFoB > 0)
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
        Idle = NoCover;
        AmTurn = false;
        Am.SetBool("Turn", false);
        Am.SetBool("Back", false);
        Am.SetBool("FCover", false);
        Am.SetBool("HCover", false);
        Am.SetBool("Left", false);
        Am.SetBool("Right", false);
        Am.SetBool("Aim", false);
        Am.SetBool("Fire", false);
        Target = null;
    }


    public void Skip()//跳過回合
    {
        AP = 0;
        Turn = false;
        NPCPreaera = false;
        DoActing = null;
    }





    //移動

    public List<Tile> VisitedTiles = new List<Tile>();
    public Stack<(Tile, MoveWay)> Path = new Stack<(Tile, MoveWay)>();      //tile的資料設定為Stack(後進先出)
    public Tile CurrentTile;      //玩家目前站的tile
    public bool Turn = false;       //回合判斷
    public bool Moving = false;     
    public bool Running = false;
    public float MoveSpeed = 4;
    protected float ChaHeight;      //角色高度
    public bool End = false;
    Vector3 Heading;                //移動方向
    public UISystem UI;
    public bool Prepera = false;
    public enum MoveWay    //移動方式(簡化
    {
        Run=0,
        Ladder,
        Jump,       
    }

    public void MoveRange() //開始計算移動範圍
    {
        if (AP == 2)
        {
            Queue<Tile> Process = new Queue<Tile>();
            Process.Enqueue(CurrentTile);
            AddVisited(CurrentTile);
            Process = FindSelectableTiles(Process,1); //第一個AP 的範圍
            UI.DrawMRLine(Process,UI.Blue,1.0f);          //畫線
            Process = FindSelectableTiles(Process,2);   //return的是之後畫線要的格子，格子畫好後又會塞給FindSelectableTiles
            UI.DrawMRLine(Process, UI.Yellow,2.0f);
            Process.Clear();
            CurrentTile.selectable = false;
        }
        else if (AP == 1)
        {
            Queue<Tile> Process = new Queue<Tile>();
            Process.Enqueue(CurrentTile);
            AddVisited(CurrentTile);
            Process = FindSelectableTiles(Process, 1);
            UI.DrawMRLine(Process, UI.Yellow,1.0f);
            Process.Clear();
            CurrentTile.selectable = false;
        }


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

            if ((transform.position - target).magnitude >= 0.05f)
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
                    CurrentTile.walkable = true;
                    CurrentTile = Path.Peek().Item1;
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


            //AttakeAbleList.Clear();
            //UI.LRDestory();
            CurrentTile.walkable = false;
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


    void Run(Vector3 target)
    {
        if ((transform.position - target).magnitude >= 0.05f)
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
            }
            transform.position = target;

            Path.Pop();
        }
    }
    void Across()
    {

    }
    void Jump()
    {

    }
    void Ladder()
    {

    }
    void Climbup()
    {

    }
    void ClimbDown()
    {

    }

    public void AddVisited(Tile T)
    {
        VisitedTiles.Add(T);
        T.visited = true;
    }
    public void RemoveVisitedTiles()
    {

        foreach (Tile tile in VisitedTiles)
        {
            tile.Reset();
        }
        VisitedTiles.Clear();
    }



    //攻擊
    protected List<AI> Enemies;
    public LinkedList<(AI, int ,int )> AttakeableList = new LinkedList<(AI, int, int)>();//角色,射擊位置,命中率;
    public LinkedList<(AI, int, int)> PreAttakeableList;
    public Weapon Gun;
    public bool PreAttack = false;
    protected bool Death = false;
    public bool Attack;
    public bool ChangeTarget = false;
    public AI Target;
    protected Vector3 TargetDir;
    public Transform FirePoint;
    public GameObject Bullet;
    public Transform BeAttakePoint;
    internal Vector3 AttackPoint;
    internal bool Miss = false;
    internal Vector3 AttackPosition;
    protected List<AI> AttPredict = new List<AI>();
    public bool BeAimed = false;

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
            if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
            {//確保路徑上沒有障礙物
                AttakeableList.AddLast((Enemy, -1, CalculateAim(Enemy, CurrentTile)));
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
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            AttakeableList.AddLast((Enemy, i, CalculateAim(Enemy, CurrentTile)));//todo
                            continue;
                        }
                    }
                }
                else
                {
                    if (CurrentTile.AdjList[(i + 1) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            AttakeableList.AddLast((Enemy, (i + 1) % 4, CalculateAim(Enemy, CurrentTile)));//todo
                            continue;
                        }
                    }
                    if (CurrentTile.AdjList[(i + 3) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1.2f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            AttakeableList.AddLast((Enemy, (i + 3) % 4, CalculateAim(Enemy, CurrentTile)));//todo
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
            if (!Physics.Raycast(Location + new Vector3(0, 1f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
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
                        if (!Physics.Raycast(Location + new Vector3(0, 1f, 0), TDiv, TDiv.magnitude, 1 << 9))
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
                        if (!Physics.Raycast(Location + new Vector3(0, 1f, 0), TDiv, TDiv.magnitude, 1 << 9))
                        {//確保路徑上沒有障礙物
                            AttPredict.Add(Enemy);
                            continue;
                        }
                    }
                    if (T.AdjList[(i + 3) % 4].walkable)
                    {//判斷旁邊可以站
                        TDiv = Target - Location;
                        if (!Physics.Raycast(Location + new Vector3(0, 1f, 0), TDiv, TDiv.magnitude, 1 << 9))
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

    int CalculateAim(AI Enemy,Tile Location)
    {
        Vector3 div = Location.transform.position - Enemy.CurrentTile.transform.position;
        div.y = 0;
        Tile.Cover[] cover;
        float dis = div.magnitude;
        int AimAngle;
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
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 40 + 20 * AimAngle / 45;
                }
                else if(cover[1] == Tile.Cover.HalfC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 20;
                }
                else
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 20 + 10 * AimAngle / 45;
                }
            }
            else
            {
                if(cover[0] == Tile.Cover.FullC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 40 * (1 - AimAngle / 45);
                }
                else if(cover[1] == Tile.Cover.HalfC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.CeilToInt(dis)] - 20*(1 - AimAngle / 45);
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
        NPCPreaera = true;
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
            if (Vector3.Cross(transform.forward, dir).y >= 0)
            {
                Am.SetBool("Right", true);
            }
            else
            {
                Am.SetBool("Left", true);
            }
            Am.SetBool("FCover", true);

            Am.SetBool("HCover", false);

            TileCount = FindDirection(dir);
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
                StartCoroutine(AIWaitPreAtkChange());//todo
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
        }
        else
        {
            if(!Am.GetBool("HCover")&& !Am.GetBool("FCover")&& !ChangeTarget)
            {
                PreAttakeIdle = PreAtkNoCover;
                Am.SetBool("Aim", true);
                StartCoroutine(AIWaitPreAtkChange());
                return;
            }
            Am.SetBool("FCover", false);
            Am.SetBool("Left", false);
            Am.SetBool("HCover", false);
            Am.SetBool("Right", false);
            PreAttakeIdle = PreAtkNoCover;
            ChangeTarget = true;
            Idle = NoCover;

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

         if (stateinfo.IsName("RightTurn") || stateinfo.IsName("LeftTurn")&& Am.GetBool("Turn"))
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
                AttakeableList.Clear();
                UI.LRDestory();
                Am.SetBool("Fire", true);
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
            if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.98f)
            {
                Am.SetBool("Aim", true);
                transform.forward = TargetDir;
                if (stateinfo.normalizedTime > 0.8f && stateinfo.IsName("Aim"))
                {
                    AP = 0;
                    AttakeableList.Clear();
                    UI.LRDestory();
                    Am.SetBool("Aim", false);
                    Am.SetBool("Fire", true);
                    RemoveVisitedTiles();
                    StartCoroutine(FireWait());
                }

            }
            else 
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
        }
        else if (ChangeTarget)
        {
            stateinfo = Am.GetCurrentAnimatorStateInfo(0);

            float FoB = Vector3.Dot(transform.forward, TargetDir.normalized);
            Vector3 LoR = Vector3.Cross(transform.forward, TargetDir);
            if (FoB > 1 / Mathf.Sqrt(2) - 0.01f&& stateinfo.normalizedTime >= 1.0f)
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
                    AttakeableList.Clear();
                    UI.LRDestory();
                    Am.SetBool("Aim", false);
                    Am.SetBool("Fire", true);
                    RemoveVisitedTiles();
                    StartCoroutine(FW());
                //}
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
            }
            //else if (Am.GetBool("Turn")&&!stateinfo.IsName("RunToAttack"))
            //{
            //    if (stateinfo.normalizedTime > 0.9f)
            //    {
            //        Am.SetBool("Aim", true);
            //        Am.SetBool("Turn", false);
            //    }
            //}
            else if(stateinfo.IsName("LeftToAttack")|| stateinfo.IsName("RightToAttack"))
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
        int i =  Random.Range(0, 100);
        if (FireTarget.aim < i)//Miss
        {
            Miss = true;
            RaycastHit RH;
                Vector3 ShotPoint = CurrentTile.transform.position + new Vector3(0, 1.34f, 0) + Direction(FireTarget.location);
            while (true)
            {
                if (Physics.Raycast(ShotPoint, (Target.transform.position
                    + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))) - ShotPoint,out RH))
                {
                    if (RH.collider.tag == "En")
                    {
                        AttackPoint = RH.point;

                        var RHObsetacle = RH.transform.GetComponent<Obstacle>();
                        if (RHObsetacle != null)
                            RHObsetacle.TakeDamage(Gun.Damage[Random.Range(0, Gun.DamageRange - 1)]);
                        break;
                    }
                }
            }
        }
        else
        {
            Miss = false;
            AttackPoint = Target.BeAttakePoint.position;
            Target.BeDamaged(Gun.Damage[Random.Range(0, Gun.DamageRange - 1)]);
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



        //AP = 0;
        //AttakeableList.Clear();
        //UI.LRDestory();
        //Am.SetBool("Fire", true);
        //RemoveVisitedTiles();
        //StartCoroutine(FireWait());
        //PreAttack = false;
    }

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
        }
        
        B.transform.forward = TargetDir;
        Attack = false;
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
        //yield return new WaitUntil(() => stateinfo.IsName("LeftToAttack") || stateinfo.IsName("RightToAttack"));

        Vector3 dir = AttackPosition - CurrentTile.transform.position;
        dir.y = 0;
        transform.forward = Direction(TileCount);
        AttackPosition = CurrentTile.transform.position;
        yield return new WaitUntil(() => PreAttack == false);
        transform.forward = Direction(TileCount);
        Am.SetBool("Fire", false);
        EndTurn();

    }
    public IEnumerator FullCoverFireWait2()
    {
        PreAttack = false;
        yield return new WaitUntil(() => stateinfo.IsName("Fire"));
        Am.SetBool("Fire", false);
        yield return new WaitForSeconds(1f);
        //transform.forward = Direction(TileCount);
        EndTurn();
        PreAttack = false;
        //ResetBool();
    }

    protected void EndTurn()
    {
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
        Turn = false;
    }




    
    
    public void BeDamaged(int damage)
    {
        Cha.HP -= damage;//todo UI?
    }
    private void AIDeath()
    {
        CurrentTile.walkable = true;
        Am.Play("Death");
        Death = true;
        RoundSysytem.GetInstance().DeathKick(this);
        UI.DeathKick(this);
        CurrentTile.walkable = true;
        Destroy(Cha);
        Destroy(this);
    }

    public void Hurt(Vector3 dir , Vector3 Pos)
    {
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
            ResetBool();
            Am.Play("Hurt");
            Idle = NoCover;
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

        StartCoroutine( WaitNextAction());
    }
    protected IEnumerator WaitNextAction()
    {
        yield return new WaitForSeconds(0.5f);
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





















    /////AI

    public void FindSelectableTiles()
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
                    if (adjT.distance > Cha.Mobility * 2) //移動距離不會超過上限
                    {
                        continue;
                    }
                    adjT.Parent = T;  //visited過的就被設為 parent
                    AddVisited(adjT);
                    CalPointAction(adjT);
                    Process.Enqueue(adjT);
                }
            }
        }
    }


    protected void PreMove()
    {
        MoveToTile(BestT);
        Am.SetBool("Run", true);
        ResetBool();
        Idle = NoCover;
        Moving = true;
        RemoveVisitedTiles();//重置Tile狀態
        CurrentTile.walkable = true;
        CurrentTile = BestT;
        CurrentTile.walkable = false;
        DoActing = null;
        Am.Play("Run");

    }

    public void Move2()
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
                DoActing = Acting2;
                Acting2 = null;
                //StartCoroutine(MoveWait());
            }
            else
            {
                Turn = false;
                PreAttack = false;
                NPCPreaera = false;
                ResetBool();
            }
        }
    }
    //protected IEnumerator MoveWait()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    DoActing = Acting2;
    //    Acting2 = null;
    //}

    






    protected Tile BestT;
    public Action DoActing;
    protected Action Acting;
    protected Action Acting2;
    protected (AI, int, int) AttakeTarget;
    protected int BestPoint;
    public bool NPCPreaera = false;
    public bool NPC_Prefire;

    protected void CalPointAction(Tile T)
    {
        int Point = 0;
        Vector3 Location = T.transform.position;
        bool Reload = false;
        (AI, int, int) aim=(null,0,0);
        float MinDis = 99;
        foreach (AI enemy in Enemies)//有障礙物則加分
        {
            Vector3 Edir = enemy.transform.position - Location;
            if (T.AdjCoverList[FindDirection(Edir)] == Tile.Cover.FullC)
            {
                Point += 2;
            }
            else if (T.AdjCoverList[FindDirection(Edir)] == Tile.Cover.HalfC)
            {
                Point += 1;
            }
            if (MinDis > Edir.magnitude)
            {
                MinDis = Edir.magnitude;
            }
        }
        int i = 16 / ((int)MinDis + 1);
        if (i > 4)
        {
            Point += 4;
        }
        else
        {
            Point += i;
        }
        //可用能力巡一遍，選擇得分高的能力 再拿出來加分
        (Action, int point) Sec = (null, 0);

        if (T.distance < Cha.Mobility)
        {
            if (T.distance != 0)
            {
                if (Gun.bullet != 0)
                {
                    aim = AttakeableDetect(T);
                    if (aim.Item1 != null)
                    {
                        Sec.point += 2;
                        Sec.Item1 = PreFire;
                        Sec.point += aim.Item3 / 20;
                    }
                }
                else
                {
                    Reload = true;
                    Sec.point += 1;
                }
            }
            else
            {
                if (Gun.bullet == 0)
                {
                    //reload 4分
                    Sec.point += 1;
                    Reload = true;

                }
                aim = AttakeableDetect(T);
                if (aim.Item1 != null)
                {
                    Sec.point += 1;
                    Sec.point += aim.Item3 / 20;
                    Sec.Item1 = PreFire;
                }

            }
        }

        //todo特殊能力 先確認CD 如果可以用 在計算命中 得分會比普通射擊高一些
        Point += Sec.point;
        if (Point > BestPoint)
        {
            BestT = T;
            BestPoint = Point;
            if (T.distance > 0)
            {
                Acting = Move2;
            }
            if (Reload)
            {
                //if (Acting != null)
                //    //Acting2 = reload;
                //else
                //acting=reload

            }
            if (Acting != null)
            {
                Acting2 = Sec.Item1;
                AttakeTarget = aim;
            }
            else
            {
                Acting = Sec.Item1;
                AttakeTarget = aim;
            }

        }
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
                int j = CalculateAim(Enemy, T);
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
                            int j = CalculateAim(Enemy, T);
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
                            int j = CalculateAim(Enemy, T);
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
                            int j = CalculateAim(Enemy, T);
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

    protected void PreFire()
    {
        Target = AttakeTarget.Item1;
        TargetDir = Target.transform.position - transform.position;
        TargetDir.y = 0;
        ChangeTarget = true;
        ChangePreAttakeIdle(TargetDir);
        PreAttack = true;
        NPCPreaera = false;
        DoActing = Fire;
        NPC_Prefire = true;
        UI.MoveCam.att_cam_bool = true;
    }

    public void Fire()
    {
        int i = Random.Range(0, 100);
        NPC_Prefire = true;
        UI.MoveCam.att_cam_bool = true;
        if (AttakeTarget.Item3 < i)//Miss
        {
            Miss = true;
            RaycastHit RH;
            Vector3 ShotPoint = CurrentTile.transform.position + new Vector3(0, 1.34f, 0) + Direction(AttakeTarget.Item2);
            while (true)
            {
                if (Physics.Raycast(ShotPoint, (AttakeTarget.Item1.transform.position
                    + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))) - ShotPoint, out RH))
                {
                    if (RH.collider.tag == "En")
                    {
                        AttackPoint = RH.point;

                        var RHObsetacle = RH.transform.GetComponent<Obstacle>();
                        if (RHObsetacle != null)
                            RHObsetacle.TakeDamage(Gun.Damage[Random.Range(0, Gun.DamageRange - 1)]);
                        break;
                    }
                }
            }
        }
        else
        {
            Miss = false;
            AttackPoint = AttakeTarget.Item1.BeAttakePoint.position;
            Target.BeDamaged(Gun.Damage[Random.Range(0, Gun.DamageRange - 1)]);
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
        NPCPreaera = false;
        AttakeTarget.Item1.BeAim(this);
        //AP = 0;
        //AttakeableList.Clear();
        //UI.LRDestory();
        //Am.SetBool("Fire", true);
        //RemoveVisitedTiles();
        //StartCoroutine(FireWait());
        //PreAttack = false;
    }


    public void ConfirmAction()
    {
        if (Acting == Move2)
        {
            DoActing = PreMove;
            Acting = null;
        }
        else if(Acting == PreFire)
        {
            DoActing = PreFire;
            Acting = null;
        }//else if(Acting=reload)

        if (Acting2 == PreFire)
        {
            //ChangePreAttakeIdle();
        }//else if (Acting2==reload)

        NPCPreaera = true;
    }


    public void AIReload()
    {
        Gun.bullet = Gun.MaxBullet;
        AP -= 1;
        //Am.set
        if (Acting2 != null)
        {
            DoActing = Acting2;
            Acting2 = null;
        }
        else
        {
            Turn = false;
            PreAttack = false;
            NPCPreaera = false;
            ResetBool();
        }
    }

    protected void MindControl()
    {

    }


}

