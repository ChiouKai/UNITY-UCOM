using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Bar : MonoBehaviour
{
    public Transform followedTarget;      //血條跟隨目標座標    

    private HP_Bar bar;
    public Image[] LiHP_Tiles;
    public Sprite full_HP_Tile;
    public Sprite empty_HP_Tile;
    public int HP;
    public int MaxHP;    //NPC血量
   
    void Start()
    {
        HP_Bar bar = GetComponent<HP_Bar>();
        Sprite _full_HP_Tile = gameObject.GetComponent<Sprite>();
        Sprite _empty_HP_Tile = gameObject.GetComponent<Sprite>();
              
    }

    public void UpdateHP_Bar()
    {
        Canvas cvs = UIMain.m_Instance.m_Canvas;
        Vector3 vPos = followedTarget.position + Vector3.up * 1.5f;
        Vector3 vScreenPos = Camera.main.WorldToScreenPoint(vPos);
        transform.position = vScreenPos;
    }

    void Update()
      {
        if (HP > MaxHP) { HP = MaxHP; }
        int i;
        for (i = 1; i < LiHP_Tiles.Length; i++)
        {
            if (i < HP)
            {
                LiHP_Tiles[i].sprite = full_HP_Tile;
            }
            else
            {
                LiHP_Tiles[i].sprite = empty_HP_Tile;
            }

            if (i < MaxHP)
            {
                LiHP_Tiles[i].enabled = true;
            }
            else
            {
                LiHP_Tiles[i].enabled = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.X)) { HP -= 2; }
        if (Input.GetKeyDown(KeyCode.C)) { HP += 2; }
        if (HP <= 0) { gameObject.SetActive(false); }
        //if (HP <= 0) { Destroy(Main.npcPrefab); } 
      }
    
}

    
