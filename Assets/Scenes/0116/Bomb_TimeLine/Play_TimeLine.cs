using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Play_TimeLine : MonoBehaviour
{
    public GameObject CAM;
    public GameObject CAM_TIMELINE;
    public void OPEN_TIMELINE()
    {
        CAM.SetActive(false);
        CAM_TIMELINE.SetActive(true);
    }
}
