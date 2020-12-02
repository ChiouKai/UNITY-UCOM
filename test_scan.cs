/*
    先鎖定一個物件，可用滑鼠點擊別的物件時建立兩個物件之間的射線

    製作一個固定點發出雷射光 (判斷武器類別決定雷射光長度) 
    再判斷到目標點的直線距離是否遮蔽物(o)
    最後確認打到目標後是否確實命中
    命中率可以random1~100 如果有70%就射 i>30時命中
    先計算命中率 射擊後判斷有無命中 若無命中 亂數打到其他地方
    
    使用雷射光判斷是否可以看到目標 (點擊左鍵時)
    點擊後出現目標的UI 顯示命中率 子彈等參數
    (?) 如果點擊目標後再點擊其他敵人，參數必須要更改為對後面敵人的參數
    設兩函式，一按鈕
    -> 第一個函式 判斷射線是否可以看到目標(不管距離)，可以時才顯示ui 
    -> 第二個函式 判斷另一條射線 射程 傷害 命中率 子彈數 並將值傳給 UI
    按鈕 -> 決定要不要攻擊
    
    狙擊 霰彈 步槍 重機槍 小手槍  → 彈藥數量
    按鈕點擊時為true，如果狙擊有5發子彈，為true時5-1 若=0時則不能攻擊
    每個武器的命中率 射程 傷害 距離 →用雷射判斷距離、命中率
    粒子系統黏槍上，子彈噴出，如果有命中就命中，沒有的話亂數打中其他地方，若剛好打中可破壞障礙物，讓障礙物扣血
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using System;
public class test_scan : MonoBehaviour
{
    public GameObject man; //為準備攻擊的人
    public weapons_inf weapon;
    Ray mouse_ray;
    Ray AttackH_ray;
    RaycastHit hit_mouse;
    RaycastHit hit_attack;
    
    public GameObject Att_UI;
    public TextMeshProUGUI accurancy; //顯示命中素質
    public TextMeshProUGUI bullet_num; //顯示子彈數量
    bool attack;
    bool open_ui;
    float dis; //主角到目標的距離
    float dir; //子彈發射距離
    float atk; //攻擊力
    float acc; //準確度
    int bullet; //子彈數
    private void Start()
    {
        attack = false;
        Att_UI.SetActive(false);
        weapon = new weapons_inf();
        weapon.gun = this.gameObject;
        judgment_weapon(weapon); //判斷武器種類
    }
    // Update is called once per frame
    void Update()
    {
        target(); //判斷是否擊中目標
        GetAttack(weapon);//判斷射程 傷害 命中率 子彈數 並回傳到UI介面
    }
    bool target()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 man_pos = man.transform.position;
            mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition); //ray從攝影機位置出發到滑鼠點擊的位置        
            if (Physics.Raycast(mouse_ray, out hit_mouse)) //射線發出後第一個擊中的目標
            {
                Vector3 hit_pos = hit_mouse.transform.position; //滑鼠點擊到物件的座標
                Vector3 x = Vector3.Normalize(hit_pos - man_pos);//將看向目標的方向變成單位向量
                Vector3 look_target = hit_pos - man_pos;
                dis = Vector3.Distance(man_pos, hit_pos); //兩目標的距離
                Vector3 forwardLeft = Quaternion.Euler(0, 0, 0) * look_target; //從物件本身位置偏移
                for (float i = -5; i < 10; i += 3.0f)
                {
                    for (float j = -5; j < 5; j += 2.0f)
                    {
                        Vector3 vector_plus = Quaternion.Euler(i, j, 0) * forwardLeft;
                        Vector3 poss = look_target + vector_plus;
                        AttackH_ray = new Ray(man_pos, poss);//射線發出位置，方向
                        if (Physics.Raycast(AttackH_ray, out hit_attack) && hit_attack.transform.tag == "enemys")//確實看到敵人
                        {
                            GameObject gg = hit_attack.collider.gameObject;
                            man.transform.parent.transform.LookAt(gg.transform); //看向目標
                            attack = true;/*顯示UI 判斷命中率 子彈 等數據*/
                            Debug.DrawRay(man_pos, x*dir);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
    public bool GetAttack(weapons_inf gun)
    {
        if (attack)
        {
            Att_UI.SetActive(true);//顯示UI
            accurancy.text = "accuracy : " + acc;
            bullet_num.text = "bullet : " + bullet;
            
            if (dir > dis) //射程比距離長
            {
                acc = 70.0f;
                Debug.Log("可以瞄準到目標");
            }
            else
            {
                acc = 0.0f;
                Debug.Log("與目標的距離太遠 準確度為0");
                
            }
            Debug.Log(hit_attack.transform.position);
            //Debug.Log("Gun : " + gun.gun.tag +" dir : " +dir +" atk : " +atk +" acc : "  +acc + " bullet :" +  bullet + "--------------");
            //Debug.Log(bullet);
            //Debug.Log("目標可看見且在攻擊範圍內");
            attack = false;
            return true;
        }
        return false;
    }
    void judgment_weapon(weapons_inf Gun)//判斷武器種類
    {
        if (Gun.gun.tag == "Rifle")
        {
            Gun.dir = 15.0f;
            Gun.atk = 30.0f;
            Gun.acc = 50.0f;
            Gun.bullet = 30;
        }
        else if (Gun.gun.tag == "Sniper")
        {
            Gun.dir = 20.0f;
            Gun.atk = 70.0f;
            Gun.acc = 70.0f;
            Gun.bullet = 5;
        }
        else if (Gun.gun.tag == "ShotGun")
        {
            Gun.dir = 5.0f;
            Gun.atk = 80f;
            Gun.acc = 60f;
            Gun.bullet = 7;
        }
        else if (Gun.gun.tag == "Pistol")
        {
            Gun.dir = 10.0f;
            Gun.atk = 20.0f;
            Gun.acc = 50.0f;
            Gun.bullet = 7;
        }
        else if (Gun.gun.tag == "Knife")
        {
            Gun.dir = 1.0f;
            Gun.atk = 40.0f;
            Gun.acc = 100.0f;
            Gun.bullet = 10000;
        }
        dir = Gun.dir;
        atk = Gun.atk;
        acc = Gun.acc;
        bullet = Gun.bullet;
    }
}

