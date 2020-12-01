using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    private static UISystem m_instance;
    public UISystem GetInstance()
    {
        if (m_instance != null)
            return m_instance;
        else
        {
            m_instance = this;
            return m_instance;
        }
    }
    AI[] Humans;
    AI[] Aliens;
    RoundSysytem m_Roundsystem;
    Thread Round;
    // Start is called before the first frame update
    void Start()
    {
        m_Roundsystem = RoundSysytem.GetInstance();
        GameObject[] GHumans = GameObject.FindGameObjectsWithTag("Human");
        Humans = new AI[GHumans.Length];
        for(int count = 0; count< GHumans.Length; ++count)
        {
            Humans[count] = GHumans[count].GetComponent<HumanAI>();
        }
        GameObject[] GAliens = GameObject.FindGameObjectsWithTag("Alien");
        Aliens = new AI[GAliens.Length];
        for (int count = 0; count < GAliens.Length; ++count)
        {
            Aliens[count] = GAliens[count].GetComponent<NPC_AI>();
        }
        m_Roundsystem.RoundPrepare(Humans, Aliens);
        Round = new System.Threading.Thread(m_Roundsystem.RoundStart);
        Round.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }








    public void onExitClicked()
    {
        //thread abort
        Application.Quit();
    }
}
