using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    public AI ai;
    public GameObject Blood;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")//子彈打到才做動作
        {
            Vector3 DIV = other.transform.forward;
            ai.Hurt(DIV);
            GameObject blood = Instantiate(Blood, transform.position, new Quaternion());
            blood.transform.forward = -DIV;//火花方向 = 子彈的反方向
            blood.SetActive(true); //讓火花顯示
            Destroy(blood, 0.7f); //一秒後刪除火花效果
        }
    }
}
