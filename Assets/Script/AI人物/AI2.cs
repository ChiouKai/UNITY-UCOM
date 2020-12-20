using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI2 : MonoBehaviour
{
    protected Animator Am;
    protected AnimatorStateInfo stateinfo;
    public Transform enemy;
    protected delegate IEnumerator move();
    protected private move MV = null;
    protected Action Idel = null;
    protected bool Acting = false;
    public Character Cha;
    public int AP = 0;
    protected Vector3 Ediv;
    protected int EnemyLayer;
    protected int TileCount;
    

    //動畫
    protected void NoCover()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (MV == null && stateinfo.IsName("Idle"))
        {
            Ediv = (enemy.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, Ediv);
            TileCount = FindDirection(transform.forward);
            if (FoB > 1 / Mathf.Sqrt(2) - 0.1f)
            {
                if (CurrentTile.AdjCoverList[TileCount] == Tile.Cover.None)
                {
                    if (Vector3.Dot(transform.forward, Ediv)> 0.99f)
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
                        Idel = HalfCover;
                        TileCount = i;
                    }
                    else
                    {
                        Idel = FullCover;
                        Am.SetBool("FCover", true);
                        TileCount = i;
                    }

                }
                else if (CurrentTile.AdjCoverList[TileCount] == Tile.Cover.HalfC)
                {
                    Am.SetBool("HCover", true);
                    Idel = HalfCover;
                }
                else
                {
                    //
                }
            }
            else if (FoB < -1 / Mathf.Sqrt(2))
            {
                Am.SetBool("Back", true);
                MV = BackTurn;
                TileCount = (TileCount + 2) % 4;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                if (LoR.y > 0.0f)
                {
                    Am.SetBool("Right", true);
                    MV = RightTurn;
                    TileCount = (TileCount + 3) % 4;
                }
                else
                {
                    Am.SetBool("Left", true);
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
                        if (Vector3.Dot(transform.forward, Ediv) > 0.99f)
                        {
                            Idel = NoCover;
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
                            Idel = NoCover;
                            Am.SetBool("HCover", false);
                            TileCount = EDir;
                        }
                        else if (CurrentTile.AdjCoverList[i] == Tile.Cover.HalfC)
                        {
                            TileCount = i;
                        }
                        else
                        {
                            Idel = FullCover;
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
                MV = BackTurn;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, Ediv);
                if (LoR.y > 0.0f)
                {
                    Am.SetBool("Right", true);
                    MV = RightTurn;
                }
                else
                {
                    Am.SetBool("Left", true);
                    MV = LeftTurn;
                }

            }
        }
    }

    protected void FullCover()
    {

    }


    protected void IdelChange(Tile.Cover cover)
    {
        if (cover == Tile.Cover.None)
        {
            Am.SetBool("HCover", false);
            Am.SetBool("FCover", false);
            Idel = NoCover;
        }else if (cover == Tile.Cover.HalfC)
        {
            Am.SetBool("HCover", true);
            Am.SetBool("FCover", false);
            Idel = HalfCover;
        }
        else
        {
            Am.SetBool("HCover", false);
            Am.SetBool("FCover", true);
            Idel = FullCover;
        }

    }



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








    //移動

    public List<Tile> SelectableTiles = new List<Tile>();
    Stack<(Tile, MoveWay)> Path = new Stack<(Tile, MoveWay)>();      //tile的資料設定為Stack(後進先出)
    public Tile CurrentTile;      //玩家目前站的tile
    public Tile TargetTile;
    public bool Turn = false;
    public bool Moving = false;
    public bool Running = false;
    public float MoveSpeed = 4;
    protected float ChaHeight;
    public bool End = false;
    Vector3 Heading;
    public UISystem UI;
    enum MoveWay
    {
        Run=0,
        Across,
        ClimbUp,
        Ladder,
        ClimbDown,
        Jump,
        AcrossJump
           
    }

    //public void GetCurrentTile()  //for either player or NPC to use as the starting tile for path finding.
    //{
    //    CurrentTile = GetTargetTile(gameObject);

    //    CurrentTile.CurrentChange();
    //}

    //public Tile GetTargetTile(GameObject target)
    //{
    //    RaycastHit hit;
    //    Tile tile = null;

    //    if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 3, LayerMask.GetMask("tile")))
    //    {
    //        tile = hit.collider.GetComponent<Tile>();
    //    }
    //    return tile;
    //}
    //bool ObliqueCheck(Tile T,int i)
    //{
    //    if (!T.AdjList[i-4].walkable && !T.AdjList[(i - 3)%4].walkable&&
    //        T.AdjCoverList[i - 4] != Tile.Gap.None && T.AdjCoverList[(i - 3) % 4] != Tile.Gap.None)
    //    {
    //        return true;
    //    }
    //    else return false;
