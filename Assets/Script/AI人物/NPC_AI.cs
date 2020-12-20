using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class NPC_AI : AI
{

    // Start is called before the first frame update
    void Start()
    {
        CurrentTile.walkable = false;
        Vector3 CTP = CurrentTile.transform.position;
        ChaHeight = transform.position.y - CTP.y;
        CTP.y = transform.position.y;
        transform.position = CTP;
        //Cha = GetComponent<Character>();
        Am = GetComponent<Animator>();
        EnemyLayer = 1 << 11;
        UI = UISystem.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);

        if (!Turn)
        {
            NoCover();
        }
        else if (!Moving)
        {
            if(MV != null)
                StartCoroutine(MV());
            //AttakeableDetect();
            //FindSelectableTiles();
            //CheckMouse();
        }
        else if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            Move();

        }

    }
    private void FixedUpdate()
    {
        Ediv = (enemy.position - transform.position).normalized;
    }
    private void LateUpdate()
    {
        if (MV != null && stateinfo.IsName("Idle"))
        {
            StartCoroutine(MV());
        }
        if (Moving)
        {
            Am.Play("Run");
        }

    }



}