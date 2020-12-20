using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnTest : MonoBehaviour
{
    private bool isEN = false;
    private GameObject iu;
    private void Start()
    {
        iu = transform.GetChild(0).gameObject;
        iu.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "environment")
        {
            isEN = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "environment")
        {
            isEN = false;
        }
    }
    private void OnMouseOver()
    {
        if(!isEN)
            iu.SetActive(true);
        Debug.Log("1");
    }
    private void OnMouseExit()
    {
        iu.SetActive(false);
        Debug.Log("0");
    }

}
