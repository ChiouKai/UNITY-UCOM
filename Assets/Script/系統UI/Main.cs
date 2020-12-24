using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Object npcPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit rh;
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out rh, 1000f, 1 << LayerMask.NameToLayer("Tile")))
            {
                GameObject go = GameObject.Instantiate(npcPrefab) as GameObject;
                go.transform.position = rh.point;
                UIMain.m_Instance.CreateHP_Bar(go.transform);
            }            
        }               
    }
    public void TakeDamage(int damage)
    {

    }
}
