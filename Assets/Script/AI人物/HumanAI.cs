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
        //Cha = GetComponent<Character>();
        Am = GetComponent<Animator>();
        EnemyLayer = 1 << 10;
        Gun = GetComponent<Weapon>();
        TileCount = FindDirection(transform.forward);
        Idle = NoCover;
        UI = UISystem.getInstance();
        Enemies = RoundSysytem.GetInstance().Aliens;
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
            if (PreAttack)
            {

            }
            else
            {
                if(Prepera)
                    CheckMouse();
            }
        }
        else if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            Move();
        }
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
        }else if (PreAttack)
        {
            if (ChangeTarget)
            {
                FaceTarget();
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.01f);
            }
 
        }

    }
    private void FixedUpdate()
    {
        float MinDis = 99f;        
        foreach(AI EnCha in Enemies)
        {
            float dis=(EnCha.transform.position - transform.position).magnitude;
            if (dis < MinDis)
            {
                MinDis = dis;
                enemy = EnCha.transform;
            }
        }
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
