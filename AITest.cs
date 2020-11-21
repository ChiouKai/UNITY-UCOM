using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class AITest : MonoBehaviour
{
    
    Animator Am;
    AnimatorStateInfo stateinfo;
    public Transform player;
    public delegate IEnumerator move();
    private move MV = null;
    Character Cha;
    // Start is called before the first frame update
    void Start()
    {
        Cha = GetComponent<Character>();
        Am = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Cha.AP == 0)
        {
            Idle();
        }
        else
        {
            //move
            //attack
        }
        

    }
    private void LateUpdate()
    {
        if (MV != null && stateinfo.normalizedTime >= 1.0f)
        {
            StartCoroutine(MV());
        }
    }

    void Idle()
    {
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (MV == null && stateinfo.IsName("Idle"))
        {
            Vector3 vdiv = (player.position - transform.position).normalized;
            float FoB = Vector3.Dot(transform.forward, vdiv);
            if (FoB > 1 / Mathf.Sqrt(2))
            {
                
            }
            else if (FoB < -1 / Mathf.Sqrt(2))
            {
                Am.SetBool("Back", true);
                MV = BackTurn;
            }
            else
            {
                Vector3 LoR = Vector3.Cross(transform.forward, vdiv);
                if (LoR.y > 0.0f)
                {
                    Am.SetBool("Right", true);
                    MV = RightTurn;
                }
                else
                {
                    Am.SetBool("Left", true);
                    MV = LeftTurn;
                }

            }

        }
    }

    IEnumerator LeftTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, -90f, 0);
            Am.SetBool("Left", false);
            MV = null;
        }
        else
            yield return LeftTurn();
    }
    IEnumerator RightTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >=1.0f)
        {
             transform.Rotate(0, 90f, 0);
             Am.SetBool("Right", false);
            MV = null;
        }
        else
            yield return RightTurn();
    }
    IEnumerator BackTurn()
    {
        yield return null;
        stateinfo = Am.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.normalizedTime >= 1.0f)
        {
            transform.Rotate(0, 180f, 0);
            Am.SetBool("Back", false);
            MV = null;
        }
        else
            yield return BackTurn();
    }

}
