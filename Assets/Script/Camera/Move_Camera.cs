using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    float td = 0; //時間
    int i = 0; // 地板提示變數
    bool move_tr = false; //為true時攝影機移動到目標身上
    //bool rot_cam = false; //旋轉攝影機
    public GameObject Enemy_star;
    public GameObject Our_star;
    public GameObject Now_man;
    public Material[] enstar;
    public Material[] ourstar;
    public float cam_dis;
    UISystem US;
    public bool att_cam_bool;
    public Transform Birth_cam;
    AI Target;

    private void Start()
    {
        att_cam_bool = false;//關掉攻擊攝影機
        US = UISystem.getInstance();
        Enemy_star.SetActive(false);
        Our_star.SetActive(false);
    }
    float T_Explosion;
    private void LateUpdate()
    {
        float fH = Input.GetAxis("Horizontal");
        float fV = Input.GetAxis("Vertical");
        //增加以45度角面向目標
        US.final_text.text = (5 - US.Bomb_Round).ToString();
        if (US.Bomb_Round == 3)
        {
            US.toggle[2].SetActive(true);
            US.toggle[1].transform.GetChild(1).GetComponent<Text>().color = Color.green;
            US.toggle[1].transform.GetChild(0).GetComponent<Image>().sprite = US.mission_Images[1];
        }
        if (US.Bomb_Round > 5) //安裝炸彈且稱超過5回合
            US.lose_check = true;

        if (US.lose_check) //
            US.mission_failure.SetActive(true);
        if (US.win_check)
        {
            T_Explosion += Time.deltaTime;
            if (T_Explosion >= 3f)
            {
                US.explosion.SetActive(false);
                US.mission_success.SetActive(true);
                US.CAM.SetActive(false);
                US.CAM_TIMELINE.SetActive(true);
                US.HPCanvas.gameObject.SetActive(false);
            }
        }

        scene_camera.transform.LookAt(transform);
        if (att_cam_bool == false)
        {
            float axc = Vector3.Distance(scene_camera.transform.position, Birth_cam.transform.position);
            scene_camera.transform.position = Vector3.Lerp(scene_camera.transform.position, Birth_cam.transform.position, 3 * Time.deltaTime);
            if (axc <= 0.05) scene_camera.transform.position = Birth_cam.transform.position;
        } //攻擊攝影機關閉時回到原位

        if (Target != null)
        {
            if (Target.tag == "Alien")
            {
                var b = Enemy_star.GetComponent<MeshRenderer>();

                td += Time.deltaTime;

                Enemy_star.SetActive(true);
                Our_star.SetActive(false);
                if (td > 1)
                {
                    td = 0;
                    i++;
                    if (i >= enstar.Length) i = 0;
                }
                b.material = enstar[i];
                Now_man.transform.position = Target.transform.position + new Vector3(0, 3f, 0);
                Enemy_star.transform.position = Target.transform.position + new Vector3(0, 0.05f, 0);
            }
            if (Target.tag == "Human")
            {
                var b = Our_star.GetComponent<MeshRenderer>();
                td += Time.deltaTime;
                if (td > 1)
                {
                    td = 0;
                    i++;
                    if (i >= ourstar.Length) i = 0;
                }
                b.material = ourstar[i];
                Enemy_star.SetActive(false);
                Our_star.SetActive(true);
                Now_man.transform.position = Target.transform.position + new Vector3(0, 3f, 0);
                Our_star.transform.position = Target.transform.position + new Vector3(0, 0.05f, 0);
            }
            if (att_cam_bool == false)
            {
                if (move_tr == true || Target.Moving)
                {
                    transform.position = Vector3.Lerp(transform.position, Target.transform.position, 5 * Time.deltaTime); //目前位置 要前往的位置 移動速度
                    float gg = Vector3.Distance(transform.position, Target.transform.position);
                    if (gg < 0.01f)
                    {
                        transform.position = Target.transform.position;
                        move_tr = false; //為false則表示一開始移動到目標後就不進行動作
                    }
                }
            }
            if (att_cam_bool)
            {
                US.Attack_camera();
            }

        }


        if (fH != 0 || fV != 0) //wsad移動
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
