using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*  1. 將血條獨立給個體 目前改動一個全部都會連動
    2. 目前血條預設最高 6點 使用陣列判斷血量 -> 改成自動生成血條 ->先設第一個血的固定位置 在他之後固定距離生成(-23開始 每個x+8)
    3. 按o 被攻擊時扣血 ->攻擊時先不顯示血條 顯示攻擊數字後再把血條顯示
    4. 按p 治療時加血 */
public class HP_Bar : MonoBehaviour
{
    public Transform followedTarget;      //血條跟隨目標座標    

    //private HP_Bar bar;
    public Image[] LiHP_Tiles;
    public Sprite full_HP_Tile;
    public Sprite empty_HP_Tile;
    public int HP;
    public int MaxHP;    //NPC血量

    void Start()
    {

        //HP_Bar bar = GetComponent<HP_Bar>();
        //Sprite _full_HP_Tile = gameObject.GetComponent<Sprite>();
        //Sprite _empty_HP_Tile = gameObject.GetComponent<Sprite>();
              
    }

    public void UpdateHP_Bar() //放血條的位置
    {
        Vector3 vPos = followedTarget.position + Vector3.up * 1f;
        Vector3 vScreenPos = Camera.main.WorldToScreenPoint(vPos);
        transform.position = vScreenPos;
    }
}

    
