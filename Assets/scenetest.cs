using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scenetest : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CSceneManager.m_Instance.ChangeSceneAsync("Main_GameScene"));
    }
    public void ActiveLoadingScene()
    {
        if (CSceneManager.m_Instance.m_bLoadingScene) //當m_bLoadingScene 還沒100%時為true
        {
            return;
        }
        CSceneManager.m_Instance.m_bActiveScene = true; //當loading完成時 m_bActiveScene 才為true 可以換場景
    }
}
