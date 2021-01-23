using UnityEngine;
using UnityEngine.SceneManagement;
public class UIstartMenu : MonoBehaviour
{
    public AudioSource AS;
    public AudioClip mouseOn;
    public AudioClip quitGame;
    public AudioClip startGame;
 
    public void MouseOn()
    {
        AS.PlayOneShot(mouseOn);
    }
    public void MouseClickedToQuit()
    {
        AS.PlayOneShot(quitGame);
    }

    public void MouseClickedToStart()
    {
        AS.PlayOneShot(startGame);
    }

    public void QuitGame()
    {       
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
