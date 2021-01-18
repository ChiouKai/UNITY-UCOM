using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_text : MonoBehaviour
{
    public Text mis_col;
    public bool open;
    public bool fin;
    bool textfin;
    void Update()
    {
        if (mis_col.color.a == 0) fin = true;
        if (mis_col.color.a == 1) fin =false;
        if (fin) fadein(mis_col, 1);
        if (!fin) fadeout(mis_col, 1);
    }
    void fadein(Text text, float speed)
    {
        var a = text.color;
        a.a = Mathf.Min(a.a + Time.deltaTime * speed, 1);
        text.color = a;
    }
    void fadeout(Text text, float speed)
    {
        var a = text.color;
        a.a = Mathf.Max(a.a - Time.deltaTime * speed, 0);
        text.color = a;
    }
}
