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
    GameObject[] Humans;
    GameObject[] Aliens;
    RoundSysytem m_Roundsystem;
    // Start is called before the first frame update
    void Start()
    {
        m_Roundsystem = RoundSysytem.GetInstance();
        Humans = GameObject.FindGameObjectsWithTag("Human");
        Aliens = GameObject.FindGameObjectsWithTag("Alien");
        m_Roundsystem.RoundPrepare(Humans, Aliens);
        Thread Round = new System.Threading.Thread(m_Roundsystem.RoundStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }








    void onExitClicked()
    {
        //thread abort
        Application.Quit();
    }
}
