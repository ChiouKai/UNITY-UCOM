using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*  假設場景上有一個目標 攝影機為跟著目標移動
    攝影機的位置等於目標的位置加上一個距離
    攝影機永遠看向目標
    ------------------------------------
    搜尋場上已有的目標 放入列表
     */
public class Move_Camera : MonoBehaviour
{
    public Camera scene_camera;
    float move_speed = 6f;
    public float y = -45; //旋轉45度
    float gg;

    bool move_tr = false; //為true時攝影機移動到目標身上
    //bool rot_cam = false; //旋轉攝影機

    AI Target;

    private void Start()
    {

        scene_camera.transform.position = transform.position + new Vector3(7.95f, 15f, -7.95f);
        scene_camera.transform.LookAt(transform);
    }

    private void LateUpdate()
    {
        float fH = Input.GetAxis("Horizontal");
        float fV = Input.GetAxis("Vertical");
        //增加以45度角面向目標
        if (Target != null)
        {
            if (move_tr == true || Target.Moving)
            {

                //Vector3 div = (Target.transform.position - transform.position).normalized;
                //transform.position += div * Time.deltaTime * 10; //此方法為朝目標方向移動 but 太僵硬
                transform.position = Vector3.Lerp(transform.position, Target.transform.position, 5 * Time.deltaTime); //目前位置 要前往的位置 移動速度
                gg = Vector3.Distance(transform.position, Target.transform.position);
                if (gg < 0.001f)
                {
                    transform.position = Target.transform.position;
                    if (Target.tag == "Alien" && Target.Turn) //如果tag為敵人時繼續為true 代表持續鎖定目標位置
                        move_tr = true; //為true時不可移動
                    else move_tr = false; //為false則表示一開始移動到目標後就不進行動作
                }
            }
        }
        if (fH != 0 || fV != 0 ) //wsad移動
        {
            if (move_tr == false && !Target.Moving)
            {
                Vector3 mc_MoveForward = transform.forward * fV; //Vector3.forward => Vector3(0,0,1)
                mc_MoveForward.y = 0.0f;
                Vector3 mc_MoveRight = transform.right * fH; // Vector3.right = > Vector3(1,0,0)
                mc_MoveRight.y = 0.0f;
                transform.position = transform.position + (mc_MoveForward + mc_MoveRight) * move_speed * Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            y += 45;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            y -= 45;
        }
        Quaternion avc = Quaternion.Euler(0, y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, avc, Time.deltaTime * 5.0f);
        
    }

    public void ChaTurn(AI target)
    {
        Target = target;
        move_tr = true;
    } 
}