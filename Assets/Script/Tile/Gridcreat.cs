using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridcreat : MonoBehaviour
{

    public GameObject grid;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 27; ++i)
        {
            for (int j = 0; j < 42; ++j)
            {
                GameObject GD = Instantiate(grid, transform.position + new Vector3((float)i*0.67f, 0.001f,(float) j*0.67f), grid.transform.rotation);
                GD.transform.parent = gameObject.transform;
            }
        }
    }


}
