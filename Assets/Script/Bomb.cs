using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    static public bool Bomb_start;
    static public int Bomb_round;
    RoundSysytem RS;
    // Start is called before the first frame update
    void Start()
    {
        RS = RoundSysytem.GetInstance();
        Bomb_start = false;
        Bomb_round = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
