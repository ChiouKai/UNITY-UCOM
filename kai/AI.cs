using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    protected Animator Am;
    protected AnimatorStateInfo stateinfo;
    public Transform enemy;
    protected delegate IEnumerator move();
    protected private move MV = null;
    protected private move action = null;
    protected bool Acting = false;
    protected Character Cha;
    protected PlayerMove PM;
    public int AP = 0;
    protected Vector3 Ediv;

    //動畫
    protected void Idle()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (MV == null && stateinfo.IsName("Idle"))
        {
            Ediv = (enemy.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, Ediv);
            if (FoB > 1 / Mathf.Sqrt(2) - 0.1f)
            {
                ;
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
    public virtual IEnumerator  LeftTurn()
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

    //移動

    List<Tile> SelectableTiles = new List<Tile>();
    Stack<Tile> Path = new Stack<Tile>();      //tile的資料設定為Stack(後進先出)
    public Tile CurrentTile;      //玩家目前站的tile
    public Tile TargetTile;
    public bool Turn = false;
    public bool Moving = false;
    public float MoveSpeed = 4;
    protected float ChaHeight;
    public bool End = false;
    Vector3 Heading;
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

    public void FindSelectableTiles()
    {
        if (SelectableTiles.Count > 0)
            return;
        //BFS 寬度優先使用Queue
        Queue<Tile> Process1 = new Queue<Tile>();
        Process1.Enqueue(CurrentTile);
        CurrentTile.visited = true;

        while (Process1.Count > 0)
        {
            Tile T = Process1.Dequeue();
            foreach (Tile tile in T.AdjacencyList)
            {
                if (!tile.visited && tile.walkable)
                {
                    float TDis = (tile.gameObject.transform.position - T.gameObject.transform.position).magnitude;
                    tile.distance = TDis + T.distance;
                    if (tile.distance < Cha.Mobility)
                    {
                        tile.SelectableChange1();
                    }
                    else if (tile.distance < Cha.Mobility*AP)
                    {
                        tile.SelectableChange2();
                    }
                    else
                        continue;
                    SelectableTiles.Add(tile);
                    tile.parent = T;  //visited過的就被設為 parent
                    tile.visited = true;  //so we won't come back and process the visited tile. the visited will only be process onced.
                                          //距離成為parent tile + 1，如此每次往前走一格，就遠離 start tile
                    Process1.Enqueue(tile);
                }
            }

        }
        
    }
    public void FindSelectableTiles2()
    {
        if (SelectableTiles.Count > 0)
            return;
        //BFS 寬度優先使用Queue
        Queue<Tile> Process1 = new Queue<Tile>();
        Process1.Enqueue(CurrentTile);
        CurrentTile.visited = true;

        while (Process1.Count > 0)
        {
            Tile T = Process1.Dequeue();
            foreach (Tile tile in T.AdjacencyList)
            {
                if (!tile.visited && tile.walkable)
                {
                    float TDis = (tile.gameObject.transform.position - T.gameObject.transform.position).magnitude;
                    tile.distance = TDis + T.distance;
                    if (tile.distance < Cha.Mobility)
                    {
                        tile.SelectableChange2();
                    }
                    else
                        continue;
                    SelectableTiles.Add(tile);
                    tile.parent = T;  //visited過的就被設為 parent
                    tile.visited = true;  //so we won't come back and process the visited tile. the visited will only be process onced.
                                          //距離成為parent tile + 1，如此每次往前走一格，就遠離 start tile
                    Process1.Enqueue(tile);
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
                    TargetTile = T;
                    MoveToTile(T);

                }
            }

        }
    }
    public void MoveToTile(Tile tile)
    {
        Path.Clear();
        tile.TargetChange();
        Moving = true;

        Tile next = tile;
        while (next.parent != null)
        {
            Path.Push(next);
            next = next.parent;
        }
    }
    public void Move()
    {
        //if path.Count > 0, move, or remove selectable tiles, disable moving and end the turn. 

        if (Path.Count > 0)
        {
            Tile T = Path.Peek();
            Vector3 target = T.transform.position;
            target.y += ChaHeight;
            //Calculate the unit's position on top of the target tile
            //or the unit moves under/in the tiles
            //"t.GetComponent<Collider>().bounds.extents.y" is for ...?
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
            if (CalAngle > 10 || CalAngle < -10)
            {
                if (Vector3.Cross(Ediv, Heading).y < 0)
                {
                    transform.Rotate(0, 45, 0);
                }
                else
                    transform.Rotate(0, -45, 0);
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
    protected void RemoveSelectableTiles()
    {

        foreach (Tile tile in SelectableTiles)
        {
            tile.Reset();
        }

        SelectableTiles.Clear();
    }
}

