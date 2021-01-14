using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    public GameObject menu;

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
            Humans.Add(GHumans[count].GetComponent<AI>());
        }
        GameObject[] GAliens = GameObject.FindGameObjectsWithTag("Alien");
        Aliens = new List<AI>();
        for (int count = 0; count < GAliens.Length; ++count)
        {
            Aliens.Add(GAliens[count].GetComponent<AI>());
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

        onEscapeKeyed();  //退出選單
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
        if (AimTarget.activeSelf)
        {
            Vector3 vScreenPos = Camera.main.WorldToScreenPoint(AimPos.position);
            AimTarget.transform.position = vScreenPos;
        }
    }

    //press Esc button
    public void onEscapeKeyed()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { menu.SetActive(!menu.activeInHierarchy); }
    }

    //menu buttons
    public void onExitClicked()
    {       
        if (menu.activeInHierarchy) { menu.SetActive(false);}
        else { menu.SetActive(true); }
    }

    public void QuitGame()
    {
        Round.Abort();
        Application.Quit();
    }

    public void ShowActionUI()
    {
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        BelowButtonAndText.SetActive(true);
        AttDectPanel.gameObject.SetActive(true);
        RunUI = null;
    }

    public void CloseActionUI()
    {
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        BelowButtonAndText.SetActive(false);
        AttPredictPanel.gameObject.SetActive(false);
        RunUI = null;
    }



    public void MouseInTile(Tile T)
    {
        //if (T.transform.rotation != Quaternion.Euler(90, 0, 0))
        //{
        //    //變形
        //    MouseOnTile.transform.position = T.transform.position + Vector3.up * 0.06f;
        //    MouseOnTile.GetComponent<Renderer>().enabled = true;
        //}
        //else
        //{
           MouseOnTile.transform.position = T.transform.position + Vector3.up * 0.06f;
           MouseOnTile.GetComponent<Renderer>().enabled = true;
        //}
        if (T.selectable && TurnCha.Moving != true)
        {
            ShowPredictAttable(T);
            var path = TurnCha.MoveToTile(T);
            DrawHeadingLine(path);
            Prepera = true;
        }

    }
    public void MouseOutTile(Tile T)
    {
        MouseOnTile.GetComponent<Renderer>().enabled = false;
        if (T.selectable && TurnCha.Moving != true)
        {
            DestroyAPPImage();
            Destroy(GLR);
            Prepera = false;
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonDown(1)&& Prepera)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "tile")
            {
                Tile T = hit.collider.GetComponent<Tile>();
                if (T.selectable)
                {
                    AttPredictPanel.gameObject.SetActive(false);
                    DestroyADPButton();
                    DestroySkillButton();
                    Prepera = false;
                    LRDestory();
                    TurnCha.PrepareMove(T);
                    RunUI = null;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            AttPredictPanel.gameObject.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            AttPredictPanel.gameObject.SetActive(false);
        }
    }




    
    public GameObject Blue;
    public GameObject Yellow;
    public List<GameObject> LRList;
    private GameObject GLR;
    public bool Prepera = false;
    public Transform AttDectPanel;
    public Transform AttPredictPanel;
    public Transform SkillsPanel;
    List<Button> ADPButtonList = new List<Button>();
    List<GameObject> APPImageList = new List<GameObject>();
    List<Button> SButtonList = new List<Button>();

    public void PlayerStartTurn()
    {
        TurnCha.MoveRange();
        TurnCha.AttakeableDetect();
        TurnCha.CountCD();
        ShowAttackableButton();
        FindSkillButton();
        RunUI = ShowActionUI;
        TurnRun = CheckMouse;
    }
    public void EnemyStartTurn()
    {
        TurnCha.CountCD();
        TurnCha.FindSelectableTiles();
        TurnCha.ConfirmAction();
        TurnRun = null;
    }
    public void FindSkillButton()
    {
        foreach(var skill in TurnCha.Skills)
        {
            AI target = null;
            if (TurnCha.AttakeableList.Count > 0)
            {
                target = TurnCha.AttakeableList.First.Value.Item1;
            }
            if(skill.CheckUseable(target))
            {
                Button go = Instantiate<GameObject>(Resources.Load<GameObject>(skill.Name)).GetComponent<Button>();
                go.transform.SetParent(SkillsPanel);
                SButtonList.Add(go);
                Type type = typeof(UISystem);
                MethodInfo method = type.GetMethod("Pre" + skill.Name);
                go.onClick.AddListener(() => method.Invoke(this, null));//todo CD
            }
        }
    }
    public void DestroySkillButton()
    {
        if (SButtonList.Count > 0)
        {
            foreach (var button in SButtonList)
            {
                Destroy(button.gameObject);
            }
        }
        SButtonList.Clear();
    }









    public void ShowAttackableButton()
    {
        if (TurnCha.AttakeableList.Count == 0)
        {
            return;
        }

        var Attackable = TurnCha.AttakeableList.First;
        while (Attackable != null)
        {
            Button button;
            if (Attackable.Value.Item1.tag == "Alien")
            {
                button = Instantiate<GameObject>(Resources.Load<GameObject>("AlianButton")).GetComponent<Button>();
            }
            else
            {
                button = Instantiate<GameObject>(Resources.Load<GameObject>("HumanButton")).GetComponent<Button>();
            }
            ADPButtonList.Add(button);
            button.transform.SetParent(AttDectPanel);
            AI ai = Attackable.Value.Item1;

            button.onClick.AddListener(() =>ChangeAttakeTargetButton(ai));
            Attackable = Attackable.Next;
        }     
    }
    public void DestroyADPButton()
    {
        if (ADPButtonList.Count > 0)
        {
            foreach (Button button in ADPButtonList)
            {
                Destroy(button.gameObject);
            }
        }
        ADPButtonList.Clear();
    }
    public void ShowPredictAttable(Tile T)
    {
        AttPred = TurnCha.AttackablePredict(T);
        foreach(AI ai in AttPred)
        {
            GameObject go;
            if (ai.tag == "Alien")
            {
                go = Instantiate<GameObject>(Resources.Load<GameObject>("AlianImage"));
            }
            else
            {
                go = Instantiate<GameObject>(Resources.Load<GameObject>("HumanImage"));
            }
            go.transform.SetParent(AttPredictPanel);
            APPImageList.Add(go);
        }
    }
    public void DestroyAPPImage()
    {
        AttPred.Clear();
        if (APPImageList.Count > 0)
        {
            foreach (GameObject go in APPImageList)
            {
                Destroy(go);
            }
        }
        APPImageList.Clear();
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


    public GameObject BelowButtonAndText;
    public RectTransform RT;
    public Text ButtonText;
    public Text DescribeText;
    public Text LeftText;
    public Text RightText;
    LinkedListNode<(AI, int, int)> Target;
    LinkedListNode<(AI, Tile)> MeleeTarget;
    LinkedListNode<AI> HealTarget;
    public GameObject MouseOnTile;
    public Canvas HPCanvas;
    List<AI> AttPred = new List<AI>();
    public GameObject AimTarget;
    private Transform AimPos;
    public Button ActionButton;





    //底部UI

    public void PreFire()//button
    {
        if (TurnCha.AttakeableList.Count == 0)
        {
            return;
        }
        Prepera = false;
        Target = TurnCha.AttakeableList.First;
        MoveCam.att_cam_bool = true;
        per_but = true;
        TurnCha.ChangePreAttackIdle();
        TurnCha.ChaChangeTarget(Target.Value.Item1);
        Target.Value.Item1.BeAim(TurnCha);
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        AimPos = Target.Value.Item1.BeAttakePoint;
        AimTarget.SetActive(true);
        ButtonText.text = "開火";
        DescribeText.text = "朝向目標開火。";
        LeftText.text = "傷害:" + TurnCha.Gun.Damage[0]+"~"+TurnCha.Gun.Damage[1];
        RightText.text = "命中率:" + Target.Value.Item3 + "%";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(()=>Fire());
        TurnRun = ChangeAttakeTarget;
    }
    public void Fire()//button
    {
        AimTarget.SetActive(false);
        TurnCha.Fire(Target.Value);
        DestroyADPButton();
        DestroySkillButton();
        TurnRun = null;
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
            MoveCam.att_cam_bool = true;
            AimPos = Target.Value.Item1.BeAttakePoint;
            TurnCha.ChaChangeTarget(Target.Value.Item1);
            LeftText.text = "傷害:" + TurnCha.Gun.Damage[0] + "~" + TurnCha.Gun.Damage[1];
            RightText.text = "命中率:" + Target.Value.Item3 + "%";
        }
        if (Input.GetMouseButtonDown(1))
        {
            AimTarget.SetActive(false);
            RT.anchoredPosition3D = new Vector3(0, 240, 0);
            TurnCha.PreAttack = false;
            TurnCha.Am.SetBool("Aim", false);
            TurnCha.Target = null;
            MoveCam.att_cam_bool = false;
            TurnRun = CheckMouse;
            //StartCoroutine(WaitMove());
            per_but = false;
        }
    }
    public void ChangeAttakeTargetButton(AI ai)
    {
        Target = TurnCha.AttakeableList.First;
        while (Target.Value.Item1 != ai)
        {
            Target = Target.Next;
        }
        AimPos = Target.Value.Item1.BeAttakePoint;
        TurnCha.ChaChangeTarget(Target.Value.Item1);
        LeftText.text = "傷害：" + TurnCha.Gun.Damage[0] + "~" + TurnCha.Gun.Damage[1];
        RightText.text = "命中率：" + Target.Value.Item3 + "%";
    }



    public void PreReload()
    {
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        Prepera = false;
        ButtonText.text = "換彈";
        DescribeText.text = "更換武器彈夾。";
        LeftText.text = "";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Reload());
        TurnRun = Canceal;
    }
    public void Reload()
    {
        TurnCha.Reload();
        DestroyADPButton();
        DestroySkillButton();
        TurnRun = null;
    }


    public void PreMelee()
    {
        if (TurnCha.MeleeableList.Count == 0)
        {
            return;
        }
        Prepera = false;
        AttDectPanel.gameObject.SetActive(false);
        foreach (var T in TurnCha.MeleeableList)
        {
            T.Item2.MeleePos();
        }
        MeleeTarget = TurnCha.MeleeableList.First;
        MeleeTarget.Value.Item2.ChoMeleePos();
        AimTarget.SetActive(true);
        AimPos = MeleeTarget.Value.Item1.BeAttakePoint;
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        ButtonText.text = "斬殺";
        DescribeText.text = "用長劍攻擊一名在你移動範圍的敵人。";
        LeftText.text = "傷害：3 ～ 4 ";
        RightText.text = "命中率：90 %";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Melee());
        TurnRun = ChangeMeleeTarget;
    }

    void ChangeMeleeTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            MeleeTarget.Value.Item2.MeleePos();
            MeleeTarget = MeleeTarget.Next;
            if (MeleeTarget == null)
            {
                MeleeTarget = TurnCha.MeleeableList.First;
            }
            MeleeTarget.Value.Item2.ChoMeleePos();
            AimPos = MeleeTarget.Value.Item1.BeAttakePoint;
        }
        //ifcheckmouse
        if (Input.GetMouseButtonDown(1))
        {
            foreach (var T in TurnCha.MeleeableList)
            {
                T.Item2.Recover();
            }
            AimTarget.SetActive(false);
            RT.anchoredPosition3D = new Vector3(0, 240, 0);
            AttDectPanel.gameObject.SetActive(true);
            TurnRun = CheckMouse;
        }
    }

    public void Melee()
    {
        AimTarget.SetActive(false);
        TurnCha.PreMelee(MeleeTarget);
        DestroyADPButton();
        DestroySkillButton();
        TurnRun = null;
    }


    public void PreHeal()//button
    {
        if (TurnCha.HealList.Count == 0)
        {
            return;
        }
        Prepera = false;
        HealTarget = TurnCha.HealList.First;
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        //UI
        ButtonText.text = "治療";
        DescribeText.text = "快速治療一名友軍。";
        LeftText.text = "治癒：3";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Heal());
        TurnRun = ChangeHealTarget;
    }

    private void ChangeHealTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HealTarget = HealTarget.Next;
            if (HealTarget == null)
            {
                HealTarget = TurnCha.HealList.First;
            }
        }
        Canceal();
    }

    public void Heal()//button
    {
        TurnCha.PreHeal(HealTarget.Value);
        DestroyADPButton();
        DestroySkillButton();
        TurnRun = null;
    }

    public void PreCooperation()
    {
        if (m_Roundsystem.Humans.Count < 1)
        {
            return;
        }
        Prepera = false;
        
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        //UI
        ButtonText.text = "合作";
        DescribeText.text = "提供資訊，使隊友獲得一個行動點。";
        LeftText.text = "";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Cooperation());
        TurnRun = ChangeAllyTarget;
    }

    private void ChangeAllyTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HealTarget = HealTarget.Next;
            if (HealTarget == null)
            {
                HealTarget = TurnCha.HealList.First;
            }
        }
        Canceal();
    }


    private void Cooperation()
    {

    }




    private void Canceal()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RT.anchoredPosition3D = new Vector3(0, 240, 0);
            AttDectPanel.gameObject.SetActive(true);
            TurnRun = CheckMouse;
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
            CreateHP_Bar(Cha.Value.Cha, Cha.Value.Cha.Cha.MaxHP, Cha.Value.Cha.Cha.HP);
            Cha.Value.Cha.UI = this;
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
        DestroyADPButton();
        TimeLine.Instance.Moved = false;
        TLine.TEndLogo(TurnCha, Count);
        TurnRun = null;
    }
    public void TurnEnd()
    {
        TimeLine.Instance.Moved = false;
        TLine.TEndLogo(GetComponent<AI>(), Sequence.Count - 1);
        TurnRun = null;
    }
    public void DeathKick(AI Cha)
    {
        DestroyHPBar(Cha);
        TLine.DestoryLogo(Cha);
        if (!Aliens.Remove(Cha))
        {
            Humans.Remove(Cha);
        }
    }
    public void ChangeLogo(AI Cha)
    {
        TLine.ChangeLogo(Cha);
    }





    public GameObject HPBar_Human;
    public GameObject HPBar_Alian;
    private List<createhp> m_HP_Bar = new List<createhp>();
    private Dictionary<AI, createhp> HPBarDic = new Dictionary<AI, createhp>();
    ///HP bar


    public void CreateHP_Bar(AI target, int MaxHP,int HP)
    {
        GameObject Bar;
        if (target.Cha.camp == Character.Camp.Human)
        {
            Bar = GameObject.Instantiate(HPBar_Human) as GameObject; //生成血條
        }
        else
        {
            Bar = GameObject.Instantiate(HPBar_Alian) as GameObject; //生成血條
        }
        createhp bar = Bar.GetComponent<createhp>();
        bar.MaxHP = MaxHP;
        bar.HP = HP;
        bar.followedTarget = target.transform; //血條的位置 = 角色的位置
        Bar.transform.SetParent(HPCanvas.transform);
        HPBarDic.Add(target, bar);
        m_HP_Bar.Add(bar); //放到列表中
    }

    public void HpControl(AI Target,int valve)
    {
        createhp bar;
        HPBarDic.TryGetValue(Target,out bar);
        bar.HPControl(valve);
    }
    public void DestroyHPBar(AI Cha)
    {
        createhp HPBar;
        HPBarDic.TryGetValue(Cha, out HPBar);
        HPBarDic.Remove(Cha);
        m_HP_Bar.Remove(HPBar);
        Destroy(HPBar.gameObject);
    }

    public bool per_but;
    public GameObject[] camera_point;
    float[] cam_dir;
    float max_dis;
    bool change_point;

    public void Attack_camera()
    {
        Vector3 Target_position; //目標點
        //Vector3 sce_cam_pos = MoveCam.scene_camera.transform.position; //攝影機位置
        cam_dir = new float[9];
        cam_dir[0] = 0.0f;
        max_dis = 0;
        if (TurnCha.Cha.tag == "Human" && per_but == true)
        {
            float dis = Vector3.Distance(TurnCha.Cha.transform.position, Target.Value.Item1.transform.position); //與目標的距離
            Vector3 dir = (Target.Value.Item1.transform.position - TurnCha.Cha.transform.position).normalized; //到目標的方向
            Target_position = TurnCha.Cha.transform.position + dir * dis / 2 + new Vector3(0, 1.2f, 0f); //目標點位置
            if (dis > 15f)
            {
                MoveCam.cam_dis = 25f; //攝影機位置往後移動到25
                MoveCam.transform.position = Vector3.Lerp(MoveCam.transform.position, Target_position, 5 * Time.deltaTime);//標的物往目標點移動
                float Gg = Vector3.Distance(MoveCam.transform.position, Target_position);//如果movecam與目標點距離小於0.02 位置直接等於目標點
                if (Gg <= 0.05)
                    MoveCam.transform.position = Target_position;
                Vector3 scp = MoveCam.transform.position + -MoveCam.scene_camera.transform.forward * MoveCam.cam_dis;//攝影機位置往後
                MoveCam.scene_camera.transform.position = Vector3.Lerp(scp, MoveCam.scene_camera.transform.position, 3f * Time.deltaTime); //攝影機滑順移動到指定距離
                float mg = Vector3.Distance(MoveCam.scene_camera.transform.position, scp);
                if (mg <= 0.05)
                {
                    MoveCam.scene_camera.transform.position = scp;
                }
            }
            else
            {
                camera_point[0].transform.position = TurnCha.Cha.transform.position + new Vector3(0f, 1f, 0f); //攝影機八個位置
                for (int i = 1; i < camera_point.Length; i++) //0為父物件 分別判斷1~8到目標點的距離
                {
                    cam_dir[i] = Vector3.Distance(camera_point[i].transform.position, Target_position);
                }
                for (int i = 1; i < cam_dir.Length; i++) //比大小 判斷1~8
                {
                    if (max_dis < cam_dir[i]) max_dis = cam_dir[i];
                }
                int f = Array.IndexOf(cam_dir, max_dis);
                if (f != -1)
                {
                    RaycastHit hit;
                    Vector3 eni_dir = Target_position - camera_point[f].transform.position;
                    MoveCam.transform.position = Vector3.Lerp(MoveCam.transform.position, Target_position, 5 * Time.deltaTime); //標的物到雙方中間
                    Debug.DrawRay(camera_point[f].transform.position, eni_dir);
                    if (Physics.Raycast(camera_point[f].transform.position, eni_dir, out hit, 1.5f))
                    {
                        if (f == 1) f = 8;
                        Debug.DrawRay(camera_point[f - 1].transform.position, Target_position - camera_point[f - 1].transform.position);
                        if (Physics.Raycast(camera_point[f - 1].transform.position, Target_position - camera_point[f - 1].transform.position, out hit, 1.5f))
                        {
                            if (f == 8) f = 1;
                            MoveCam.scene_camera.transform.position = Vector3.Lerp(MoveCam.scene_camera.transform.position, camera_point[f + 1].transform.position, 5 * Time.deltaTime);
                        }
                        else
                            MoveCam.scene_camera.transform.position = Vector3.Lerp(MoveCam.scene_camera.transform.position, camera_point[f - 1].transform.position, 5 * Time.deltaTime);
                    }
                    else
                        MoveCam.scene_camera.transform.position = Vector3.Lerp(MoveCam.scene_camera.transform.position, camera_point[f].transform.position, 5 * Time.deltaTime);
                    MoveCam.scene_camera.transform.LookAt(Target_position);
                }
            }
        }

        if (TurnCha.Cha.tag == "Alien" && TurnCha.NPC_Prefire == true)
        {
            MoveCam.cam_dis = 25f; //攝影機位置往後移動到25
            float dis = Vector3.Distance(TurnCha.Cha.transform.position, TurnCha.Target.transform.position); //與目標的距離
            Vector3 dir = (TurnCha.Target.transform.position - TurnCha.Cha.transform.position).normalized; //到目標的方向
            Target_position = TurnCha.Cha.transform.position + dir * dis / 2 + new Vector3(0, 1.2f, 0f); //目標位置

            MoveCam.transform.position = Vector3.Lerp(MoveCam.transform.position, Target_position, 3 * Time.deltaTime);//攝影機往前目標移動
            float Gg = Vector3.Distance(MoveCam.transform.position, Target_position);

            if (Gg <= 0.05)
            {
                MoveCam.transform.position = Target_position;
            }
            Vector3 scp = MoveCam.transform.position + -MoveCam.scene_camera.transform.forward * MoveCam.cam_dis;//攝影機為標的物加往後一個方向的距離                                                                                                                  
            MoveCam.scene_camera.transform.position = Vector3.Lerp(scp, MoveCam.scene_camera.transform.position, 5 * Time.deltaTime);
            float mg = Vector3.Distance(MoveCam.scene_camera.transform.position, scp);
            if (mg <= 0.05)
            {
                MoveCam.scene_camera.transform.position = scp;
            }
        }
    }

    public bool Bomb_start;
    public int Bomb_Round;
    //public Text fr;
    public Text final_text;
    public GameObject[] toggle;
    public GameObject explosion;
    public Sprite[] mission_Images;
    public void Bomb_button()
    {
        Bomb_start = true;
        Debug.Log("安裝炸彈");
        toggle[1].SetActive(true);
        toggle[0].transform.GetChild(1).GetComponent<Text>().color = Color.green;
        toggle[0].transform.GetChild(0).GetComponent<Image>().sprite = mission_Images[1];
        explosion.SetActive(true);
    }
}
