using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_text : MonoBehaviour
{
    public Text mis_col;
    public bool fin;
    public float speed;
    void Update()
    {
        if (fin) fadein(mis_col, speed);
        else fadeout(mis_col, speed);
    }
    void fadein(Text text, float speed)
    {
        var a = text.color;
        a.a = Mathf.Min(a.a + Time.deltaTime * speed, 1);
        text.color = a;
        if (a.a == 1) fin = false;
    }
    void fadeout(Text text, float speed)
    {
        var a = text.color;
        a.a = Mathf.Max(a.a - Time.deltaTime * speed, 0);
        text.color = a;
        if (a.a == 0) fin = true;
    }
}
