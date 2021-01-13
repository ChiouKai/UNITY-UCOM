using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*  在子彈上面掛三種粒子系統
 *  子彈撞到障礙物後不能馬上recovery才能在子彈子物件展示火花
 *  撞到後 子彈消失 ->在製作一個火花出來
 *  or 跟物件池一起 撞到後顯示出來 but 要讓子彈隨障礙物消失並產生火花
 *  or 撞到後在碰撞的位置建立新的物件產生火花
    如果碰到障礙物 - > 出現黃色火花
    碰到外星人 - > 濺出藍血
    碰到我方 -> 濺出紅血*/
public class bullet : MonoBehaviour
{
    public GameObject wall_par;
    public GameObject HumanBlood;
    public GameObject AlienBlood;
    public Vector3 AttackPoint;
    public Vector3 FirePoint;
    float distance;
    public float Speed = 20f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((transform.position-AttackPoint).magnitude <0.5f)
        {
            transform.position = AttackPoint;
            Destroy(gameObject,0.1f);
        }
        else
        {
            transform.position += transform.forward * Time.deltaTime * Speed;
        }
    }


    public void SetAttackPoint(Vector3 Position ,Vector3 point)
    {
        FirePoint = Position;
        AttackPoint = point;
        distance = (Position - point).magnitude;
    }



    private void OnTriggerEnter(Collider other)
    {
        
            if (other.tag == "En"||other.tag == "Wall")
            {

                Vector3 DIV = -transform.forward; //子彈前進方向的反方向
                if (wall_par == null)
                {
                    return;
                }
                GameObject fire = Instantiate(wall_par, transform.position, new Quaternion());
                fire.transform.forward = DIV;//火花方向 = 子彈的反方向
                fire.SetActive(true); //讓火花顯示
                Destroy(fire, 0.7f); //一秒後刪除火花效果

            }
    }
}
