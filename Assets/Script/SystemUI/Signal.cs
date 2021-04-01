using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signal : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Location = Camera.main.WorldToScreenPoint(Target.position) + Vector3.right * 150f + Vector3.up * (100f+Sec*40f);
        transform.position = Location;
        Sec += Time.deltaTime;
        if (Sec > 2f)
        {
            Destroy(gameObject);
        }
    }

    float Sec = 0f;
    public Text Word;
    Transform Target;
    Vector3 Location;

    public void SetTarget(Transform target,string word)
    {
        Word.text = word;
        Target = target;
        Location = Camera.main.WorldToScreenPoint(Target.position) + Vector3.right * Random.Range(100, 200) + Vector3.up * 100f;
    }
}
