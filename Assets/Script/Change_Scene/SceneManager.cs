using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSceneManager
{
    public static CSceneManager m_Instance = null;

    public delegate void FinishPreparedScene();
    FinishPreparedScene m_FinishPrepared;

    public bool m_bLoadingScene; //進度條的bool
    public bool m_bActiveScene; //可切換場景的bool

    public CSceneManager()
    {
        m_Instance = this;
        m_bLoadingScene = false; 
        m_bActiveScene = false;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene s, LoadSceneMode mode) //場景切換後執行
    {

        Debug.Log(s.name + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    /*public void ActiveLoadingScene()
    {
        if (m_bLoadingScene) //當m_bLoadingScene 還沒100%時為true
        {
            return;
        }
        m_bActiveScene = true; //當loading完成時 m_bActiveScene 才為true 可以換場景
    }*/

    public IEnumerator ChangeSceneAsync(string sName, IEnumerator customLoader)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sName);
        if (ao == null)  //如果廠景名稱不存在就不執行
        {
            yield break;
        }
        LoadingManager.m_Instance.BeginLoading(); //按下去時開始loading 進度條=0
        m_bLoadingScene = true; //開始跑進度條
        m_bActiveScene = false; //還不能切換場景
        ao.allowSceneActivation = false; //還不允許切換場景
        float fTotalProgress = 0.0f; //剛按下時進度條為0
        while (true)
        {
            LoadingManager.m_Instance.UpdateLoading(fTotalProgress);
            fTotalProgress = ao.progress * 50.0f;
            if (ao.progress > 0.8999f)
            {
                fTotalProgress = 50.0f;
                LoadingManager.m_Instance.UpdateLoading(fTotalProgress);
                yield return customLoader;
                Debug.Log("Finish Load custom data");
                break;
            }
            yield return 0;
        }
        LoadingManager.m_Instance.UpdateLoading(100.0f);
        Debug.Log("Wait for active");
        m_bLoadingScene = false;
        while (m_bActiveScene == false) //當切換場景的bool為false時 不執行下面程式
        {
            yield return 0;
        }

        ao.allowSceneActivation = true; //切換場景
        LoadingManager.m_Instance.FinishLoading(); //跳出切換成功提示
    }
}
