using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Tile T1, T2;
    // Start is called before the first frame update
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(T1.transform.position + Vector3.up * 1.2f, T2.transform.position + Vector3.up * 1.2f);
    }
}
