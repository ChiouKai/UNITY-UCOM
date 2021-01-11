using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COLLIDER_WALL : MonoBehaviour
{
    public Material[] wall_render;
    public Material[] wall_render_alpha;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hello : " +other.gameObject.name);
        if (other.tag == "Wall")
        {
            other.GetComponent<Renderer>().materials = wall_render_alpha;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wall")
        {
            other.GetComponent<Renderer>().materials = wall_render;
        }
    }
}
