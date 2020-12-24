using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    public Canvas m_Canvas;
    public GameObject BarIndicatorPrefab;
    public static UIMain m_Instance;  

    private List<HP_Bar> m_HP_Bar;
   
    void Start()
    {
        m_Instance = this;
        m_Canvas = GetComponent<Canvas>();
        m_HP_Bar = new List<HP_Bar>();
    }

    public void CreateHP_Bar(Transform target)
    {
        GameObject go = GameObject.Instantiate(BarIndicatorPrefab) as GameObject;
        HP_Bar bar = go.GetComponent<HP_Bar>();
        bar.followedTarget = target;
        go.transform.parent = this.transform;  //UIMain指定給Canvas，藉此指定血條給Canvas
        m_HP_Bar.Add(bar);
    }

    private void LateUpdate()
    {
        foreach (HP_Bar bar in m_HP_Bar) {
            if (bar.gameObject.activeSelf) { bar.UpdateHP_Bar(); }            
        }        
    }

}
