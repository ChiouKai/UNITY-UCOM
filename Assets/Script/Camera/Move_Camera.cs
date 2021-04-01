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
    float move_speed = 7f;
    public float y = -45; //旋轉45度
    bool move_tr = false; //為true時攝影機移動到目標身上
    public float cam_dis;
    UISystem US;
    public bool att_cam_bool;
    public Transform Birth_cam;
    AI TurnCha;

    private void Start()
    {
        US = UISystem.getInstance();
    }

    private void LateUpdate()
    {
        float fH = Input.GetAxis("Horizontal");
        float fV = Input.GetAxis("Vertical");
        //增加以45度角面向目標

        scene_camera.transform.LookAt(transform);
        if (att_cam_bool == false)
        {
            float axc = Vector3.Distance(scene_camera.transform.position, Birth_cam.transform.position);
            scene_camera.transform.position = Vector3.Lerp(scene_camera.transform.position, Birth_cam.transform.position, 3 * Time.deltaTime);
            if (axc <= 0.05) scene_camera.transform.position = Birth_cam.transform.position;
        } //攻擊攝影機關閉時回到原位

        if (TurnCha != null)
        {
            if (att_cam_bool == false)
            {
                if (move_tr == true || TurnCha.Moving)
                {
                    transform.position = Vector3.Lerp(transform.position, TurnCha.transform.position, 5 * Time.deltaTime); //目前位置 要前往的位置 移動速度
                    float gg = Vector3.Distance(transform.position, TurnCha.transform.position);
                    if (gg < 0.01f)
                    {
                        transform.position = TurnCha.transform.position;
                        move_tr = false; //為false則表示一開始移動到目標後就不進行動作
                    }
                }
            }
            else
            {
                if (PlayerCam)
                {
                    Vector3 dis = TurnCha.transform.position - TargetPos;
                    CubePoint += dis*0.5f;
                    CamPoint += dis * 1.5f;
                    TargetPos = TurnCha.transform.position;
                }
                transform.position = Vector3.Lerp(transform.position, CubePoint, 5 * Time.deltaTime);
                scene_camera.transform.position = Vector3.Lerp(scene_camera.transform.position, transform.position + CamPoint, 3 * Time.deltaTime);
            }

        }


        if (fH != 0 || fV != 0) //wsad移動
        {
            if (move_tr == false && !TurnCha.Moving)
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

    private Vector3 CamPoint;
    private Vector3 CubePoint;
    public float anglee;
    Vector3 TargetPos;
    public bool PlayerCam;
    public void PlayerSetAtkCam(AI Target)
    {
        att_cam_bool = true;
        anglee = 15f;

        PlayerCam = true;
        TargetPos = TurnCha.transform.position;
        Vector3 div = Target.transform.position - TurnCha.transform.position;
        int dir = TurnCha.FindDirection(div);
        Tile.Cover[] cover=TurnCha.CurrentTile.JudgeCover(div, out float non);
        int i = 1;
        if (cover[0] != Tile.Cover.None || cover[1] != Tile.Cover.None)
        {
            for (int j = 2; j > 0; --j)
            {
                if ((int)cover[0] == j)
                {
                    break;
                }
                else if ((int)cover[1] == j)
                {
                    if (dir % 2 == 0)
                    {
                        if (div.x > 0) dir = 3;
                        else dir = 1;
                    }
                    else
                    {
                        if (div.z > 0) dir = 0;
                        else dir = 2;
                    }
                    break;
                }
            }
            Vector3 LoR = Vector3.Cross(TurnCha.Direction(dir), div);
            if (LoR.y >= 0)
            {
                i = 1;
            }
            else
            {
                i = -1;
            }
            CubePoint = TurnCha.transform.position + div / 4f + Vector3.up * 1f;
            //float tmpangle = Mathf.Asin(Mathf.Sin(anglee * Mathf.Deg2Rad) / div.magnitude * 1.5f) * Mathf.Rad2Deg;
            //tmpangle = 180f - tmpangle - anglee;
            //CamPoint = -div / 5f + Quaternion.AngleAxis(tmpangle * i, Vector3.up) * div.normalized * 1.5f;
            for (int j = 0; j < 4; ++j)
            {

                float tmpangle = Mathf.Asin(Mathf.Sin(anglee * Mathf.Deg2Rad) / div.magnitude * 1.5f) * Mathf.Rad2Deg;
                tmpangle = 180f - tmpangle - anglee;
                CamPoint = -div / 5f + Quaternion.AngleAxis(tmpangle * i, Vector3.up) * div.normalized * 1.5f;
                if (!Physics.Raycast(CamPoint + CubePoint, Target.transform.position - CamPoint - CubePoint+Vector3.up*1.3f, div.magnitude, 1 << 9))
                {
                    break;
                }
                anglee -= 5f;
            }
            if (anglee < 0.1f)
            {
                float tmpangle = Mathf.Asin(Mathf.Sin(10f * Mathf.Deg2Rad) / div.magnitude * 1.5f) * Mathf.Rad2Deg;
                tmpangle = 180f - tmpangle - 10f;
                CamPoint = -div / 5f + Quaternion.AngleAxis(tmpangle * i, Vector3.up) * div.normalized * 1.5f;
            }
        }
        else
        {
            CubePoint = TurnCha.transform.position + div / 4f + Vector3.up * 1f;
            for (int j = 0; j < 4; ++j)
            {
                float tmpangle = Mathf.Asin(Mathf.Sin(anglee * Mathf.Deg2Rad) / div.magnitude * 1.5f) * Mathf.Rad2Deg;
                tmpangle = 180f - tmpangle - anglee;
                CamPoint = -div / 5f + Quaternion.AngleAxis(tmpangle, Vector3.up) * div.normalized * 1.5f;
                if (!Physics.Raycast(CamPoint + CubePoint, Target.transform.position - CamPoint - CubePoint + Vector3.up * 1.3f, div.magnitude, 1 << 9))
                {
                    break;
                }
                anglee -= 5f;
            }
            if (!(anglee < 0.1f))
            {
                return;
            }
            anglee = 20f;
            for (int j = 0; j < 4; ++j)
            {
                float tmpangle = Mathf.Asin(Mathf.Sin(anglee * Mathf.Deg2Rad) / div.magnitude * 1.5f) * Mathf.Rad2Deg;
                tmpangle = 180f - tmpangle - anglee;
                CamPoint = -div / 5f + Quaternion.AngleAxis(-tmpangle, Vector3.up) * div.normalized * 1.5f;
                if (!Physics.Raycast(CamPoint + CubePoint, Target.transform.position - CamPoint - CubePoint + Vector3.up * 1.3f, div.magnitude, 1 << 9))
                {
                    break;
                }
                anglee -= 5f;
            }
            if ((anglee < 0.1f))
            {
                float tmpangle = Mathf.Asin(Mathf.Sin(10f * Mathf.Deg2Rad) / div.magnitude * 1.5f) * Mathf.Rad2Deg;
                tmpangle = 180f - tmpangle - 10f;
                CamPoint = -div / 5f + Quaternion.AngleAxis(tmpangle, Vector3.up) * div.normalized * 1.5f;
            }

            //float tmpangle = Mathf.Asin(Mathf.Sin(anglee * Mathf.Deg2Rad) / div.magnitude * 1.5f) * Mathf.Rad2Deg;
            //tmpangle = 180f - tmpangle - anglee;
            //CamPoint = -div / 5f + Quaternion.AngleAxis(tmpangle * i, Vector3.up) * div.normalized * 1.5f;
            //CubePoint = TurnCha.transform.position + div / 5f + Vector3.up * 1f;

        }

        
    }
    public void NPCSetAtkCam(AI Target)
    {
        att_cam_bool = true;
        Vector3 div = Target.transform.position - TurnCha.transform.position;

        float temp = div.magnitude / 2f * 37f / 20f * 5f / 3f / (scene_camera.transform.position - transform.position).magnitude;
        CubePoint = TurnCha.transform.position + div / 2;
        if (temp > 1)
        {
            CamPoint = temp *1.2f* (scene_camera.transform.position - transform.position);
        }
        else
        {
            CamPoint = scene_camera.transform.position-transform.position;
        }
    }

    

    public void ChaTurn(AI target)
    {
        TurnCha = target;
        move_tr = true;
    }

    public void EndCam()
    {
        att_cam_bool = false;
        PlayerCam = false;
    }
}
