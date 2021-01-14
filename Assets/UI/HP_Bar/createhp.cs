using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*  此程式需放在血條的預置物中
 
    . 將血條獨立給個體 目前改動一個全部都會連動
    . 按o 被攻擊時扣血 ->攻擊時先不顯示血條 顯示攻擊數字後再把血條顯示
    . 按p 治療時加血 */
public class createhp : MonoBehaviour
{
    public Transform followedTarget;      //血條跟隨目標座標    
    public int MaxHP;
    public int HP;
    public Image hp_tiles;
    Image[] LiHP_Tiles;
    public Sprite full_HP_Tile; //該格有血量時顯示
    public Sprite empty_HP_Tile; //該格沒血時顯示
    // Start is called before the first frame update
    void Start()
    {
        LiHP_Tiles = new Image[MaxHP];
        Create_HPBAR();
        HPControl(HP);
    }
    public void UpdateHP_Bar() //放血條的位置
    {
        Vector3 vScreenPos = Camera.main.WorldToScreenPoint(followedTarget.position+new Vector3(0,2.2f,0));
        vScreenPos += Vector3.right * 5f;
        transform.position = vScreenPos;
    }
    
    public void HPControl(int valve)
    {
        HP = valve;
        if (HP > MaxHP) { HP = MaxHP; }

        for (int i = 1; i < LiHP_Tiles.Length; i++)
        {
            if (i < HP)
            {
                LiHP_Tiles[i].sprite = full_HP_Tile;
            }
            else
            {
                LiHP_Tiles[i].sprite = empty_HP_Tile;
            }
        }
        if (HP <= 0) { Destroy(gameObject); }
    }



    void Create_HPBAR()
    {
        for (int i = 0; i < LiHP_Tiles.Length; i++)
        {
            int j = i;
            j = i / 5;
            int y = 16;
            int x =  -40+ y*i+10*j; //血條位置
            LiHP_Tiles[i] =  Instantiate(hp_tiles,transform);
            LiHP_Tiles[i].transform.position = transform.position +new Vector3(x,-20,0);
            LiHP_Tiles[i].sprite = full_HP_Tile;
            //,new Vector3(x, -40, 0), Quaternion.Euler(0,0,0)
        }
    }
}
