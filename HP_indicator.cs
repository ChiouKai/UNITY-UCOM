using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_indicator : MonoBehaviour
{ 
    public int HP;  //目前HP
    public int numOfHP;  //HP總量

    public Image[] HP_tiles; 
    public Sprite full_HP_Tile;
    public Sprite empty_HP_Tile;

    private void Start()
    {
        Image[] _HP_tiles = gameObject.GetComponent<Image[]>();
        Sprite _full_HP_Tile = gameObject.GetComponent<Sprite>();
        Sprite _empty_HP_Tile = gameObject.GetComponent<Sprite>();

    }

    public void takeDamage(int hp)
    {

    }


    // Update is called once per frame
    void Update()
    {   if (HP > numOfHP) { HP = numOfHP; }
    
        for (int i = 0; i < HP_tiles.Length; i++)
        {
            if (i < HP)
            {
                HP_tiles[i].sprite = full_HP_Tile;
            }
            else
            {
                HP_tiles[i].sprite = empty_HP_Tile;
            }
            if (i< numOfHP) {
                HP_tiles[i].enabled = true;
            } else  {
                HP_tiles[i].enabled = false;
            }
        }

    }
}
