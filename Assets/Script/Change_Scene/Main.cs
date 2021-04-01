using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    public void change_scene(string scenename)
    {
        Debug.Log(scenename);
        SceneManager.LoadScene(scenename);
        StartCoroutine(CSceneManager.m_Instance.ChangeSceneAsync("Main_GameScene"));
        
    }
    //IEnumerator LoadCustomData()
    //{
    //    float fProgress = 50.0f;
    //    // Load Configure
    //    while (true)
    //    {
    //        if (fProgress > 60.0f)
    //        {
    //            break;
    //        }
    //        fProgress += 1f;
    //        yield return 0; //下一幀數繼續處理
    //    }

    //    // Load Character
    //    while (true)
    //    {
    //        if (fProgress > 70.0f)
    //        {
    //            break;
    //        }
    //        LoadingManager.m_Instance.UpdateLoading(fProgress);
    //        fProgress += 1f;
    //        yield return 0;
    //    }


    //    // Load NPC
    //    while (true)
    //    {
    //        if (fProgress > 80.0f)
    //        {
    //            break;
    //        }
    //        LoadingManager.m_Instance.UpdateLoading(fProgress);
    //        fProgress += 1f;
    //        yield return 0;
    //    }

    //    // Load Others.
    //    while (true)
    //    {
    //        if (fProgress > 100.0f)
    //        {
    //            LoadingManager.m_Instance.UpdateLoading(100.0f);
    //            break;
    //        }
    //        LoadingManager.m_Instance.UpdateLoading(fProgress);
    //        fProgress += 1f;
    //        yield return 0;
    //    }
    //}
    //public void reload_scene()
    //{
    //    UISystem.getInstance().Round.Abort();
    //    Scene scene = SceneManager.GetActiveScene();
    //    SceneManager.LoadScene(scene.name);
    //}

    //public IEnumerator ChangeSceneAsync(string sName, IEnumerator customLoader)
    //{
    //    AsyncOperation ao = SceneManager.LoadSceneAsync(sName);
    //    if (ao == null)  //如果廠景名稱不存在就不執行
    //    {
    //        yield break;
    //    }
    //    //LoadingManager.m_Instance.BeginLoading(); //按下去時開始loading 進度條=0
    //    m_bLoadingScene = true; //開始跑進度條
    //    m_bActiveScene = false; //還不能切換場景
    //    ao.allowSceneActivation = false; //還不允許切換場景
    //    //float fTotalProgress = 0.0f; //剛按下時進度條為0
    //    while (true)
    //    {
    //       // LoadingManager.m_Instance.UpdateLoading(fTotalProgress);
    //        //fTotalProgress = ao.progress * 50.0f;
    //        if (ao.progress > 0.8999f)
    //        {
    //            //fTotalProgress = 50.0f;
    //            //LoadingManager.m_Instance.UpdateLoading(fTotalProgress);
    //            yield return customLoader;
    //            Debug.Log("Finish Load custom data");
    //            break;
    //        }
    //        yield return 0;
    //    }
    //   // LoadingManager.m_Instance.UpdateLoading(100.0f);
    //    Debug.Log("Wait for active");
    //    m_bLoadingScene = false;
    //    while (m_bActiveScene == false) //當切換場景的bool為false時 不執行下面程式
    //    {
    //        yield return null;
    //    }

    //    ao.allowSceneActivation = true; //切換場景
    //   // LoadingManager.m_Instance.FinishLoading(); //跳出切換成功提示
    //}
}
