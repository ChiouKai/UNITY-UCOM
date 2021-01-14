using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager m_Instance;
    public float m_fProgress;

    private void Awake()
    {
        m_Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
    }

    public void BeginLoading()
    {
        // Setup loading element.
        m_fProgress = 0.0f;

        this.enabled = true;
    }

    public void UpdateLoading(float fProgres)
    {
        m_fProgress = fProgres;
    }

    public void FinishLoading()
    {
        Debug.Log("Finish");
        // Finish the loading.
    }

    // Update is called once per frame
    void Update()
    {

    }
}
