using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class NPC_AI : AI
{

    // Start is called before the first frame update
    void Start()
    {
        Cha = GetComponent<Character>();
        Am = GetComponent<Animator>();
        PM = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()

    {
        Ediv = (enemy.position - transform.position).normalized;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);

        if (!Turn)
        {
            Idle();
        }
        else if (!Moving)
        {
            FindSelectableTiles();
            CheckMouse();
        }
        else if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            Move();

        }

    }
    private void LateUpdate()
    {
        if (MV != null && stateinfo.normalizedTime >= 1.0f)
        {
            StartCoroutine(MV());
        }
        if (Moving)
        {
            Am.Play("Run");
        }

    }



}