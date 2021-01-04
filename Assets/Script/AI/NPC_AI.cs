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
        Cha = GetComponent<Character>();
        Am = GetComponent<Animator>();
        EnemyLayer = 1 << 11;
        Gun = GetComponent<Weapon>();
        TileCount = FindDirection(transform.forward);
        Idle = NoCover;
        Enemies = RoundSysytem.GetInstance().Humans;
    }

    // Update is called once per frame
    void Update()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);

        if (!Turn)
        {
            Idle();
        }
        else if (stateinfo.IsName("Run") || stateinfo.IsName("Stop"))
        {
            Move2();
        }
        else if (NPCPreaera)
        {
            DoActing?.Invoke();
        }
    }
    private void FixedUpdate()
    {
        if (!Turn)
        {
            float MinDis = 99f;
            foreach (AI EnCha in Enemies)
            {
                float dis = (EnCha.transform.position - transform.position).magnitude;
                if (dis < MinDis)
                {
                    MinDis = dis;
                    enemy = EnCha.transform;
                }
            }
            Ediv = (enemy.position - transform.position).normalized;
        }
    }
    private void LateUpdate()
    {
        MTurn?.Invoke();

        if (PreAttack)
        {
            PreAttakeIdle();
        }
        
        //else if (PreAttack)
        //{
        //    if (ChangeTarget)
        //    {
        //        FaceTarget();
        //    }
        //    else
        //    {
        //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDir), 0.01f);
        //    }
        //}
    }




}