using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Animator Am;
    protected AnimatorStateInfo stateinfo;
    public Transform enemy;
    public delegate IEnumerator move();
    public move MV = null;
    protected Action Idle = null;
    protected bool Acting = false;
    public Character Cha;
    public int AP = 0;
    protected Vector3 Ediv;
    protected int EnemyLayer;
    protected int TileCount;
    

    //動畫
    protected void NoCover()//沒Cover的狀態
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (MV == null && stateinfo.IsName("Idle"))
        {
            Ediv = (enemy.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, Ediv);
            TileCount = FindDirection(transform.forward);
            if (FoB > 1 / Mathf.Sqrt(2) - 0.1f)   //判斷前後左右
            {
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
                        if (LoR.y > 0)
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
                    //
                }
            }
            else if (FoB < -1 / Mathf.Sqrt(2))
            {
                Am.SetBool("Back", true);
                Am.SetTrigger("Turn");
                MV = BackTurn;
                TileCount = (TileCount + 2) % 4;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                if (LoR.y > 0.0f)
                {
                    Am.SetBool("Right", true);
                    Am.SetTrigger("Turn");
                    MV = RightTurn;
                    TileCount = (TileCount + 3) % 4;
                }
                else
                {
                    Am.SetBool("Left", true);
                    Am.SetTrigger("Turn");
                    MV = LeftTurn;
                    TileCount = (TileCount + 1) % 4;
                }

            }

        }
    }


    protected void HalfCover()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (MV == null && stateinfo.IsName("Idle"))//
        {
            Ediv = (enemy.position - transform.position).normalized;
            int EDir = FindDirection(Ediv);
            float CFoB = Vector3.Dot(Direction(TileCount), Ediv);

            float FoB = Vector3.Dot(transform.forward, Ediv);

            if (FoB > 1 / Mathf.Sqrt(2) - 0.05f)
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
            else if (FoB < -1 / Mathf.Sqrt(2))
            {
                Am.SetBool("Back", true);
                Am.SetTrigger("Turn");
                MV = BackTurn;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                if (LoR.y > 0.0f)
                {
                    Am.SetBool("Right", true);
                    Am.SetTrigger("Turn");
                    MV = RightTurn;
                }
                else
                {
                    Am.SetBool("Left", true);
                    Am.SetTrigger("Turn");
                    MV = LeftTurn;
                }

            }
        }
    }

    protected void FullCover()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (MV == null && stateinfo.IsName("Idle"))//
        {
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


    public IEnumerator Skip()//跳過回合
    {
        yield return new WaitForSeconds(1.5f);
        AP = 0;
        Turn = false;
        MV = null;
    }





    //移動

    public List<Tile> VisitedTiles = new List<Tile>();
    public List<(Tile, MoveWay)> Path2 = new List<(Tile, MoveWay)>();
    public Stack<(Tile, MoveWay)> Path = new Stack<(Tile, MoveWay)>();      //tile的資料設定為Stack(後進先出)
    public Tile CurrentTile;      //玩家目前站的tile
    public Tile TargetTile;         //移動位置  
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
                    Am.SetBool("HCover", false);
                    Idle = NoCover;
                    //TargetTile = T;
                    Moving = true;
                    RemoveVisitedTiles();//重置Tile狀態
                    Prepera = false;
                    CurrentTile.walkable = true;
                    CurrentTile = T;
                    //TargetTile = null;
                    //CurrentTile.walkable = false;
                    AttakeAbleList.Clear();
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
            
            
            //AttakeAbleList.Clear();
            //UI.LRDestory();


            TargetTile = null;
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
    public LinkedList<(AI, Vector3 ,int )> AttakeAbleList = new LinkedList<(AI, Vector3, int)>();//角色,射擊位置,命中率;
    public Weapon Gun;
    public bool PreAttack = false;
    protected bool Death = false;
    public bool Attack;
    public bool ChangeTarget = false;
    public GameObject Target;
    protected Vector3 TargetDir;
    protected float RotationProgress = 0;
    public Transform FirePoint;
    public GameObject Bullet;

    public void GetTargets(List<AI> enemy)
    {
        Enemies = enemy;
    } 
    public void AttakeableDetect()
    {
        if (AttakeAbleList.Count>0 || AP == 0)
        {
            return;
        }
        foreach (AI Enemy in Enemies)//改queue ?
        {
            Vector3 Location = CurrentTile.transform.position;
            

            Vector3 Target = Enemy.CurrentTile.transform.position;


            Vector3 RTDiv = Location - Target;
            int i = FindDirection(RTDiv);
            if (Enemy.CurrentTile.AdjCoverList[i] == Tile.Cover.FullC)
            {
                if (i % 2 == 0)
                {
                    RTDiv.z = 0;
                }
                else
                {
                    RTDiv.x = 0;
                }
                i = FindDirection(RTDiv);
            }

            Vector3 TDiv = Target - Location;
            i = FindDirection(TDiv);
            if (CurrentTile.AdjCoverList[i] == Tile.Cover.FullC)
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
                if (CurrentTile.AdjCoverList[j] == Tile.Cover.None && Physics.CheckBox(CurrentTile.transform.position+Direction(j)*0.67f
                    , new Vector3(0.1f, 0, 0), Quaternion.identity, 1 << 8))
                {
                    TDiv = Target - Location;
                    if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), TDiv, TDiv.magnitude, 1 << 9))
                    {//確保路徑上沒有障礙物
                        AttakeAbleList.AddLast((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));//todo
                        continue;
                    }
                }
                if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                {//確保路徑上沒有障礙物
                    AttakeAbleList.AddLast((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                }
                else
            }




            if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
            {//確保路徑上沒有障礙物
                AttakeAbleList.AddLast((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
            }
            else
            {//如果有障礙物，判斷玩家有沒有在FullCover後面，如有 跨出去 判斷可不可以打敵人
                for (int i = 0; i < 4; ++i)
                {
                    if (CurrentTile.AdjCoverList[i] == Tile.Cover.FullC)
                    {
                        if (i % 2 == 0)
                        {
                            Location += Vector3.right * 0.335f;
                            if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                            {

                                AttakeAbleList.AddLast((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                            }
                            else
                            {
                                Location += Vector3.left * 0.67f;
                                if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                                {
                                    AttakeAbleList.AddLast((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                                }
                            }
                        }
                        else
                        {
                            Location += Vector3.forward * 0.335f;
                            if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                            {
                                AttakeAbleList.AddLast((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                            }
                            else
                            {
                                Location += Vector3.back * 0.67f;
                                if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                                {
                                    AttakeAbleList.AddLast((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                                }
                            }
                        }
                    }
                    else
                        continue;
                }
            }
        }
        for(int i =  0; i < AttakeAbleList.Count; ++i)
        {
            var Current = AttakeAbleList.First;
            var Previous = Current;
            Current = Current.Next;
            for (int j =0;j< AttakeAbleList.Count -1- i; ++j)
            {
                if (Previous.Value.Item3 < Current.Value.Item3)
                {
                    AttakeAbleList.Remove(Previous);
                    AttakeAbleList.AddAfter(Current, Previous);
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
        if (Cha.type == Character.Type.Humanoid)
        {
            cover = CurrentTile.JudgeCover(div, out AimAngle);
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

    public void ChaChangeTarget(AI target)
    {
        Target = target.gameObject;
        ChangeTarget = true;
        Am.SetBool("Aim", false);
        TargetDir = target.transform.position - transform.position;
        Vector3 LoR = Vector3.Cross(transform.forward, TargetDir);
        if (LoR.y > 0)
        {
            Am.SetBool("Right", true);
            Am.SetTrigger("Turn");
        }
        else
        {
            Am.SetBool("Left", true);
            Am.SetTrigger("Turn");
        }
    }
    protected void FaceTarget()
    {
        TargetDir.y = 0;
        if (Vector3.Dot(transform.forward, TargetDir.normalized) > 0.97f)
        {
            transform.forward = TargetDir;
            ChangeTarget = false;
            Am.SetBool("Right", false);
            Am.SetBool("Left", false);
            Am.SetBool("Aim", true);
        }

        if (stateinfo.IsName("RightTurn") || stateinfo.IsName("LeftTurn"))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.01f);
        }

    }


    public void Fire(LinkedListNode<(AI target, Vector3 location, int aim)> attacklist)
    {
        AP=0;
        AttakeAbleList.Clear();
        UI.LRDestory();
        Am.SetBool("Fire", true);
        attacklist.Value.target.BeDamaged(Gun.Damage[0]);
        RemoveVisitedTiles();
        StartCoroutine(FireWait());
        PreAttack = false;
    }
    public void FireBullet()
    {
        GameObject B = Instantiate(Bullet,FirePoint.transform.position,Quaternion.identity);
        
        B.transform.forward = TargetDir;
    }

    public IEnumerator FireWait()
    {
        yield return new WaitForSeconds(1.5f);
        Turn = false;
        ResetBool();
    }
    
    public void BeDamaged(int damage)
    {
        Cha.HP -= damage;

        //if (Cha.HP <= 0)
        //{
        //    AIDeath();
        //}
        //else
        //{
        //    Am.Play("preHurt");
        //}
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
        if (Cha.HP <= 0)
        {
            AIDeath();
        }
        else
        {
            Am.Play("Hurt");
        }
    }

}

