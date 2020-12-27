using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    public GameObject BarPrefab;
    public static UIMain m_Instance;  

    private List<createhp> m_HP_Bar= new List<createhp>();

    private void Awake()
    {
        m_Instance = this;        
    }
    void Start()
    {
    }

    public void CreateHP_Bar(Transform target, int HP)
    {
        GameObject go = GameObject.Instantiate(BarPrefab) as GameObject; //生成血條
        createhp bar = go.GetComponent<createhp>();
        bar.MaxHP = HP;
        bar.followedTarget = target; //血條的位置 = 角色的位置
        go.transform.SetParent(transform);

        m_HP_Bar.Add(bar); //放到列表中
    }

    private void LateUpdate()
    {
        foreach (createhp bar in m_HP_Bar) {
            if (bar.gameObject)
            {
                bar.UpdateHP_Bar();
            }            
        }        
    }

}
