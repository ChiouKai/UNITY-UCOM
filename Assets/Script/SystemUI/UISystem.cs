using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public Move_Camera MoveCam;
    List<AI> Humans, Aliens;
    public AI TurnCha;
    RoundSysytem m_Roundsystem;
    Thread Round;
    public Action TurnRun;
    public Action RunUI;
    public GameObject BelowButtonAndText;
    public RectTransform RT;
    public Text ButtonText;
    public Text DescribeText;
    public Text LeftText;
    public Text RightText;
    LinkedListNode<(AI, int, int)> Target;
    public GameObject MouseOnTile;

    static public UISystem getInstance()
    {
        return Instance;
    }
    static private UISystem Instance;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Roundsystem = RoundSysytem.GetInstance();
        GameObject[] GHumans = GameObject.FindGameObjectsWithTag("Human");
        Humans = new List<AI>();
        for(int count = 0; count< GHumans.Length; ++count)
        {
            Humans.Add(GHumans[count].GetComponent<HumanAI>());
        }
        GameObject[] GAliens = GameObject.FindGameObjectsWithTag("Alien");
        Aliens = new List<AI>();
        for (int count = 0; count < GAliens.Length; ++count)
        {
            Aliens.Add(GAliens[count].GetComponent<NPC_AI>());
        }
        m_Roundsystem.RoundPrepare(Humans, Aliens, MoveCam, this);
        LRList = new List<GameObject>();
       
        RT = BelowButtonAndText.GetComponent<RectTransform>();
        Round = new System.Threading.Thread(m_Roundsystem.RoundStart);
    }


    private void Update()
    {
        TurnRun?.Invoke();//控制角色UI
        RunUI?.Invoke();//控制UI
    }
    private void LateUpdate()
    {
        if (UIAnima != null)
        {
            StartCoroutine(UIAnima());
            UIAnima = null;
        }
        if (m_HP_Bar.Count > 0)
        {
            foreach (createhp bar in m_HP_Bar)
            {
                if (bar.gameObject)
                {
                    bar.UpdateHP_Bar();
                }
            }
        }

    }




    public void onExitClicked()
    {
        Round.Abort();
        Application.Quit();
    }

    public void ShowActionUI()
    {
       // BelowButtonAndText.SetActive(true);
    }

    public void CloseActionUI()
    {
        //RT.anchoredPosition3D = new Vector3(0, -45, 0);
        //BelowButtonAndText.SetActive(false);
        RunUI = null;
    }



    public void MouseInTile(Tile T)
    {
        if (T.transform.rotation != Quaternion.Euler(90, 0, 0))
        {
            //變形
            MouseOnTile.transform.position = T.transform.position + Vector3.up * 0.06f;
            MouseOnTile.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            MouseOnTile.transform.position = T.transform.position + Vector3.up * 0.06f;
            MouseOnTile.GetComponent<Renderer>().enabled = true;
        }
        if (T.selectable && TurnCha.Moving != true)
        {
            var path = TurnCha.MoveToTile(T);
            DrawHeadingLine(path);
            Prepera = true;
        }

    }
    public void MouseOutTile(Tile T)
    {
        MouseOnTile.GetComponent<Renderer>().enabled = false;
        if (T.selectable)
        {
            Destroy(GLR);
            Prepera = false;
        }
    }









    //畫線
    public GameObject Blue;
    public GameObject Yellow;
    public List<GameObject> LRList;
    private GameObject GLR;
    public bool Prepera = false;

    public void PlayerStartTurn()
    {
        TurnCha.MoveRange();
        TurnCha.AttakeableDetect();
        TurnRun = null;
    }
    public void EnemyStartTurn()
    {
        TurnCha.FindSelectableTiles();
        TurnCha.ConfirmAction();
        TurnRun = null;
    }

    public void DrawMRLine(Queue<Tile> Process, GameObject LR,float ap)//畫移動範圍線
    {
        int Count = Process.Count;
        for (int i = 0; i < Count; ++i)
        {
            Tile T = Process.Dequeue();
            foreach (Tile AdjT in T.AdjList)
            {
                Vector3 div = AdjT.transform.position - T.transform.position;
                div.y = 0;

                if (div.magnitude > 0.9f|| !AdjT.walkable)
                {
                    continue;
                }
                if (AdjT.visited == true &&  AdjT.distance < TurnCha.Cha.Mobility * ap && TurnCha.Cha.Mobility * (ap-1)<AdjT.distance)
                {
                    int count = 0;
                    GameObject GLR = Instantiate(LR);
                    LineRenderer color = GLR.GetComponent<LineRenderer>();
                    color.positionCount = 2;
                    Vector3 middle = T.transform.position + div / 2 + Vector3.up * 0.05f;
                    if (T.transform.rotation != Quaternion.Euler(90, 0, 0))
                    {
                        if (TurnCha.FindDirection(div) % 2 == 0)
                        {
                            if (T.transform.rotation.eulerAngles.x - 90 != 0)
                            {
                                float angle = T.transform.rotation.eulerAngles.x - 90;
                                Vector3 Y = Vector3.up * 0.335f * Mathf.Tan(angle);
                                middle += Y;
                                color.SetPosition(count++, middle + Vector3.right * 0.335f);
                                color.SetPosition(count++, middle - Vector3.right * 0.335f);
                            }
                            else
                            {
                                float angle = T.transform.rotation.eulerAngles.z;
                                Vector3 Y = Vector3.up * 0.335f * Mathf.Tan(angle);
                                color.SetPosition(count++, middle + Vector3.right * 0.335f + Y);
                                color.SetPosition(count++, middle - Vector3.right * 0.335f - Y);
                            }
                        }
                        else
                        {
                            if (T.transform.rotation.eulerAngles.x - 90 != 0)
                            {
                                float angle = T.transform.rotation.eulerAngles.x - 90;
                                Vector3 Y = Vector3.up * 0.335f * Mathf.Tan(angle);
                                color.SetPosition(count++, middle + Vector3.forward * 0.335f + Y);
                                color.SetPosition(count++, middle - Vector3.forward * 0.335f - Y);
                            }
                            else
                            {
                                float angle = T.transform.rotation.eulerAngles.z;
                                Vector3 Y = Vector3.up * 0.335f * Mathf.Tan(angle);
                                middle += Y;
                                color.SetPosition(count++, middle + Vector3.forward * 0.335f);
                                color.SetPosition(count++, middle - Vector3.forward * 0.335f);
                            }
                        }
                    }
                    else
                    {
                        switch (TurnCha.FindDirection(div))
                        {
                            case 0:
                                color.SetPosition(count++, middle + Vector3.right * 0.335f);
                                color.SetPosition(count++, middle - Vector3.right * 0.335f);
                                break;
                            case 1:
                                color.SetPosition(count++, middle + Vector3.forward * 0.335f);
                                color.SetPosition(count++, middle - Vector3.forward * 0.335f);
                                break;
                            case 2:
                                color.SetPosition(count++, middle - Vector3.right * 0.335f);
                                color.SetPosition(count++, middle + Vector3.right * 0.335f);
                                break;
                            case 3:
                                color.SetPosition(count++, middle - Vector3.forward * 0.335f);
                                color.SetPosition(count++, middle + Vector3.forward * 0.335f);
                                break;
                        }
                    }
                    if (color.positionCount == 0)
                    {
                        Destroy(GLR);
                    }
                    else
                    {
                        LRList.Add(GLR);
                    }
                }
            }
            if (T.distance> TurnCha.Cha.Mobility * ap)
            {
                Process.Enqueue(T);
            }

        }
    }

    public void DrawHeadingLine(Stack<(Tile, AI.MoveWay)> path)//畫移動路徑線
    {
        GLR = Instantiate(Blue);
        LineRenderer LR = GLR.GetComponent<LineRenderer>();
        LR.positionCount = path.Count;
        int Count = 0;
        while (path.Count > 0)
        {
            (Tile, AI.MoveWay) T = path.Pop();
            LR.SetPosition(Count, T.Item1.transform.position + Vector3.up * 0.06f);
            ++Count;
        }
    }


    public void LRDestory(GameObject LR)//把線都清光
    {
         Destroy(LR);
    }
    public void LRDestory()
    {
        if (LRList.Count==0)
        {
            return;
        }
        foreach(GameObject GLR in LRList)
        {
            Destroy(GLR);
        }
        Destroy(GLR);
        LRList.Clear();
    }












    //底部UI

    public void PerAttatk()//button
    {
        if (TurnCha.AttakeableList.Count == 0)
        {
            return;
        }
        Target = TurnCha.AttakeableList.First;
        TurnCha.ChangePreAttakeIdle();
        TurnCha.ChaChangeTarget(Target.Value.Item1);
        //RT.anchoredPosition3D = new Vector3(0, 45, 0);
        ButtonText.text = "開火";
        DescribeText.text = "朝向目標開火。";
        LeftText.text = "傷害:" + TurnCha.Gun.Damage[0]+"~"+TurnCha.Gun.Damage[1];
        RightText.text = "命中率:" + Target.Value.Item3 + "%";
        RunUI = ChangeAttakeTarget;
    }
    public void Attack()//button
    {
        TurnCha.Fire(Target);
        RunUI = null;
    }

    void ChangeAttakeTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Target = Target.Next;
            if (Target == null)
            {
                Target = TurnCha.AttakeableList.First;
            }
            TurnCha.ChaChangeTarget(Target.Value.Item1);

            LeftText.text = "傷害:" + TurnCha.Gun.Damage[0] + "~" + TurnCha.Gun.Damage[1];
            RightText.text = "命中率:" + Target.Value.Item3 + "%";
        }
        if (Input.GetMouseButtonDown(1))
        {
            RT.anchoredPosition3D = new Vector3(0, -45, 0);
            TurnCha.PreAttack = false;
            TurnCha.Am.SetBool("Aim", false);
            RunUI = null;
        }
    }






    public  LinkedList<(AI Cha, int Speed)> Sequence;
    //時間軸及回合開始
    public delegate IEnumerator UIAnimation();
    public UIAnimation UIAnima;
    public TimeLine TLine;
    public int Count;
    public IEnumerator PrepareTimeLine()
    {
        var Cha = Sequence.First;
        for(int i = 0; i < Sequence.Count - 1; ++i)
        {
            Cha = Cha.Next;
            CreateHP_Bar(Cha.Value.Cha, Cha.Value.Cha.Cha.MaxHP);
        }
        Cha = Sequence.First;
        for (int i = 0; i < Sequence.Count-1; ++i)
        {
            Cha = Cha.Next;
            GameObject ChaLogo = Resources.Load<GameObject>(Cha.Value.Cha.name);
            TLine.NewLogo(Cha.Value.Cha, ChaLogo, i);
            yield return new WaitForSecondsRealtime(0.3f);
        }
        GameObject EndLogo = Resources.Load<GameObject>("End");
        TLine.NewLogo(Sequence.First.Value.Cha, EndLogo, Sequence.Count-1);
        Round.Start();
        yield return 0;
    }

    public void ChaTurnEnd()
    {
        TLine.TEndLogo(TurnCha, Count);
        TurnRun = null;
    }
    public void TurnEnd()
    {
        TLine.TEndLogo(GetComponent<AI>(), Sequence.Count - 1);
        TurnRun = null;
    }
    public void DeathKick(AI Cha)
    {
        TLine.DestoryLogo(Cha);
    }






    public GameObject BarPrefab;
    private List<createhp> m_HP_Bar = new List<createhp>();
    private Dictionary<AI, createhp> HPBarDic = new Dictionary<AI, createhp>();
    ///HP bar


    public void CreateHP_Bar(AI target, int HP)
    {
        GameObject go = GameObject.Instantiate(BarPrefab) as GameObject; //生成血條
        createhp bar = go.GetComponent<createhp>();
        bar.MaxHP = HP;
        bar.followedTarget = target.transform; //血條的位置 = 角色的位置
        go.transform.SetParent(transform);
        HPBarDic.Add(target, bar);
        m_HP_Bar.Add(bar); //放到列表中
    }

    public void HpControl(AI Target,int valve)
    {
        createhp bar;
        HPBarDic.TryGetValue(Target,out bar);
        bar.HPControl(valve);
    }
}
