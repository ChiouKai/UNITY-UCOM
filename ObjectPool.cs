using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
    創立一個空物件放入物件池程式再放入槍管位置
    創一個子彈預製物放入bullet程式

     */
public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initailSize = 10;

    private Queue<GameObject> m_pool = new Queue<GameObject>();

    void Awake()
    {
        for (int cnt = 0; cnt < initailSize; cnt++) // 先建立20個預製物放入佇列
        {
            GameObject go = Instantiate(prefab) as GameObject;
            m_pool.Enqueue(go); go.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ReUse(transform.position, transform.rotation);
        }
        

    }
    public void ReUse(Vector3 position, Quaternion rotation)
    {
        if (m_pool.Count > 0) //從物件池中取出
        {
            GameObject reuse = m_pool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
        }
        else //如果物件池都被取出了則重新生成物件
        {
            GameObject go = Instantiate(prefab) as GameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
        }
    }


    public void Recovery(GameObject recovery)//將物件放回物件池中
    {
        m_pool.Enqueue(recovery);
        recovery.SetActive(false);
    }
}