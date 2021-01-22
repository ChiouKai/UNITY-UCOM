using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_TimeLine : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Main_cam;
    public void Close_TimeLine()
    {
        Main_cam.SetActive(true);
        this.gameObject.SetActive(false); 
    }
    public void Open_Main_camera()
    {
        Main_cam.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void Close_Main_camera()
    {
        Main_cam.SetActive(false);
    }
}
