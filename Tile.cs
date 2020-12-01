using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;

    public List<Tile> AdjacencyList = new List<Tile>();
    
    //needed breath first search
    public bool visited = false;
    public Tile parent = null;
    public float distance = 0;
    float grid = 0.67f;


    // Start is called before the first frame update
    private void Start()
    {
        
        //FindNeighbors();
    }

    public void CurrentChange()
    {
        current = true;
        GetComponent<Renderer>().material.color = Color.cyan;
    }
    public void TargetChange()
    {
        target = true;
        GetComponent<Renderer>().material = Resources.Load<Material>("targetTile");
    }
    public void SelectableChange1()
    {
        selectable = true;
        GetComponent<Renderer>().material = Resources.Load<Material>("selectable1");
    }
    public void SelectableChange2()
    {
        selectable = true;
        GetComponent<Renderer>().material = Resources.Load<Material>("selectable2");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("En"))
        {
            walkable = false;
            GetComponent<Renderer>().enabled = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("En"))
        {
            walkable = true;
            GetComponent<Renderer>().enabled = true;
        }
    }


    public bool WalkableDetect()
    {
        RaycastHit hit;
        return !Physics.Raycast(transform.position, Vector3.up, out hit, 3);

    }

    public void Reset()
    {
        //adjacencyList.Clear();
        GetComponent<Renderer>().material = Resources.Load<Material>("Tile");
        current = false;
        target = false;
        selectable = false;
        walkable = true;

         visited = false;
        parent = null;
        distance = 0;
    } 

    public void FindNeighbors()   //檢查前後左右的tile
    {
        CheckTile(Vector3.forward*2/3);
        CheckTile(-Vector3.forward*2/3);
        CheckTile(-Vector3.right*2/3);
        CheckTile(Vector3.right*2/3);
        CheckTile(new Vector3(grid, 0, grid));
        CheckTile(new Vector3(-grid, 0, grid));
        CheckTile(new Vector3(grid, 0, -grid));
        CheckTile(new Vector3(-grid, 0, -grid));
    }

    public void CheckTile(Vector3 direction)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, new Vector3(0.1f,0,0));
        
        foreach (Collider item in colliders) 
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
                AdjacencyList.Add(tile);  
        }
    }
}
