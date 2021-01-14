using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    CSceneManager m_ScManager;
    bool b_change;
    string scene_name;
    public GameObject button_canvas;
    // Start is called before the first frame update
    void Start()
    {
        m_ScManager = new CSceneManager();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (b_change)
        {
            Debug.Log("開始換場景");
            StartCoroutine(m_ScManager.ChangeSceneAsync(scene_name, LoadCustomData()));
            b_change = false;
            button_canvas.SetActive(false);
        }
        else if (m_ScManager.m_bLoadingScene==false) //當進度條跑到100時為false
        {
            m_ScManager.m_bActiveScene = true;
        }
    }
    public void change_scene(string scenename)
    {
        b_change = true;
        scene_name = scenename;
        
    }
    IEnumerator LoadCustomData()
    {
        float fProgress = 50.0f;
        // Load Configure
        while (true)
        {
            if (fProgress > 60.0f)
            {
                break;
            }
            LoadingManager.m_Instance.UpdateLoading(fProgress);
            fProgress += 1f;
            yield return 0; //下一幀數繼續處理
        }

        // Load Character
        while (true)
        {
            if (fProgress > 70.0f)
            {
                break;
            }
            LoadingManager.m_Instance.UpdateLoading(fProgress);
            fProgress += 1f;
            yield return 0;
        }


        // Load NPC
        while (true)
        {
            if (fProgress > 80.0f)
            {
                break;
            }
            LoadingManager.m_Instance.UpdateLoading(fProgress);
            fProgress += 1f;
            yield return 0;
        }

        // Load Others.
        while (true)
        {
            if (fProgress > 100.0f)
            {
                LoadingManager.m_Instance.UpdateLoading(100.0f);
                break;
            }
            LoadingManager.m_Instance.UpdateLoading(fProgress);
            fProgress += 1f;
            yield return 0;
        }
    }
}
