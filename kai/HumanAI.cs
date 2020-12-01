using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class HumanAI : AI
{ 

    // Start is called before the first frame update
    void Start()
    {
        CurrentTile.walkable = false;
        Vector3 CTP = CurrentTile.transform.position;
        ChaHeight = transform.position.y - CTP.y;
        CTP.y = transform.position.y;
        transform.position = CTP;
        Cha = GetComponent<Character>();
        Am = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        
        //}
        

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
    private void FixedUpdate()
    {
        Ediv = (enemy.position - transform.position).normalized;
    }
    public override IEnumerator LeftTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            
            Am.SetBool("Left", false);
            MV = null;
        }
        else
            yield return LeftTurn();
    }
    public override IEnumerator RightTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            
            Am.SetBool("Right", false);
            MV = null;
        }
        else
            yield return RightTurn();
    }
    public override IEnumerator BackTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            
            Am.SetBool("Back", false);
            MV = null;
        }
        else
            yield return BackTurn();
    }

}
