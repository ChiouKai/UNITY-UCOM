using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*  假設場景上有一個目標 攝影機為跟著目標移動
    攝影機的位置等於目標的位置加上一個距離
    攝影機永遠看向目標*/
public class Move_Camera : MonoBehaviour
{
    public Camera scene_camera;
    public GameObject target;
    float move_speed = 5f;
    float y = 0; 
    int i = 0; 
    bool rot_cam;
    void Update()
    {
        //move(scene_camera,target);
    }
    private void LateUpdate()
    {
        float fH = Input.GetAxis("Horizontal");
        float fV = Input.GetAxis("Vertical");
        if (scene_camera != null)
        {
            Transform mc = target.transform;
            scene_camera.transform.LookAt(mc);

            if (fH != 0 || fV != 0)
            {
                Vector3 mc_MoveForward = mc.forward * fV; //Vector3.forward => Vector3(0,0,1)
                mc_MoveForward.y = 0.0f;
                Vector3 mc_MoveRight = mc.right * fH; // Vector3.right = > Vector3(1,0,0)
                mc_MoveRight.y = 0.0f;
                mc.position = mc.position + (mc_MoveForward + mc_MoveRight) * move_speed * Time.deltaTime;
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                y -= 45;
                i--;
                rot_cam = true;
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                y += 45;
                i ++;
                rot_cam = true;
            }
            if (rot_cam == true)
            {
                Quaternion avc = Quaternion.Euler(0, y, 0);
                mc.rotation = Quaternion.Slerp(mc.rotation, avc, Time.deltaTime * 5.0f);
                if (mc.localEulerAngles.y == 45 * i)
                {
                    rot_cam = false;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
            Gizmos.DrawLine(scene_camera.transform.position, target.transform.position);
    }
}