//}

    public void MoveRange()
    {
        Queue<Tile> Process = new Queue<Tile>();
        Tcolor = SelectColor1;
        Process = FindSelectableTiles(Process);


        Tcolor = SelectColor2;

    }
    public Action<Tile> Tcolor;

    protected void SelectColor1(Tile T)
    {
        T.SelectableChange1();
    }
    protected void SelectColor2(Tile T)
    {
        T.SelectableChange2();
    }

    void DrawLine(Queue<Tile> Process)
    {
        for(int i = 0; i < Process.Count; ++i)
        {
            Tile T = Process.Dequeue();
            foreach(Tile AdjT in T.AdjList)
            {
                Vector3 div = T.transform.position - AdjT.transform.position;
                div.y = 0;
                if (div.magnitude > 0.9f)
                {
                    break;
                }
                if (AdjT.visited == true)
                {

                }
            }
        }
    }



    public Queue<Tile> FindSelectableTiles(Queue<Tile> Process)
    {
        if (SelectableTiles.Count > 0||AP==0)
            return null;
        //BFS 寬度優先使用Queue
        Queue<Tile> Process2 = new Queue<Tile>();
        Process.Enqueue(CurrentTile);
        CurrentTile.visited = true;

        while (Process.Count > 0)
        {
            Tile T = Process.Dequeue();
            foreach(Tile adjT in T.AdjList)
            {
                if (!adjT.visited && adjT.walkable)
                {
                    if (!adjT.walkable)
                    {
                        Process2.Enqueue(adjT);
                        continue;
                    }
                    //Vector3 vdiv = adjT.transform.position - T.transform.position + new Vector3(0, 1.34f, 0);
                    Vector3 vdiv = adjT.transform.position - T.transform.position;
                    vdiv.y = 0;
                    float TDis = vdiv.magnitude;
                    if (TDis > 0.9f)
                    {
                        if (Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, LayerMask.GetMask("Environment"))
                        || Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, EnemyLayer))
                        {
                            continue;
                        }
                    }
                    adjT.distance = TDis + T.distance;
                    if (adjT.distance < Cha.Mobility)
                    {
                        Tcolor(adjT);
                    }
                    else
                    {
                        Process2.Enqueue(adjT);
                        continue;
                    }
                    SelectableTiles.Add(adjT);
                    adjT.Parent = T;  //visited過的就被設為 parent
                    adjT.visited = true;  //so we won't come back and process the visited tile. the visited will only be process onced.
                                          //距離成為parent tile + 1，如此每次往前走一格，就遠離 start tile
                    Process.Enqueue(adjT);

                }
            }
        }
        return Process2;

    }
    public void FindSelectableTiles2()
    {
        if (SelectableTiles.Count > 0)
            return;
        //BFS 寬度優先使用Queue
        Queue<Tile> Process = new Queue<Tile>();
        Process.Enqueue(CurrentTile);
        CurrentTile.visited = true;

        while (Process.Count > 0)
        {
            Tile T = Process.Dequeue();
            foreach (Tile adjT in T.AdjList)
            {
                if (!adjT.visited && adjT.walkable)
                {
                    //Vector3 vdiv = adjT.transform.position - T.transform.position + new Vector3(0, 1.34f, 0);
                    Vector3 vdiv = adjT.transform.position - T.transform.position;
                    vdiv.y = 0;
                    float TDis = vdiv.magnitude;
                    if (TDis > 0.9f)
                    {
                        if (Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, LayerMask.GetMask("Environment"))
                        || Physics.CheckBox(T.transform.position + (vdiv / 2) + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, EnemyLayer))
                        {
                            continue;
                        }
                    }
                    adjT.distance = TDis + T.distance;
                    if (adjT.distance < Cha.Mobility)
                    {
                        adjT.SelectableChange2();
                    }
                    else
                        continue;
                    SelectableTiles.Add(adjT);
                    adjT.Parent = T;  //visited過的就被設為 parent
                    adjT.visited = true;  //so we won't come back and process the visited tile. the visited will only be process onced.
                                          //距離成為parent tile + 1，如此每次往前走一格，就遠離 start tile
                    Process.Enqueue(adjT);

                }
            }
        }
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
                    Idel = NoCover;
                    TargetTile = T;
                    MoveToTile(T);
                }
            }
        }
    }

    public void MoveToTile(Tile T)
    {
        Path.Clear();
        T.TargetChange();

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
                Current = Current.Parent;
                while (Current.Parent != null)
                {
                    
                    vdiv = Current.Parent.transform.position - Current.transform.position;
                    Height = vdiv.y;
                    if (Mathf.Abs(Height) > 0.6f )
                    {
                        break;
                    }
                    Pdiv = Current.Parent.transform.position - Previous.transform.position;
                    if (Physics.BoxCast(Previous.transform.position + new Vector3(0, 0.67f, 0), new Vector3(0.3f, 0.3f, 0.3f), Pdiv
                            , Quaternion.identity, Pdiv.magnitude))
                    {
                        Path.Push((Current, MW));
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
        }
        Moving = true;
    }

    public void Move()
    {
        //if path.Count > 0, move, or remove selectable tiles, disable moving and end the turn. 

        if (Path.Count > 0)
        {
            (Tile T,MoveWay M) = Path.Peek();
            Vector3 target = T.transform.position;
            target.y += ChaHeight;
            //switch (M)
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
            float CalAngle = transform.rotation.eulerAngles.y % 90;
            if (CalAngle > 5 || CalAngle < -5)
            {
                if (Vector3.Cross(Ediv, Heading).y < 0)
                {
                    transform.Rotate(0, CalAngle, 0);
                }
                else
                    transform.Rotate(0, -CalAngle, 0);
            }
            RemoveSelectableTiles();
            CurrentTile.Reset();
            CurrentTile = TargetTile;
            TargetTile = null;
            CurrentTile.walkable = false;
            Moving = false;

            if (AP != 0)
                FindSelectableTiles2();
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
    protected int FindDirection(Vector3 div)
    {
        if (Mathf.Abs(div.x) > Mathf.Abs(div.z))
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


    public void RemoveSelectableTiles()
    {

        foreach (Tile tile in SelectableTiles)
        {
            tile.Reset();
        }

        SelectableTiles.Clear();
    }



    //攻擊
    List<AI> Enemies;
    public List<(AI, Vector3 ,int )> AttakeAbleList;//角色,射擊位置,命中率;
    public Weapon Gun;
    public bool Attack = false;
    bool Death = false;

    public void GetTargets(List<AI> enemy)
    {
        Enemies = enemy;
    } 
    protected void AttakeableDetect()
    {
        if (AttakeAbleList.Count>0 || AP == 0)
        {
            return;
        }
        foreach (AI Enemy in Enemies)//改queue ?
        {

            Vector3 Location = CurrentTile.transform.position;
            Vector3 Target = Enemy.CurrentTile.transform.position;

            if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
            {
                AttakeAbleList.Add((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
            }
            else
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (CurrentTile.AdjCoverList[i] == Tile.Cover.FullC)
                    {
                        if (i % 2 == 0)
                        {
                            Location += Vector3.right * 0.335f;
                            if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                            {

                                AttakeAbleList.Add((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                            }
                            else
                            {
                                Location += Vector3.left * 0.67f;
                                if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                                {
                                    AttakeAbleList.Add((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                                }
                            }
                        }
                        else
                        {
                            Location += Vector3.forward * 0.335f;
                            if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                            {
                                AttakeAbleList.Add((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                            }
                            else
                            {
                                Location += Vector3.back * 0.67f;
                                if (!Physics.Raycast(Location + new Vector3(0, 1.34f, 0), Target - Location, (Target - Location).magnitude, 1 << 9))
                                {
                                    AttakeAbleList.Add((Enemy, Location, CalculateAim(Enemy.CurrentTile, CurrentTile)));
                                }
                            }
                        }
                    }
                    else
                        continue;
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
                    return Cha.BasicAim + Gun.atkRange[Mathf.FloorToInt(dis)] - 20 * (1 - AimAngle / 45);
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

    public void Fire((AI target, Vector3 location, int aim) attacklist)
    {
        AP=0;
        AttakeAbleList.Clear();
        attacklist.target.BeDamaged(Gun.Damage[0]);
        
        Am.SetBool("Fire", true);
        RemoveSelectableTiles();
        StartCoroutine(FireWait());
        Attack = false;
    }
    public IEnumerator FireWait()
    {
        yield return new WaitForSeconds(1.0f);
        Turn = false;
        ResetBool();
    }
    
    public void BeDamaged(int damage)
    {
        Cha.HP -= damage;

        if (Cha.HP <= 0)
        {
            Am.SetBool("Death", true);
            Death = true;
            //RoundSysytem.GetInstance().DeathKick(this);
        }
        else
        {
            Am.Play("preHurt");
        }
    }

        
    
}

