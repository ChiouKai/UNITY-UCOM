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
    public GameObject target;
    public GameObject[] targets;
    List<GameObject> target_list;
    float move_speed = 5f;
    float y =0; //旋轉45度
    int tr = 0 ; //列表索引值
    bool move_tr = true; //為true時攝影機移動到目標身上
    bool rot_cam = false; //旋轉攝影機
    Transform mc; //mc變數為目標的transform

    private void Start()
    {
        //初始化攝影機位置
        scene_camera.transform.localPosition = new Vector3(0, 5f, -3.5f);
        mc = target.transform;
        target_list = new List<GameObject>();

        for (int i=0;i<targets.Length;i++)
        {
            target_list.Add(targets[i]);
        }
        target_list.Insert(0,target); 
        //target_list.Add(target);
        foreach (GameObject tem in target_list)
        {
            Debug.Log(tem.transform.position);
        }
        Debug.Log("----------\n"+target_list[4].transform.position);
        Debug.Log("----------\n" + target_list[1].transform.localEulerAngles);
        //mc.parent = target_list[tr].transform;
        //mc.parent.rotation = Quaternion.Euler(0,0, 0);
        //Debug.Log(scene_camera.transform.position);
    }
    void Update()
    {
        Debug.Log(y);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tr >= target_list.Count -1 )
            {
                tr = 0;
            }
            //mc.localEulerAngles = new Vector3(0, 0, 0);
            tr++;
            //mc.parent = target_list[tr].transform;
            //mc.rotation = Quaternion.Euler(0, 0, 0);
            move_tr = true;
        }
        //move(scene_camera,target);
    }
    private void LateUpdate()
    {
        float fH = Input.GetAxis("Horizontal");
        float fV = Input.GetAxis("Vertical");
        if (scene_camera != null)
        {
            //增加以45度角面向目標
            if (move_tr == true)
            {

                mc.position = target_list[tr].transform.position; //一開始為true時將位置轉換到目標位置
                
                if (target_list[tr].transform.tag == "enemys") //如果tag為敵人時繼續為true 代表持續鎖定目標位置
                    move_tr = true; //為true時不可移動
                else move_tr = false; //為false則表示一開始移動到目標後就不進行動作
            }
            
            //scene_camera.transform.LookAt(mc); //攝影機看向目標

            if (fH != 0 || fV != 0) //wsad移動
            {
                if (move_tr == false)
                {
                    Vector3 mc_MoveForward = mc.forward * fV; //Vector3.forward => Vector3(0,0,1)
                    mc_MoveForward.y = 0.0f;
                    Vector3 mc_MoveRight = mc.right * fH; // Vector3.right = > Vector3(1,0,0)
                    mc_MoveRight.y = 0.0f;
                    mc.position = mc.position + (mc_MoveForward + mc_MoveRight) * move_speed * Time.deltaTime;
                }
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                y -= 45;
                rot_cam = true;
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                y += 45;
                rot_cam = true;
            }
            if (rot_cam == true)
            {
                //Debug.Log(mc.localEulerAngles.y + "-------------" + Mathf.Round(mc.localEulerAngles.y) % 45);
                Quaternion avc = Quaternion.Euler(0, y, 0);
                mc.rotation = Quaternion.Slerp(mc.rotation, avc, Time.deltaTime * 5.0f);
                /*if (Mathf.Round(mc.localEulerAngles.y) % 45  ==0 )
                {  
                    mc.rotation = Quaternion.Euler(0, y, 0);
                    rot_cam = false;
                }*/
            }
        }
    }
    void OnDrawGizmos()
    {
            Gizmos.DrawLine(scene_camera.transform.position, target.transform.position);
    }
}

