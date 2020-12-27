using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Playables;


public class AI : MonoBehaviour
{
    public Animator Am;
    protected AnimatorStateInfo stateinfo;
    public Transform enemy;
    public delegate IEnumerator Movement();
    public Movement MV = null;
    protected Action Idle = null;
    public Character Cha;
    public int AP = 0;
    protected Vector3 Ediv;
    protected int EnemyLayer;
    protected int TileCount;
    public PlayableDirector playableDirector;

    
    
    //動畫
    protected void NoCover()//沒Cover的狀態
    {
        if (MV == null && stateinfo.IsName("Idle"))
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
                    Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
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
                MV = BackTurn;
                TileCount = (TileCount + 2) % 4;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                if (LoR.y > 0.0f)
                {
                    Am.SetBool("Right", true);
                    Am.SetBool("Turn", true);
                    MV = RightTurn;
                    TileCount = (TileCount + 3) % 4;
                }
                else
                {
                    Am.SetBool("Left", true);
                    Am.SetBool("Turn", true);
                    MV = LeftTurn;
                    TileCount = (TileCount + 1) % 4;
                }

            }

        }
    }


    protected void HalfCover()
    {
        if (MV == null && stateinfo.IsName("Idle"))//
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
                            Idle = FullCover;
                            Am.SetBool("HCover", false);
                            Am.SetBool("FCover", true);
                            TileCount = i;
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
                    MV = RightTurn;
                }
                else
                {
                    Am.SetBool("Left", true);
                    Am.SetBool("Turn", true);
                    MV = LeftTurn;
                }

            }
        }
    }

    protected void FullCover()
    {
        if (MV == null && stateinfo.IsName("Idle"))//
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
                TileCount = FindDirection(Ediv);
                Am.SetBool("Left", false);
                Am.SetBool("Right", false);
                Am.SetBool("HCover", false);
                Am.SetBool("FCover", false);
                Idle = NoCover;
            }
        }
    }


 



    //轉向動畫控制
    public virtual IEnumerator LeftTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, -90f, 0);
            Am.SetBool("Left", false);
            Am.SetBool("Turn", false);
            MV = null;
        }
        else
            yield return LeftTurn();
    }
    public virtual IEnumerator RightTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, 90f, 0);
            Am.SetBool("Right", false);
            Am.SetBool("Turn", false);
            MV = null;
        }
        else
            yield return RightTurn();
    }
    public virtual IEnumerator BackTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, 180f, 0);
            Am.SetBool("Back", false);
            Am.SetBool("Turn", false);
            MV = null;
        }
        else
            yield return BackTurn();
    }

    public void ResetBool()
    { 
        Am.SetBool("Aim", false);
        Am.SetBool("Fire", false);
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
    public UISystem UI = UISystem.getInstance();
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
    protected void CheckMouse()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "tile")
            {
                Tile T = hit.collider.GetComponent<Tile>();
                if (T.selectable)
                {
                    if (T.distance > Cha.Mobility)
                        AP = 0;
                    else
                        --AP;
                    Am.SetBool("Run", true);
                    Am.SetBool("FCover", false);
                    Am.SetBool("Left", false);
                    Am.SetBool("Right", false);
                    Am.SetBool("HCover", false);
                    Idle = NoCover;
                    Moving = true;
                    RemoveVisitedTiles();//重置Tile狀態
                    UI.Prepera = false;

                    //CurrentTile.walkable = false;
                    AttakeableList.Clear();
                    UI.LRDestory();
                }
            }
        }
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
            if (AP != 0)
            {
                MoveRange();
                AttakeableDetect();

            }
            else
                Turn = false;
        }
    }

    protected Vector3 Direction(int tilecount)
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
    protected float RotationProgress = 0;
    public Transform FirePoint;
    public GameObject Bullet;
    public Transform BeAttakePoint;
    private Vector3 AttackPoint;
    protected bool Miss = false;
    protected Vector3 AttackPosition;

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
            if (!Physics.Raycast(Location + new Vector3(0, 1f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
            {//確保路徑上沒有障礙物
                AttakeableList.AddLast((Enemy, -1, CalculateAim(Enemy.CurrentTile, CurrentTile)));
            } 
            else if (CurrentTile.AdjCoverList[i] == Tile.Cover.FullC) //如攻擊方有障礙物，則站出去瞄準。
            {
                if (Vector3.Cross(Direction(i), TDiv).y >= 0) 
                {
                    i = (i + 3) % 4;
                }
                else
                {
                    i = (i + 1) % 4;
                }
                Location += Direction(i) * 0.67f;
                if (CurrentTile.AdjCoverList[i] == Tile.Cover.None 
                    && Physics.CheckBox(Location, new Vector3(0.1f, 0, 0), Quaternion.identity, 1 << 8))
                {//判斷旁邊可以站
                    TDiv = Target - Location;
                    if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), TDiv, TDiv.magnitude, 1 << 9))
                    {//確保路徑上沒有障礙物
                        AttakeableList.AddLast((Enemy, i, CalculateAim(Enemy.CurrentTile, CurrentTile)));//todo
                        continue;
                    }
                }
            }
            else if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
            {//確保路徑上沒有障礙物
                AttakeableList.AddLast((Enemy, -1, CalculateAim(Enemy.CurrentTile, CurrentTile)));
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


    int CalculateAim(Tile Enemy,Tile Location)
    {
        Vector3 div = Location.transform.position - Enemy.transform.position;
        Tile.Cover[] cover;
        float dis = div.magnitude;
        int AimAngle;
        if (Cha.type == Character.Type.Humanoid) //人形怪才有障礙物Buff
        {
            cover = Enemy.JudgeCover(div, out AimAngle);
            if (cover[0] == Tile.Cover.FullC)
            {
                return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)] - 40;
            }
            else if (cover[0] == Tile.Cover.HalfC)
            {
                if (cover[1] == Tile.Cover.FullC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)] - 40 + 20 * AimAngle / 45;
                }
                else if(cover[1] == Tile.Cover.HalfC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)] - 20;
                }
                else
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)] - 20 + 10 * AimAngle / 45;
                }
            }
            else
            {
                if(cover[0] == Tile.Cover.FullC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)] - 40 * (1 - AimAngle / 45);
                }
                else if(cover[1] == Tile.Cover.HalfC)
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)] - 20*(1 - AimAngle / 45);
                }
                else
                {
                    return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)];
                }
            }

        }
        else
        {
            return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)];
        }
        //計算方向判斷對方是有遮蔽物
    //距離影響武器命中率
    }


    public void ChangePreAttakeIdle()
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
                return;
            }
            Am.SetBool("FCover", true);
            Am.SetBool("Left", true);
            Am.SetBool("HCover", false);
            Am.SetBool("Right", false);
            TileCount = FindDirection(dir);
            PreAttakeIdle = PreAtkFullCover;
            ChangeTarget = true;
        }
        else if (cover==Tile.Cover.HalfC)
        {
            if (Am.GetBool("HCover") == true)
            {
                return;
            }
            Am.SetBool("FCover", false);
            Am.SetBool("Left", false);
            Am.SetBool("HCover", true);
            Am.SetBool("Right", false);
            TileCount = FindDirection(dir);
            PreAttakeIdle = PreAtkHalfCover;
            ChangeTarget = true;
        }
        else
        {
            Am.SetBool("FCover", false);
            Am.SetBool("Left", false);
            Am.SetBool("HCover", false);
            Am.SetBool("Right", false);
            TileCount = FindDirection(dir);
            PreAttakeIdle = PreAtkNoCover;
            ChangeTarget = true;
        }
    }



    public void ChaChangeTarget(AI target)//切換攻擊目標
    {
        PreAttack = true;
        Target = target;
       

        TargetDir = Target.transform.position - transform.position;
        if (Am.GetBool("FCover"))
        {
            Vector3 LoR = Vector3.Cross(Direction(TileCount), TargetDir);
            if (LoR.y > 0)
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
                if (LoR.y > 0)
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
            if (LoR.y > 0)
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
        if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.97f)
        {
            transform.forward = TargetDir;
            ChangeTarget = false;
            Am.SetBool("Right", false);
            Am.SetBool("Left", false);
            Am.SetBool("Turn", false);
            Am.SetBool("Aim", true);
        }

        if (stateinfo.IsName("RightTurn") || stateinfo.IsName("LeftTurn"))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.02f);
        }

    }

    public Action PreAttakeIdle;
    public void PreAtkNoCover()
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
                Attack = false;
                Am.SetBool("Fire", true);
                RemoveVisitedTiles();
                StartCoroutine(FireWait());
            }

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
                NPCPreaera = true;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
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
                if (stateinfo.normalizedTime > 1.0f && stateinfo.IsName("Aim"))
                {
                    AP = 0;
                    AttakeableList.Clear();
                    UI.LRDestory();
                    Attack = false;
                    Am.SetBool("Fire", true);
                    RemoveVisitedTiles();
                    StartCoroutine(FireWait());
                }

            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
        }
        else if (ChangeTarget)
        {
            stateinfo = Am.GetCurrentAnimatorStateInfo(0);

            float FoB = Vector3.Dot(transform.forward, TargetDir.normalized);
            if (FoB > 1 / Mathf.Sqrt(2) - 0.01f&& stateinfo.normalizedTime >= 1.0f)
            {
                Am.SetBool("Left", false);
                Am.SetBool("Right", false);
                Am.SetBool("Turn", false);
                ChangeTarget = false;
                ChangePreAttakeIdle(TargetDir);
                NPCPreaera = true;
            }
            else if(stateinfo.normalizedTime>=1.0f)
            {//todo
                Am.SetBool("Turn", false);
                Am.SetBool("Turn", true);
            }

        }
    }
    public void PreAtkFullCover()
    {
        if (Attack)
        {
            if (stateinfo.IsName("Aim"))
            {
                TargetDir = AttackPoint - AttackPosition;
                TargetDir.y = 0;
                if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.98f)
                {
                    transform.forward = TargetDir;
                    AP = 0;
                    AttakeableList.Clear();
                    UI.LRDestory();
                    Am.SetBool("Fire", true);
                    RemoveVisitedTiles();
                    StartCoroutine(FW());
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.05f);
            }
            else if (Am.GetBool("Turn"))
            {
                if (stateinfo.normalizedTime > 0.9f)
                {
                    Am.SetBool("Aim", true);
                    Am.SetBool("Turn", false);
                }
            }
            else
            {
                if ((transform.position - AttackPosition).magnitude < 0.1f)
                {
                    transform.position = AttackPosition;
                    Am.SetBool("Turn", true);
                    Am.SetBool("Run", false);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, AttackPosition, 0.08f);
                    //transform.position += transform.forward * MoveSpeed * Time.deltaTime*0.1f;
                }
            }
        }
        else if (ChangeTarget)
        {
            Vector3 LoR = Vector3.Cross(Direction(TileCount), TargetDir.normalized);
            if (LoR.y >= 0)
            {
                Am.SetBool("Right", true);
                Am.SetBool("Left", false);
                ChangeTarget = false;
                ChangePreAttakeIdle(TargetDir);
                NPCPreaera = true;
            }
            else
            {
                Am.SetBool("Right", false);
                Am.SetBool("Left", true);
                ChangeTarget = false;
                ChangePreAttakeIdle(TargetDir);
                NPCPreaera = true;
            }
        }
    }
    public void PreAtkFCoverAfterAttack()
    {
        if (stateinfo.IsName("BackToCover"))
        {
            if ((transform.position - AttackPosition).magnitude < 0.1f)
            {
                transform.position = CurrentTile.transform.position + Vector3.up * ChaHeight;
                Am.SetBool("Back", false);
                transform.forward = Direction(TileCount);
                PreAttack = false;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, AttackPosition, 0.08f);
                //transform.position += -transform.forward * MoveSpeed * Time.deltaTime*0.2f;
            }
        }
    }





    public void Fire(LinkedListNode<(AI target, int location, int aim)> attacklist)
    {
        int i = 100;//Random.Range(0, 100);
        if (attacklist.Value.aim < i)//Miss
        {
            Miss = true;
            RaycastHit RH;
                Vector3 ShotPoint = CurrentTile.transform.position + new Vector3(0, 1.34f, 0) + Direction(attacklist.Value.location);
            while (true)
            {
                if (Physics.Raycast(ShotPoint, (Target.transform.position
                    + new Vector3(Random.Range(-0.67f, 0.67f), Random.Range(-0.67f, 0.67f), Random.Range(-0.67f, 0.67f))) - ShotPoint,out RH))
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
            if(attacklist.Value.location == -1)
            {
                Am.SetBool("Aim", true);
                AttackPosition = CurrentTile.transform.position;
                FW = FullCoverFireWait2;
            }
            else
            {
                Am.SetBool("Run", true);
                Vector3 dir = Direction(attacklist.Value.location);
                transform.forward = dir;
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
        if (!Miss)
        {
            TargetDir = AttackPoint - FirePoint.position;
        }
        else
        {
            TargetDir = Target.BeAttakePoint.position - FirePoint.position;
        }
        B.transform.forward = TargetDir;
        PreAttack = false;
    }

    public delegate IEnumerator FWait();
    public FWait FW;
    public IEnumerator FireWait()//攻擊後緩衝時間給下回合
    {
        Attack = false;
        yield return new WaitUntil(() => PreAttack == false);
        yield return new WaitForSeconds(2f);
        PreAttack = false;
        Turn = false;
        ResetBool();
    }
    public IEnumerator FullCoverFireWait()
    {
   
        yield return new WaitUntil(() => stateinfo.normalizedTime >= 1.0f);
        PreAttakeIdle = PreAtkFCoverAfterAttack;

        Am.SetBool("Back", true);
        yield return new WaitUntil(() => stateinfo.IsName("BackToCover"));

        Vector3 dir = AttackPosition - CurrentTile.transform.position;
        dir.y = 0;
        transform.forward = dir;
        AttackPosition -= Direction(FindDirection(dir)) * 0.67f;
        yield return new WaitUntil(() => PreAttack == false);
        Turn = false;
        ResetBool();
    }
    public IEnumerator FullCoverFireWait2()
    {
        yield return new WaitUntil(() => PreAttack == false);
        yield return new WaitForSeconds(2f);
        transform.forward = Direction(TileCount);
        Turn = false;
        ResetBool();
    }

    public void Reload()
    {
        Gun.bullet = Gun.MaxBullet;
        //Am.set
        if (Acting2 != null)
        {
            DoActing = Acting2;
            Acting2 = null;
        }
    }
    
    public void BeDamaged(int damage)
    {
        Cha.HP -= damage;//todo UI?


    }
    private void AIDeath()
    {
        Am.SetBool("Death", true);
        Death = true;
        RoundSysytem.GetInstance().DeathKick(this);
        UI.DeathKick(this);
        CurrentTile.walkable = true;
        Destroy(Cha);
        Destroy(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")//子彈打到才做動作
        {
            if (Cha.HP <= 0)
            {
                AIDeath();
            }
            else
            {
                Am.Play("Hurt");
                Am.SetBool("HCover", false);
                Am.SetBool("FCover", false);
                Am.SetBool("Left", false);
                Am.SetBool("Right", false);
            }
        }
    }







    /////AI

    public void FindSelectableTiles()
    {
        if (VisitedTiles.Count > 0)
            return;
        //BFS 寬度優先使用Queue
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

    public void Move2()
    {
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
            float CalAngle = transform.rotation.eulerAngles.y % 90;//移動完轉角度 確保人物角度不會歪掉
            if (CalAngle > 5 || CalAngle < -5)
            {
                if (Vector3.Cross(Ediv, Heading).y < 0)
                {
                    transform.Rotate(0, CalAngle, 0);
                }
                else
                    transform.Rotate(0, -CalAngle, 0);
            }
            //RemoveVisitedTiles();//重置Tile狀態
            //CurrentTile.Reset();
            //CurrentTile.walkable = true;
            //CurrentTile = TargetTile;
            CurrentTile.walkable = false;
            Moving = false;
            if (Acting2 != null)
            {
                DoActing = Acting2;
            }
            else
            {
                NPCPreaera = false;
                Turn = false;
            }

        }
    }


    


    protected Tile BestT;
    public Action DoActing;
    protected Action Acting;
    protected Action Acting2;
    protected (AI, int, int) AttakeTarget;
    protected int BestPoint;
    public bool NPCPreaera = false;

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
                Point += 4;
            }
            else if (T.AdjCoverList[FindDirection(Edir)] == Tile.Cover.HalfC)
            {
                Point += 2;
            }
            if (MinDis > Edir.magnitude)
            {
                MinDis = Edir.magnitude;
            }
        }
        Point += 5 / ((int)MinDis+1);
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
                        Sec.Item1 = PreFire;
                        Sec.point += aim.Item3 / 10;
                    }
                }
                else
                {
                    Reload = true;
                    Sec.point += 4;
                }
            }
            else
            {
                if (Gun.bullet == 0)
                {
                    //reload 4分
                    Sec.point += 4;
                    Reload = true;

                }
                aim = AttakeableDetect(T);
                if (aim.Item1 != null)
                {
                    Sec.point += aim.Item3 / 10;
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
                if (i % 2 == 0)
                {
                    RTDiv.z = 0;
                }
                else
                {
                    RTDiv.x = 0;
                }
                int j = FindDirection(RTDiv);
                Target += (Direction(i) + Direction(j)) * 0.3f;
            }
            Vector3 TDiv = Target - Location;
            i = FindDirection(TDiv);
            if (T.AdjCoverList[i] == Tile.Cover.FullC) //如攻擊方有障礙物，則站出去瞄準。
            {
                if (i % 2 == 0)
                {
                    TDiv.z = 0;
                }
                else
                {
                    TDiv.x = 0;
                }
                i = FindDirection(TDiv);
                if (T.AdjCoverList[i] == Tile.Cover.None && Physics.CheckBox(T.transform.position + Direction(i) * 0.67f
                    , new Vector3(0.1f, 0, 0), Quaternion.identity, 1 << 8))
                {//判斷旁邊可以站
                    TDiv = Target - Location;
                    if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), TDiv, TDiv.magnitude, 1 << 9))
                    {//確保路徑上沒有障礙物
                        int j = CalculateAim(Enemy.CurrentTile, T);
                        if (j > BestAim.Item3)
                        {
                            BestAim = (Enemy, i, j);
                        }
                    }
                }
            }
            else if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
            {//確保路徑上沒有障礙物
                int j = CalculateAim(Enemy.CurrentTile, T);
                if (j > BestAim.Item3)
                {
                    BestAim = (Enemy, -1, j);
                }
            }
        }
        return BestAim;
    }

    protected void PreFire()
    {
        ChaChangeTarget(AttakeTarget.Item1);
        NPCPreaera = false;
        DoActing = FireCheck;
    }
    public void FireCheck()
    {
        NPCPreaera = false;
        Attack = true;
        DoActing = null;
    }

    public void Fire()
    {
        Debug.Log("fire");
        AP = 0;
        Am.SetBool("Fire", true);
        AttakeTarget.Item1.BeDamaged(Gun.Damage[0]);
        RemoveVisitedTiles();
        StartCoroutine(FireWait());
        PreAttack = false;
        NPCPreaera = false;
    }

    protected void PreMove()
    {
        MoveToTile(BestT);
        Am.SetBool("Run", true);
        Am.SetBool("FCover", false);
        Am.SetBool("HCover", false);
        Idle = NoCover;
        Moving = true;
        RemoveVisitedTiles();//重置Tile狀態
        CurrentTile.walkable = true;
        CurrentTile = BestT;
        //CurrentTile.walkable = false; 
        DoActing = null;

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
            ChangePreAttakeIdle();
        }//else if (Acting2==reload)

        NPCPreaera = true;
    }










}

