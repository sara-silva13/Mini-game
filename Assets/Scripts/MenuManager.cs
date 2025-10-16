using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnClickStartBtn()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnOptionsBtn()
    {
        SceneManager.LoadScene("Options");
    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }
}
