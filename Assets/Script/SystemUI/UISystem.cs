using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class UISystem : MonoBehaviour
{
    public Move_Camera MoveCam;
    List<AI> Humans, Aliens;
    public AI TurnCha;
    RoundSysytem m_Roundsystem;
    internal Thread Round;
    public Action TurnRun;
    public Action RunUI;
    public GameObject menu;
    public GameObject menuOption;    
    public Slider Volume;
    public GameObject menuCheck;
    public GameObject missionDialogue;
    public GameObject menuQuit;
    public GameObject menu_success;
    public Tile[] StartTile;
    public bool acting = false;
    SoundManager SoundM;

    public List<Tile> LeaveTile = new List<Tile>();
    public List<MeshRenderer> ActionTile = new List<MeshRenderer>();

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
        Time.timeScale = 1;
        m_Roundsystem = RoundSysytem.GetInstance();
        GameObject[] GHumans = GameObject.FindGameObjectsWithTag("Human");
        Humans = new List<AI>();
        for(int count = 0; count< GHumans.Length; ++count)
        {
            AI ai = GHumans[count].GetComponent<AI>();
            if (ai == null)
                continue;
            Humans.Add(ai);
        }
        GameObject[] GAliens = GameObject.FindGameObjectsWithTag("Alien");
        Aliens = new List<AI>();
        for (int count = 0; count < GAliens.Length; ++count)
        {
            AI ai = GAliens[count].GetComponent<AI>();
            if (ai == null)
                continue;
            Aliens.Add(ai);
        }
        m_Roundsystem.RoundPrepare(Humans, Aliens, MoveCam, this);
        LRList = new List<GameObject>();
       
        RT = BelowButtonAndText.GetComponent<RectTransform>();
        JoinActionTile(BombSite);
        Round = new Thread(m_Roundsystem.RoundStart);
        Round.IsBackground = true;
        SoundM = FindObjectOfType<SoundManager>();
    }

    public GameObject TimeLine_First;
    private void Update()
    {
        TurnRun?.Invoke();//控制角色UI
        RunUI?.Invoke();//控制UI
        onEscapeKeyed();  //退出選單

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    SoundM.Play(test);
        //}
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cheat();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if(TLine.Moved)
                TurnChaSkip();
        }
        if (ActionTile.Count > 0)
        {
            UpdateActionTile();
        }
        if (TurnCha!=null)
        {
            UpdateTurnChaLogo();
        }
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
        //if (Input.GetKeyDown(KeyCode.Escape) && missionDialogue.activeInHierarchy) { menu.SetActive(true); missionDialogue.SetActive(false); }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(!menu.activeInHierarchy );
            if (menu.activeInHierarchy) PauseGame();
            else
                Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && menuOption.activeInHierarchy) 
        { 
            menu.SetActive(true);
            if (!menu.activeInHierarchy) PauseGame();
            else
                Time.timeScale = 1; 
            menuOption.SetActive(false); 
        }
        if (Input.GetKeyDown(KeyCode.Escape) && menuCheck.activeInHierarchy) 
        { 
            menu.SetActive(true); 
            if (menu.activeInHierarchy == true) PauseGame(); else  Time.timeScale = 1; menuCheck.SetActive(false);  
        }
        if (Input.GetKeyDown(KeyCode.Escape) && menu_success.activeInHierarchy) {menu.SetActive(false); menu_success.SetActive(true);}
    }
    //menu buttons
    public void onExitClicked()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
    }
    public void QuitGame()
    {
        Round.Abort();
        Application.Quit();
    }

   public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ReloadScene(string s)
    {
        Round.Abort();
        CSceneManager.m_Instance.change_scene(s);
    }

    public void ShowActionUI()
    {
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        BelowButtonAndText.SetActive(true);
        AttDectPanel.gameObject.SetActive(true);
        LRs.gameObject.SetActive(true);
        bulnum();
        RunUI = null;
    }

    public void CloseActionUI()
    {
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        BelowButtonAndText.SetActive(false);
        AttPredictPanel.gameObject.SetActive(false);
        bulletnumber.SetActive(false);
        RunUI = null;
    }

    float AlphaValve=0.7f;
    float AdjustValve = 0.25f;
    private void UpdateActionTile()
    {
        if (AlphaValve > 0.7f)
        {
            AdjustValve = -0.25f;
        }
        else if (AlphaValve < 0.2f)
        {
            AdjustValve = 0.25f;
        }
        AlphaValve += AdjustValve * Time.deltaTime;
        for(int i = 0; i < ActionTile.Count; ++i)
        {
            Color color = ActionTile[i].material.color;
            color.a = AlphaValve;
            ActionTile[i].material.color = color;
        }
    }
    public void JoinActionTile(Tile T)
    {
        ActionTile.Add(T.GetComponent<MeshRenderer>());
    }
    public void JoinActionTile(GameObject T)
    {
        ActionTile.Add(T.GetComponent<MeshRenderer>());
    }
    public void LeaveActionTile(Tile T)
    {
        ActionTile.Remove(T.GetComponent<MeshRenderer>());
        T.Recover();
    }



    public void MouseInTile(Tile T)
    {
        MouseOnTile.transform.position = T.transform.position + Vector3.up * 0.1f;

        if (Grenaded && (T.transform.position - TurnCha.transform.position).magnitude < 12f)
        {
            DangerTile.SetActive(true);
            DangerTile.transform.position = T.transform.position + Vector3.up * 0.1f;
        }
        else if (T.selectable && TurnCha.Moving != true)
        {
            ShowPredictAttable(T);
            var path = TurnCha.MoveToTile(T);
            if (T.distance > TurnCha.Cha.Mobility * (TurnCha.AP - 1))
            {
                DrawHeadingLine(path, Yellow);
            }
            else
            {
                DrawHeadingLine(path, Blue);
            }
            Prepera = true;
        }
        if (!T.walkable)
        {
            for (int i = 0; i < 4; ++i)
            {
                MouseOnTile.transform.GetChild(i).GetComponent<MeshRenderer>().material = Resources.Load<Material>("NoCover");
            }
        }
        else
        {
            for (int i = 0; i < 4; ++i)
            {
                if (T.AdjCoverList[i] == Tile.Cover.FullC)
                {
                    MouseOnTile.transform.GetChild(i).GetComponent<MeshRenderer>().material = Resources.Load<Material>("FullCover");
                }
                else if (T.AdjCoverList[i] == Tile.Cover.HalfC)
                {
                    MouseOnTile.transform.GetChild(i).GetComponent<MeshRenderer>().material = Resources.Load<Material>("HalfCover");
                }
                else
                {
                    MouseOnTile.transform.GetChild(i).GetComponent<MeshRenderer>().material = Resources.Load<Material>("NoCover");
                }
            }
        }
    
    }
    public void MouseOutTile(Tile T)
    {

        //MouseOnTile.GetComponent<Renderer>().enabled = false;
        if (Grenaded)
        {
            DangerTile.SetActive(false);
        }
        if (T.selectable && TurnCha.Moving != true)
        {
            DestroyAPPImage();
            Destroy(GLR);
            Prepera = false;
        }
    }


    public Transform TurnChaArrow;
    public MeshRenderer TurnChaLogo;
    float TCL_Time;
    int i;
    void UpdateTurnChaLogo()
    {
        TurnChaLogo.transform.position = TurnCha.transform.position + Vector3.up * 0.05f;
        TCL_Time += Time.deltaTime;
        if (TCL_Time >= 1)
        {
            TCL_Time = 0;
            ++i;
            if (i > 2) i = 0;
            TurnChaLogo.material = Resources.Load<Material>(TurnCha.Cha.camp.ToString()+"Star"+i);
        }
    }




    public void CheckMouse2(Tile T)
    {
        if (Prepera)
        {
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





    void CheckMouse()
    {
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
    public Transform LRs;
    private GameObject GLR;
    public bool Prepera = false;
    public Transform AttDectPanel;
    public Transform AttPredictPanel;
    public Transform SkillsPanel;
    List<Button> ADPButtonList = new List<Button>();
    List<GameObject> APPImageList = new List<GameObject>();
    List<Button> SButtonList = new List<Button>();
    AI TrueTurnCha = null;
    public Transform Frame;
    Action SkillCanceal;

    public void PlayerStartTurn()
    {
        TurnChaLogo.transform.position = TurnCha.transform.position + Vector3.up * 0.05f;

        TurnChaLogo.gameObject.SetActive(true);
        TurnChaArrow.transform.position = TurnCha.transform.position + Vector3.up * 3f;
        TurnChaArrow.transform.SetParent(TurnCha.transform);
        TurnChaArrow.gameObject.SetActive(true);
        if (TurnCha.Coma)
        {
            TurnCha.WakeUp();
            MoveCam.ChaTurn(TurnCha);
            StartCoroutine(TurnCha.WaitEndturn());
            TurnRun = null;
        }
        else if (TurnCha.Cha.camp == Character.Camp.Human)
        {
            TurnCha.MoveRange();
            TurnCha.AttakeableDetect();
            ShowAttackableButton();
            FindSkillButton();
            RunUI = ShowActionUI;
            TurnRun = CheckMouse;
            MoveCam.ChaTurn(TurnCha);
        }
        else
        {
            TurnCha.FindSelectableTiles(2);
            TurnCha.ConfirmAction();
            MoveCam.ChaTurn(TurnCha);
            TurnRun = null;
        }
    }
    private void TurnChaSkip()
    {
        LRDestory();
        DestroyADPButton();
        DestroySkillButton();
        TurnCha.Skip();
    }
    private void Cheat()
    {
        LRDestory();
        DestroyADPButton();
        DestroySkillButton();
        TurnCha.Skip();
        m_Roundsystem.EndChecked = true;
        EndCheck.GetInstance().ChaEnd = true;
        TLine.Moved = true;
    }




    public void FindSkillButton()
    {
        SkillCanceal = null;
        foreach (var skill in TurnCha.Skills)
        {
            AI target = null;
            if (TurnCha.AttakeableList.Count > 0)
            {
                target = TurnCha.AttakeableList.First.Value.Item1;
            }
            if (skill.type == 0)
            {
                Button go = Instantiate<GameObject>(Resources.Load<GameObject>(skill.Name)).GetComponent<Button>();
                go.transform.SetParent(SkillsPanel);
                SButtonList.Add(go);
                if (skill.CheckUseable(target))
                {
                    go.onClick.AddListener(() => { SkillCanceal?.Invoke(); skill.GetAction().Invoke(); });//todo CD
                }
                else
                {
                    go.interactable = false;
                    if (skill.CDCount > 0)
                    {
                        go.transform.GetChild(0).GetComponent<Text>().text = "T-" + skill.CDCount;
                    }
                }
            }
            else
            {
                if (skill.CheckUseable(target))
                {
                    Button go = Instantiate<GameObject>(Resources.Load<GameObject>(skill.Name)).GetComponent<Button>();
                    go.transform.SetParent(SkillsPanel);
                    SButtonList.Add(go);
                    go.onClick.AddListener(() => { SkillCanceal?.Invoke(); skill.GetAction().Invoke(); });//todo CD
                }
            }
        }
        Button SB = Instantiate<GameObject>(Resources.Load<GameObject>("Standby")).GetComponent<Button>();
        SB.transform.SetParent(SkillsPanel);
        SButtonList.Add(SB);
        SB.onClick.AddListener(() => { SkillCanceal?.Invoke(); PreStandby(); });//todo CD
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

            button.onClick.AddListener(() => { ChangeAttakeTargetButton(ai);Frame.SetParent(button.transform); });
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
        AttPred.Clear();
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
                    Vector3 middle = T.transform.position + div / 2 + Vector3.up * 0.1f;
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
                        GLR.transform.SetParent(LRs);
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

    public void DrawHeadingLine(Stack<Tile> path,GameObject Color)//畫移動路徑線
    {
        GLR = Instantiate(Color);
        LineRenderer LR = GLR.GetComponent<LineRenderer>();
        LR.positionCount = path.Count;
        int Count = 0;
        while (path.Count > 0)
        {
            Tile T = path.Pop();
            LR.SetPosition(Count, T.transform.position + Vector3.up * 0.06f);
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
    AI AllyTarget;
    int Index;



    //底部UI

    public void PreFire()//button
    {
        if (TurnCha.AttakeableList.Count == 0||TurnCha.AmTurn)
        {
            return;
        }
        SoundM.Play("Test");
        LRs.gameObject.SetActive(false);
        acting = true;
        Prepera = false;
        Target = TurnCha.AttakeableList.First;
        MoveCam.PlayerSetAtkCam(Target.Value.Item1);
        TurnCha.ChangePreAttackIdle();
        TurnCha.ChaChangeTarget(Target.Value.Item1);
        Index = 0;
        Frame = Instantiate<GameObject>(Resources.Load<GameObject>("Frame")).transform;
        Frame.SetParent(ADPButtonList[Index].transform);
        Frame.localPosition = Vector3.zero;

        Target.Value.Item1.BeAim(TurnCha);
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        AimPos = Target.Value.Item1.BeAttakePoint;
        AimTarget.SetActive(true);
        AimTarget.transform.GetChild(0).GetComponent<Text>().text= Target.Value.Item3 + "%";
        ButtonText.text = "開火";
        DescribeText.text = "朝向目標開火。";
        LeftText.text = "傷害:" + TurnCha.Gun.Damage[0]+"~"+TurnCha.Gun.Damage[1];
        RightText.text = "命中率:" + Target.Value.Item3 + "%";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(()=>Fire());
        TurnRun = ChangeAttakeTarget;
        SkillCanceal = CancealFire;
    }
    public void Fire()//button
    {
        if (TurnCha.AmTurn == true&& TurnCha.ChangeTarget)
        {
            return;
        }
        AimTarget.SetActive(false);
        AimTarget.transform.GetChild(0).GetComponent<Text>().text = "";
        TurnCha.Fire(Target.Value);
        DestroyADPButton();
        DestroySkillButton();
        CloseActionUI();
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
            ++Index;
            if (Index >= ADPButtonList.Count)
            {
                Index = 0;
            }
            MoveCam.PlayerSetAtkCam(Target.Value.Item1);
            AimTarget.transform.GetChild(0).GetComponent<Text>().text = Target.Value.Item3 + "%";
            Frame.SetParent(ADPButtonList[Index].transform);
            Frame.localPosition = Vector3.zero;
            Target.Value.Item1.BeAim(TurnCha);
            AimPos = Target.Value.Item1.BeAttakePoint;
            TurnCha.ChaChangeTarget(Target.Value.Item1);
            LeftText.text = "傷害:" + TurnCha.Gun.Damage[0] + "~" + TurnCha.Gun.Damage[1];
            RightText.text = "命中率:" + Target.Value.Item3 + "%";
        }
        if (Input.GetMouseButtonDown(1))
        {
            CancealFire();
        }
    }
    void CancealFire()
    {
        AimTarget.transform.GetChild(0).GetComponent<Text>().text = "";
        AimTarget.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        TurnCha.PreAttack = false;
        TurnCha.Am.SetBool("Aim", false);
        TurnCha.Target = null;
        MoveCam.EndCam();

        TurnRun = CheckMouse;
        Destroy(Frame.gameObject);
        acting = false;
        LRs.gameObject.SetActive(true);
        SkillCanceal = null;
    }

    public void ChangeAttakeTargetButton(AI ai)
    {
        Target = TurnCha.AttakeableList.First;
        while (Target.Value.Item1 != ai)
        {
            Target = Target.Next;
        }
        MoveCam.PlayerSetAtkCam(Target.Value.Item1);
        if(Frame!=null)
            Destroy(Frame.gameObject);
        AimPos = Target.Value.Item1.BeAttakePoint;
        TurnCha.ChaChangeTarget(Target.Value.Item1);
        LeftText.text = "傷害：" + TurnCha.Gun.Damage[0] + "~" + TurnCha.Gun.Damage[1];
        RightText.text = "命中率：" + Target.Value.Item3 + "%";
    }


    public void PreReload()
    {
        SoundM.Play("Test");
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        Prepera = false;
        ButtonText.text = "換彈";
        DescribeText.text = "更換武器彈夾。";
        LeftText.text = "";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Reload());
        TurnRun = Canceal;
        SkillCanceal = CancealSkill;
    }
    public void Reload()
    {
        TurnCha.Reload();
        DestroyADPButton();
        DestroySkillButton();
        CloseActionUI();
        TurnRun = null;
    }


    public void PreMelee()
    {

        if (TurnCha.MeleeableList.Count == 0)
        {
            return;
        }
        SoundM.Play("Test");
        Prepera = false;
        AttDectPanel.gameObject.SetActive(false);
        foreach (var T in TurnCha.MeleeableList)
        {
            T.Item2.MeleePos();
            JoinActionTile(T.Item2);
        }
        MeleeTarget = TurnCha.MeleeableList.First;
        MeleeTarget.Value.Item2.ChoMeleePos();
        MouseInTile(MeleeTarget.Value.Item2);
        MoveCam.ChaTurn(MeleeTarget.Value.Item1);
        acting = true;
        AimTarget.SetActive(true);
        AimTarget.transform.GetChild(0).GetComponent<Text>().text ="90%";
        AimPos = MeleeTarget.Value.Item1.BeAttakePoint;
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        ButtonText.text = "斬殺";
        DescribeText.text = "用長劍攻擊一名在你移動範圍的敵人。";
        LeftText.text = "傷害： 4 ";
        RightText.text = "命中率：90 %";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Melee());
        TurnRun = ChangeMeleeTarget;
        SkillCanceal = CancealMelee;
    }

    void ChangeMeleeTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            MeleeTarget.Value.Item2.MeleePos();
            MouseOutTile(MeleeTarget.Value.Item2);
            MeleeTarget = MeleeTarget.Next;
            if (MeleeTarget == null)
            {
                MeleeTarget = TurnCha.MeleeableList.First;
            }
            MeleeTarget.Value.Item2.ChoMeleePos();
            MouseInTile(MeleeTarget.Value.Item2);
            MoveCam.ChaTurn(MeleeTarget.Value.Item1);
            AimPos = MeleeTarget.Value.Item1.BeAttakePoint;
        }
        //ifcheckmouse
        if (Input.GetMouseButtonDown(1))
        {
            CancealMelee();
        }
    }

    public void Melee()
    {
        var current = TurnCha.MeleeableList.First;
        for (int i = 0; i< TurnCha.MeleeableList.Count; ++i)
        {
            var T = current.Value;
            LeaveActionTile(T.Item2);
            current = current.Next;
        }
        acting = false;
        AimTarget.SetActive(false);
        TurnCha.PreMelee(MeleeTarget);
        MoveCam.ChaTurn(TurnCha);
        DestroyADPButton();
        DestroySkillButton();
        CloseActionUI();
        TurnRun = null;
    }
    public void ClickMelee(Tile T)
    {
        Debug.Log(T.GetComponent<Renderer>().material.ToString());
        if(T.GetComponent<Renderer>().material.ToString()== "ChoMeleePos (Instance) (UnityEngine.Material)")
        {
            Melee();
        }
        else
        {
            MeleeTarget.Value.Item2.MeleePos();
            MouseOutTile(MeleeTarget.Value.Item2);


            MeleeTarget= TurnCha.MeleeableList.First;
            while (MeleeTarget.Value.Item2 != T)
            {
                MeleeTarget = MeleeTarget.Next;
            }
            
            MeleeTarget.Value.Item2.ChoMeleePos();
            MouseInTile(MeleeTarget.Value.Item2);
            MoveCam.ChaTurn(MeleeTarget.Value.Item1);
            AimPos = MeleeTarget.Value.Item1.BeAttakePoint;
        }
    }

    void CancealMelee()
    {
        foreach (var T in TurnCha.MeleeableList)
        {
            LeaveActionTile(T.Item2);
        }
        MouseOutTile(MeleeTarget.Value.Item2);
        acting = false ;
        AimTarget.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        AttDectPanel.gameObject.SetActive(true);
        TurnRun = CheckMouse;
        MoveCam.ChaTurn(TurnCha);
        SkillCanceal = null;
    }

    public void PreHeal()//button
    {
        if (TurnCha.HealList.Count == 0)
        {
            return;
        }
        SoundM.Play("Test");
        Prepera = false;
        HealTarget = TurnCha.HealList.First;
        MoveCam.ChaTurn(HealTarget.Value);
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        //UI
        ButtonText.text = "治療";
        DescribeText.text = "快速治療一名友軍。";
        LeftText.text = "治癒：3";
        RightText.text = "CD：2回合";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Heal());
        TurnRun = ChangeHealTarget;
        SkillCanceal = CancealSkill;
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
            MoveCam.ChaTurn(HealTarget.Value);
        }
        Canceal();
    }

    public void Heal()//button
    {
        TurnCha.PreHeal(HealTarget.Value);
        DestroyADPButton();
        DestroySkillButton();
        LRDestory();
        CloseActionUI();
        TurnRun = null;
    }


    public void PreCooperation()
    {
        if (Humans.Count < 2)
        {
            return;
        }
        SoundM.Play("Test");
        Prepera = false;
        Index= 0;
        AllyTarget = Humans[Index];
        if (Humans[Index] == TurnCha)
        {
            ++Index;
        }
        AllyTarget = Humans[Index];
        MoveCam.ChaTurn(AllyTarget);
        TurnChaArrow.transform.position = AllyTarget.transform.position + Vector3.up * 3f;
        TurnChaArrow.transform.SetParent(AllyTarget.transform);
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        //UI
        ButtonText.text = "指揮";
        DescribeText.text = "指揮隊友，使隊友獲得一個行動點。";
        LeftText.text = "";
        RightText.text = "CD：2回合";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Cooperation());
        TurnRun = ChangeAllyTarget;
        SkillCanceal = CancealSkill;
    }

    private void ChangeAllyTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ++Index;
            if (Index>Humans.Count-1)
            {
                Index = 0;

            }
            if (Humans[Index] == TurnCha)
            {
                ++Index;
                if (Index > Humans.Count - 1)
                {
                    Index = 0;

                }
            }
            AllyTarget = Humans[Index];
            MoveCam.ChaTurn(AllyTarget);
            TurnChaArrow.transform.position = AllyTarget.transform.position + Vector3.up * 3f;
            TurnChaArrow.transform.SetParent(AllyTarget.transform);
        }
        if (Input.GetMouseButtonDown(1))
        {
            CancealSkill();
            TurnChaArrow.transform.position = TurnCha.transform.position + Vector3.up * 3f;
            TurnChaArrow.transform.SetParent(TurnCha.transform);
        }
    }


    private void Cooperation()
    {
        AttPredictPanel.gameObject.SetActive(false);
        DestroyADPButton();
        DestroySkillButton();
        LRDestory();
        MoveCam.ChaTurn(TurnCha);
        TrueTurnCha = TurnCha;
        TurnCha.PreCooperation(AllyTarget);
        m_Roundsystem.EndChecked = false;
        CloseActionUI();
        TurnRun = null;
    }

    //每回合檢查dra

    public GameObject CAM;
    public GameObject CAM_TIMELINE;
    public bool Escape = false;
    public ThemePlayer themePlayer;
    public void CheckEvent()
    {
        CloseActionUI();
        TurnChaLogo.gameObject.SetActive(false);
        TurnChaArrow.SetParent(null);
        TurnChaArrow.gameObject.SetActive(false);
        MoveCam.EndCam();
        acting = false;

        if (TrueTurnCha != null)//指揮技能有關，可無視
        {
            TurnCha = TrueTurnCha;
            TrueTurnCha = null;
            lock (EndCheck.GetInstance())
            {
                EndCheck.GetInstance().ChaEnd = false;
            }
            m_Roundsystem.EndChecked = true;
            if (TurnCha.AP > 0)
            {
                PlayerStartTurn();
            }
            else
            {
                TurnCha.EndTurn();
            }
        }
        if (Humans.Count == 0)//我方角色全都不再場上時
        {
            m_Roundsystem.EndChecked = false;
            if (Bomb_Round < 3) //安裝炸彈且超過三回合且人走光
            {
                mission_failure.SetActive(true);
                themePlayer.PlayThemes(1);
            }
            else if (Escape)
            {
                Debug.Log("786453AS");
                toggle[2].transform.GetChild(1).GetComponent<Text>().color = Color.green;
                toggle[2].transform.GetChild(0).GetComponent<Image>().sprite = mission_Images[1];
                WinEvent();
            }
            else
            {
                mission_failure.SetActive(true);
                themePlayer.PlayThemes(1);
            }
        }
        else if(Bomb_Round > 6)
        {
            if (Escape)
            {
                Debug.Log("786453AS");
                toggle[2].transform.GetChild(1).GetComponent<Text>().color = Color.green;
                toggle[2].transform.GetChild(0).GetComponent<Image>().sprite = mission_Images[1];
                WinEvent();
            }
            else
            {
                mission_failure.SetActive(true);
                themePlayer.PlayThemes(1);
            }
        }
    }

    void WinEvent()
    {
        explosion.SetActive(false);
        mission_success.SetActive(true);
        themePlayer.PlayThemes(2);
        CAM.SetActive(false);
        CAM_TIMELINE.SetActive(true);
        HPCanvas.gameObject.SetActive(false);
        StartCoroutine(AllDie());
    }
    IEnumerator AllDie()
    {
        yield return new WaitForSeconds(3f);
        if (Aliens.Count > 0)
        {
            foreach(AI a in Aliens)
            {
                a.AIDeath();
            }
        }
        if (Humans.Count > 0)
        {
            foreach(AI a in Humans)
            {
                a.AIDeath();
            }
        }
    }


    public void PreBomb()
    {
        SoundM.Play("Test");
        Prepera = false;
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        MoveCam.ChaTurn(TurnCha);
        ButtonText.text = "引爆";
        DescribeText.text = "啟動電腦自爆程式。";
        LeftText.text = "";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Bomb());
        TurnRun = Canceal;
        SkillCanceal = CancealSkill;
    }
    private void Bomb()
    {
        TurnCha.PreBomb();

        DestroyADPButton();
        DestroySkillButton();
        LRDestory();
        CloseActionUI();
        TurnRun = null;
    }

    public void PreLeave()
    {
        SoundM.Play("Test");
        Prepera = false;
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        MoveCam.ChaTurn(TurnCha);
        ButtonText.text = "撤離";
        DescribeText.text = "撤離現場。";
        LeftText.text = "";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Leave());
        TurnRun = Canceal;
    }
    private void Leave()
    {

        DestroyADPButton();
        DestroySkillButton();
        LRDestory();
        TurnRun = null;
        CloseActionUI();
        TurnCha.Leave();
    }

    public void PreWake()
    {
        if (TurnCha.ComaList.Count == 0)
        {
            return;
        }
        SoundM.Play("Test");
        Prepera = false;
        HealTarget = TurnCha.ComaList.First;
        MoveCam.ChaTurn(HealTarget.Value);
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        //UI
        ButtonText.text = "喚醒";
        DescribeText.text = "拍醒暈眩的友軍單位。";
        LeftText.text = "";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Wake());
        TurnRun = ChangeWakeTarget;
        SkillCanceal = CancealSkill;
    }
    private void Wake()
    {
        TurnCha.PreWake(HealTarget.Value);
        DestroyADPButton();
        DestroySkillButton();
        LRDestory();
        CloseActionUI();
        TurnRun = null;
    }
    private void ChangeWakeTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HealTarget = HealTarget.Next;
            if (HealTarget == null)
            {
                HealTarget = TurnCha.ComaList.First;
            }
            MoveCam.ChaTurn(HealTarget.Value);
        }
        Canceal();
    }
    public void PreStandby()
    {
        SoundM.Play("Test");
        Prepera = false;
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        MoveCam.ChaTurn(TurnCha);
        ButtonText.text = "待機";
        DescribeText.text = "停留原地。";
        LeftText.text = "";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(() => Standby());
        TurnRun = Canceal;
        SkillCanceal = CancealSkill;
    }
    public void Standby()
    {
        TurnRun = null;
        CloseActionUI();
        TurnChaSkip();
    }



    private void Canceal()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CancealSkill();
        }
    }
    void CancealSkill()
    {
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        AttDectPanel.gameObject.SetActive(true);
        MoveCam.ChaTurn(TurnCha);
        TurnRun = CheckMouse;
        SkillCanceal = null;
    }


    bool Grenaded;
    GameObject DangerTile;
    public GameObject EmptyTile;
    public void PreGrenade()
    {
        Prepera = false;
        AttPredictPanel.gameObject.SetActive(false);
        RT.anchoredPosition3D = new Vector3(0, 340, 0);
        ButtonText.text = "手雷";
        DescribeText.text = "丟出手雷造成範圍傷害。";
        LeftText.text = "傷害： 4 ";
        RightText.text = "";
        ActionButton.onClick.RemoveAllListeners();
        if (DangerTile == null)
        {
            DangerTile = new GameObject();
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    GameObject go = Instantiate<GameObject>(EmptyTile, new Vector3(i * 0.67f, 0, j * 0.67f), EmptyTile.transform.rotation, DangerTile.transform);
                    go.GetComponent<Renderer>().material = Resources.Load<Material>("DangerTile");
                    JoinActionTile(go);
                }
            }
        }
        DangerTile.SetActive(false);

        Destroy(GLR);
        Grenaded = true;
        TurnRun = Grenade;//
        SkillCanceal = CancealGrenade;
    }
    public void ThrowGrenade(Tile T)
    {
        if (Grenaded && (T.transform.position - TurnCha.transform.position).magnitude < 12f)
        {
            Grenaded = false;
            TurnCha.PreGrenade(T);
            AttPredictPanel.gameObject.SetActive(false);
            DestroyADPButton();
            DestroySkillButton();
            Prepera = false;

            LRDestory();
            RunUI = null;
        }
    }
    public void Grenade()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CancealGrenade();
        }
    }
    public void AfterGrenade(Tile T)
    {
        DangerTile.SetActive(false);
        StartCoroutine(TurnCha.AfterGrenade());
    }
    void CancealGrenade()
    {
        DangerTile.SetActive(false);
        Grenaded = false;
        RT.anchoredPosition3D = new Vector3(0, 240, 0);
        AttDectPanel.gameObject.SetActive(true);
        MoveCam.ChaTurn(TurnCha);
        SkillCanceal = null;
        TurnRun = CheckMouse;
    }












    public  LinkedList<(AI Cha, int Speed)> Sequence;
    //時間軸及回合開始
    public delegate IEnumerator UIAnimation();
    public UIAnimation UIAnima;
    public TimeLine TLine;
    public int Count;
    public bool GameStart = false;
    public IEnumerator PrepareTimeLine()
    {
        yield return new WaitUntil(() => GameStart == true);
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
            GameObject ChaLogo = Resources.Load<GameObject>(Cha.Value.Cha.name+ "Logo");
            TLine.NewLogo(Cha.Value.Cha, ChaLogo, i);
            yield return new WaitForSecondsRealtime(0.3f);
        }
        GameObject EndLogo = Resources.Load<GameObject>("EndLogo");
        TLine.NewLogo(Sequence.First.Value.Cha, EndLogo, Sequence.Count-1);

        Round.Start();
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
        final_text.text = (6 - Bomb_Round).ToString();
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


    AI Newcome;
    List<ISkill> NewcomeSkills;
    public GameObject increase_text;
    public void NewCome(int Type,int Site)
    {
        if (Type == 0)
        {
            Type = Random.Range(1, 4);
        }
        AI Enemy = Instantiate<GameObject>(Resources.Load<GameObject>("Enemy"+ Type)).GetComponent<AI>();
        GameObject go =  Instantiate(increase_text,this.transform) as GameObject;
        go.SetActive(true);
        Destroy(go, 1.8f);
        Enemy.name = "Enemy"+ Type;
        Newcome = Enemy;
        int i = m_Roundsystem.NewCome(Enemy);
        Aliens.Add(Enemy);
        GameObject ChaLogo = Resources.Load<GameObject>(Enemy.name + "Logo");
        TLine.NewComeLogo(Enemy, ChaLogo, i);
        CreateHP_Bar(Enemy, Enemy.Cha.MaxHP, Enemy.Cha.HP);
        Enemy.InCurrentTile(StartTile[Site]);
        TurnRun = null;
        StartCoroutine(WaitInitialization());
    }
    IEnumerator WaitInitialization()
    {
        yield return new WaitForSeconds(0.1f); 
        TurnRun = NewAct;
    }
    public void NewAct()
    {
        NewcomeSkills = Newcome.Skills;
        Newcome.Skills = null;
        Newcome.Turn = true;
        Newcome.AP = 1;
        Newcome.FindSelectableTiles(1);
        Newcome.ConfirmAction();
        MoveCam.ChaTurn(Newcome);
        Newcome.Skills = NewcomeSkills;
        NewcomeSkills = null;
        Newcome = null;
        TurnRun = null;
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
        bar.MaxEng = target.Cha.MaxEnergy;
        bar.followedTarget = target.transform; //血條的位置 = 角色的位置
        Bar.transform.SetParent(HPCanvas.transform);
        HPBarDic.Add(target, bar);
        m_HP_Bar.Add(bar); //放到列表中
    }

    public void HpControl(AI Target,int valve)
    {
        createhp bar;
        HPBarDic.TryGetValue(Target,out bar);
        if (Target.Cha.MaxEnergy != 0)
        {
            bar.HPControl(valve, Target.Cha.Energy);
        }
        else
        {
            bar.HPControl(valve);
        }
    }
    public void DestroyHPBar(AI Cha)
    {
        createhp HPBar;
        HPBarDic.TryGetValue(Cha, out HPBar);
        HPBarDic.Remove(Cha);
        m_HP_Bar.Remove(HPBar);
        Destroy(HPBar.gameObject);
    }

    public void CleanGarbage()
    {
        Resources.UnloadUnusedAssets();
        RunUI = null;
    }








    //public bool per_but;
    //public GameObject[] camera_point;
    //float[] cam_dir = new float[9];
    //float max_dis;

    //public void Attack_camera()
    //{
    //    Vector3 Target_position; //目標點
    //    cam_dir[0] = 0.0f;
    //    max_dis = 0;
    //    if (TurnCha.Cha.tag == "Human" && per_but == true)
    //    {
    //        float dis = Vector3.Distance(TurnCha.Cha.transform.position, Target.Value.Item1.transform.position); //與目標的距離
    //        Vector3 dir = (Target.Value.Item1.transform.position - TurnCha.Cha.transform.position).normalized; //到目標的方向
    //        Target_position = TurnCha.Cha.transform.position + dir * dis / 2 + new Vector3(0, 1.2f, 0f); //目標點位置
    //        if (dis > 50f)
    //        {

    //            MoveCam.cam_dis = 25f; //攝影機位置往後移動到25
    //            MoveCam.transform.position = Vector3.Lerp(MoveCam.transform.position, Target_position, 5 * Time.deltaTime);//標的物往目標點移動
    //            float Gg = Vector3.Distance(MoveCam.transform.position, Target_position);//如果movecam與目標點距離小於0.02 位置直接等於目標點
    //            if (Gg <= 0.05)
    //                MoveCam.transform.position = Target_position;
    //            Vector3 scp = MoveCam.transform.position + -MoveCam.scene_camera.transform.forward * MoveCam.cam_dis;//攝影機位置往後
    //            MoveCam.scene_camera.transform.position = Vector3.Lerp(scp, MoveCam.scene_camera.transform.position, 3f * Time.deltaTime); //攝影機滑順移動到指定距離
    //            float mg = Vector3.Distance(MoveCam.scene_camera.transform.position, scp);
    //            if (mg <= 0.05)
    //            {
    //                MoveCam.scene_camera.transform.position = scp;
    //            }
    //        }
    //        else
    //        {
    //            camera_point[0].transform.position = TurnCha.Cha.transform.position + new Vector3(0f, 1f, 0f); //攝影機八個位置
    //            for (int i = 1; i < camera_point.Length; i++) //0為父物件 分別判斷1~8到目標點的距離
    //            {
    //                cam_dir[i] = Vector3.Distance(camera_point[i].transform.position, Target_position);
    //            }
    //            for (int i = 1; i < cam_dir.Length; i++) //比大小 判斷1~8
    //            {
    //                if (max_dis < cam_dir[i]) max_dis = cam_dir[i];
    //            }
    //            int f = Array.IndexOf(cam_dir, max_dis);
    //            if (f != -1)
    //            {
    //                RaycastHit hit;
    //                Vector3 eni_dir = (Target_position - camera_point[f].transform.position).normalized;
    //                MoveCam.transform.position = Vector3.Lerp(MoveCam.transform.position, Target_position, 5 * Time.deltaTime); //標的物到雙方中間
    //                Debug.DrawRay(camera_point[f].transform.position, eni_dir);
    //                if (Physics.Raycast(camera_point[f].transform.position, eni_dir, out hit, 1.5f, 1 << 11))
    //                {
    //                    if (f == 1) f = 8;
    //                    Debug.DrawRay(camera_point[f - 1].transform.position, Target_position - camera_point[f - 1].transform.position);
    //                    if (Physics.Raycast(camera_point[f - 1].transform.position, Target_position - camera_point[f - 1].transform.position, out hit, 1.5f))
    //                    {
    //                        if (f == 8) f = 1;
    //                        MoveCam.scene_camera.transform.position = Vector3.Lerp(MoveCam.scene_camera.transform.position, camera_point[f + 1].transform.position, 5 * Time.deltaTime);
    //                    }
    //                    else
    //                        MoveCam.scene_camera.transform.position = Vector3.Lerp(MoveCam.scene_camera.transform.position, camera_point[f - 1].transform.position, 5 * Time.deltaTime);
    //                }
    //                else
    //                    MoveCam.scene_camera.transform.position = Vector3.Lerp(MoveCam.scene_camera.transform.position, camera_point[f].transform.position, 5 * Time.deltaTime);
    //                MoveCam.scene_camera.transform.LookAt(Target_position);
    //            }
    //        }
    //    }


    //    if (TurnCha.Cha.tag == "Alien" && TurnCha.NPC_Prefire == true)
    //    {
    //        MoveCam.cam_dis = 25f; //攝影機位置往後移動到25
    //        float dis = Vector3.Distance(TurnCha.Cha.transform.position, TurnCha.Target.transform.position); //與目標的距離
    //        Vector3 dir = (TurnCha.Target.transform.position - TurnCha.Cha.transform.position).normalized; //到目標的方向
    //        Target_position = TurnCha.Cha.transform.position + dir * dis / 2 + new Vector3(0, 1.2f, 0f); //目標位置

    //        MoveCam.transform.position = Vector3.Lerp(MoveCam.transform.position, Target_position, 3 * Time.deltaTime);//攝影機往前目標移動
    //        float Gg = Vector3.Distance(MoveCam.transform.position, Target_position);

    //        if (Gg <= 0.05)
    //        {
    //            MoveCam.transform.position = Target_position;
    //        }

    //        Vector3 scp = MoveCam.transform.position + -MoveCam.scene_camera.transform.forward * MoveCam.cam_dis;//攝影機為標的物加往後一個方向的距離                                                                                                                  
    //        MoveCam.scene_camera.transform.position = Vector3.Lerp(scp, MoveCam.scene_camera.transform.position, 5 * Time.deltaTime);
    //        float mg = Vector3.Distance(MoveCam.scene_camera.transform.position, scp);
    //        if (mg <= 0.05)
    //        {
    //            MoveCam.scene_camera.transform.position = scp;
    //        }
    //    }
    //}

    public bool Bomb_start;
    public bool Bomb_explosion;
    public GameObject mission_success;
    public GameObject mission_failure;
    public int Bomb_Round = 0;
    //public Text fr;
    public Text final_text;
    public GameObject[] toggle;
    public GameObject explosion;
    public Sprite[] mission_Images;
    public IEnumerator Bomb_button()
    {
        LeaveActionTile(BombSite);
        Bomb_start = true;
        Debug.Log("安裝炸彈");
        toggle[1].SetActive(true);
        toggle[0].transform.GetChild(1).GetComponent<Text>().color = Color.green;
        toggle[0].transform.GetChild(0).GetComponent<Image>().sprite = mission_Images[1];
        explosion.SetActive(true);

        dialog_02.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        dialog_02.SetActive(false);
    }
    public GameObject dialog_03;
    public void StartLeave()
    {
        toggle[2].SetActive(true);
        toggle[1].transform.GetChild(1).GetComponent<Text>().color = Color.green;
        toggle[1].transform.GetChild(0).GetComponent<Image>().sprite = mission_Images[1];
        MoveCam.ChaTurn(LeaveTile[0].GetComponent<AI>());
        LeaveTile[0].MissionPos();
        LeaveTile[1].MissionPos();
        JoinActionTile(LeaveTile[0]);
        JoinActionTile(LeaveTile[1]);
        StartCoroutine(WaitTime());
        RunUI = null;
    }
    IEnumerator WaitTime()
    {
        dialog_03.SetActive(true);
        yield return new WaitForSeconds(2f);
        dialog_03.SetActive(false);
        m_Roundsystem.EndChecked = true;
    }

    public Tile BombSite;
    public GameObject[] status_UI;
    public bool status_bool;

    public void status(int number, string word, AI target)
    {
        Signal go = Instantiate(Resources.Load<GameObject>("Signal"+number)).GetComponent<Signal>();
        go.SetTarget(target.BeAttakePoint.transform, word);
        go.transform.SetParent(HPCanvas.transform);
    }

    //public void status(string a,AI target)
    //{
    //    int judge = 0;
    //    if (a == "heal")
    //    {
    //        judge = 4;
    //        Vector3 vScreenPos = Camera.main.WorldToScreenPoint(target.Target.transform.position);
    //        vScreenPos += Vector3.right * Random.Range(100, 200) + Vector3.up * 100f;
    //        GameObject go = Instantiate(status_UI[judge]) as GameObject;
    //        go.transform.position = vScreenPos;

    //        go.transform.SetParent(HPCanvas.transform);
    //        Destroy(go, 2f);
    //    }
    //    if (TurnCha.Cha.tag == "Human" && judge!=4)
    //    {
    //        if (a == "Demage") judge = 0;
    //        else if (a == "Miss") judge = 1;
    //        else if (a == "MindControl") judge = 2;
    //        else if (a == "coma") judge = 3;
    //        Vector3 vScreenPos = Camera.main.WorldToScreenPoint(target.Target.BeAttakePoint.transform.position);
    //        vScreenPos += Vector3.right * Random.Range(100, 200) + Vector3.up * 100f;
    //        GameObject go = Instantiate(status_UI[judge]) as GameObject;
    //        if (judge == 0)
    //            go.transform.GetChild(2).GetComponent<Text>().text = demage.ToString();
    //        go.transform.position = vScreenPos;

    //        go.transform.SetParent(HPCanvas.transform);
    //        Destroy(go, 2f);
    //    }
    //    if (TurnCha.Cha.tag == "Alien")
    //    {
    //        if (a == "Demage") judge = 0;
    //        else if (a == "Miss") judge = 1;
    //        else if (a == "MindControl") judge = 2;
    //        else if (a == "coma") judge = 3;
    //        Vector3 vScreenPos = Camera.main.WorldToScreenPoint(target.Target.transform.position);
    //        vScreenPos += Vector3.right * -100 + Vector3.up * 150f;
    //        GameObject go = Instantiate(status_UI[judge]) as GameObject;
    //        go.transform.position = vScreenPos;
    //        go.transform.SetParent(HPCanvas.transform);
    //        if (judge == 0)
    //            go.transform.GetChild(2).GetComponent<Text>().text = demage.ToString();
    //        if (judge == 3)
    //            go.transform.GetChild(3).GetComponent<Text>().text = demage.ToString();
    //        Destroy(go, 2f);
    //    }
    //    status_bool = false;
    //}
    public GameObject dialog_01;
    public GameObject b_mission;
    public GameObject dialog_02;
    public void gamestart()
    {
        dialog_01.SetActive(false);
        b_mission.SetActive(false);
        TimeLine_First.SetActive(true);
    }
    public GameObject option;
    public void b_option()
    {
        menu.SetActive(false);
        option.SetActive(true);
    }
    public GameObject bulletnumber;
    public Text Now_Bullet;
    public void bulnum()
    {
        bulletnumber.SetActive(true);
        Now_Bullet.text = TurnCha.Gun.bullet.ToString()+"/"+TurnCha.Gun.MaxBullet.ToString();
    }
}
