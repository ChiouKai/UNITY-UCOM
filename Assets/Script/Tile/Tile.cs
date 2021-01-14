using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    private GameObject GLR;
    public List<Tile> AdjList=new List<Tile>();
    public List<Cover> AdjCoverList;
    //needed breath first search
    public bool visited = false;
    public Tile Parent;
    public float distance = 0;
    float Grid = 0.67f;
    public AI Cha;


    public enum Cover
    {
        None=0,
        HalfC,
        FullC,
    }



    // Start is called before the first frame update
    private void Start()
    {
        //WalkableTest();
        AdjCoverList = new List<Cover>();
        //FindNeighbors();
        UpdateCover();
    }


    public void MeleePos()
    {
        GetComponent<Renderer>().material = Resources.Load<Material>("MeleePos");
    }

    public void ChoMeleePos()
    {
        GetComponent<Renderer>().material = Resources.Load<Material>("ChoMeleePos");
    }
    public void Recover()
    {
        GetComponent<Renderer>().material = Resources.Load<Material>("Tile");
    }

    private void OnMouseEnter()
    {
        UISystem.getInstance().MouseInTile(this);
    }
    private void OnMouseExit()
    {
        UISystem.getInstance().MouseOutTile(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("En"))
        {
            walkable = false;
            //GetComponent<Renderer>().material = Resources.Load<Material>("Tile");
            //GetComponent<Renderer>().enabled = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("En"))
        {
            walkable = true;
            //GetComponent<Renderer>().enabled = true;
        }
    }

    public void Reset()
    {
        //adjacencyList.Clear();
        GetComponent<Renderer>().material = Resources.Load<Material>("Tile");
        target = false;
        selectable = false;

        visited = false;
        Parent = null;
        distance = 0;
    } 

    public void FindNeighbors()   //檢查前後左右的tile
    {
        CheckTile(Vector3.forward*2/3);
        CheckTile(-Vector3.right*2/3);
        CheckTile(-Vector3.forward*2/3);
        CheckTile(Vector3.right*2/3);
        CheckTile(new Vector3(-Grid, 0, Grid));
        CheckTile(new Vector3(-Grid, 0, -Grid));
        CheckTile(new Vector3(Grid, 0, -Grid));  
        CheckTile(new Vector3(Grid, 0, Grid));  
    }

    public void CheckTile(Vector3 direction)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, new Vector3(0.1f,0,0));
        
        foreach (Collider item in colliders) 
        {
            Tile tile = item.GetComponent<Tile>();
            AdjList.Add(tile);

        }
    }
    public void UpdateCover()
    {
        AdjCoverList.Clear();
        CoverCheck(new Vector3(0, 0.335f, 0.335f));
        CoverCheck(new Vector3(-0.335f, 0.335f, 0));
        CoverCheck(new Vector3(0, 0.335f, -0.335f));
        CoverCheck(new Vector3(0.335f, 0.335f, 0));
    }
    public void CoverCheck(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction + new Vector3(0, 0.67f, 0), 1.7f,1<<9))
            AdjCoverList.Add(Cover.FullC);
        else if (Physics.Raycast(transform.position, direction, 0.8f ,1<<9))
            AdjCoverList.Add(Cover.HalfC);
        else
            AdjCoverList.Add(Cover.None);
        
    }

    public Cover[] JudgeCover(Vector3 div,out float AimAngle)
    {
        Cover[] cover = new Cover[2];
        float angel = Vector3.Angle(Vector3.forward, div.normalized);
        float LoR = Vector3.Cross(Vector3.forward, div).y;
        if (LoR < 0)
        {
            if (Mathf.Abs(90f- angel)<1)
            {
                cover[0] = AdjCoverList[1];
                cover[1] = Cover.None;
                AimAngle = 0;
                return cover;
            }
            else 
            {
                int i = (int)angel / 45;
                int j = (int)angel / 90;
                if (i % 2 == 0)
                {
                    cover[0] = AdjCoverList[j];
                    cover[1] = AdjCoverList[j + 1];
                    AimAngle = (int)angel % 45;
                    return cover;
                }
                else
                {
                    cover[0] = AdjCoverList[j+1];
                    cover[1] = AdjCoverList[j];
                    AimAngle = 90 * (j + 1) - (int)angel;
                    return cover;
                }

            }
        }
        else if (LoR > 0)
        {
            if (Mathf.Abs(90f - angel) < 1)
            {
                cover[0] = AdjCoverList[3];
                cover[1] = Cover.None;
                AimAngle = 0;
                return cover;
            }
            else
            {
                int i = (int)angel / 45;
                int j = (int)angel / 90;
                if (i % 2 == 0)
                {
                    cover[0] = AdjCoverList[(4 - j) % 4];
                    cover[1] = AdjCoverList[3 - j];
                    AimAngle = (int)angel % 45;
                    return cover;
                }
                else
                {
                    cover[0] = AdjCoverList[3 - j];
                    cover[1] = AdjCoverList[(4 - j) % 4];
                    AimAngle = 90 * (j + 1) - (int)angel;
                    return cover;
                }
            }

        }else
        {
            if (Mathf.Abs(angel) < 1)
            {
                cover[0] = AdjCoverList[0];
                cover[1] = Cover.None;
                AimAngle = 0;
                return cover;
            }
            else
            {
                cover[0] = AdjCoverList[2];
                cover[1] = Cover.None;
                AimAngle = 0;
                return cover;
            }
        }
    }

}
