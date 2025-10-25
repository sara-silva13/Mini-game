using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        // Load saved resolution
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int savedRes = PlayerPrefs.GetInt("ResolutionIndex");
            OptionsMenu.ApplyResolution(savedRes);      // applies the saved resolution
        }

    }

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
