using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnClickStartBtn()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.startEndRoundSound);
        SceneManager.LoadScene("Game");
    }

    public void OnOptionsBtn()
    {
        SceneManager.LoadScene("Options_Menu");
    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }
}
