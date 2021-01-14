using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontdestroy : MonoBehaviour
{
    public GameObject prefab;          // 這是個預製，直接拖拽賦值
    GameObject clone;                  // 用來接收預製的克隆體
    static bool isHaveClone = false;   // 靜態變量，所有腳本共用，也就是保證預製只能被克隆一次，不會出現多個角色
    // Use this for initialization
    void Start()
    {
        if (!isHaveClone)
        {

                clone = (GameObject)GameObject.Instantiate(prefab);
                isHaveClone = true;
                GameObject.DontDestroyOnLoad(clone);
        }
    }
}
