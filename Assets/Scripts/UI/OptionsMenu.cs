using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Resolutions")]
    [SerializeField] private TextMeshProUGUI resolutionTxt;
    private static List<ResItem> resolutions = new()
    {
        new ResItem { horizontal = 854, vertical = 480 },
        new ResItem { horizontal = 1280, vertical = 720 },
        new ResItem { horizontal = 1366, vertical = 768 },
        new ResItem { horizontal = 1600, vertical = 900 },
        new ResItem { horizontal = 1920, vertical = 1080 },
        new ResItem { horizontal = 2560, vertical = 1440 },
        new ResItem { horizontal = 3840, vertical = 2160 }
    };
    private int selectedRes = 0;
    [SerializeField] private float applySettingsDelay = 2.5f; // seconds to wait before applying
    private Coroutine applyCoroutine = null;
    [SerializeField] private TextMeshProUGUI confirmationText; // assign in Inspector
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float yOffset = 30f; // how much it moves up
    [SerializeField] private float scaleMultiplier = 1.2f;

    private Vector3 initialPos;
    private Vector3 initialScale;
    private Color initialColor;

    private void Awake()
    {
        initialPos = confirmationText.rectTransform.anchoredPosition;
        initialScale = confirmationText.rectTransform.localScale;
        initialColor = confirmationText.color;

        // start hidden
        var c = confirmationText.color;
        c.a = 0;
        confirmationText.color = c;
    }

    private void Start()
    {
        StartCoroutine(InitVolumeSliders());

        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal)
            {
                selectedRes = i;
                UpdateResolutionText(false);
                break;
            }
        }
    }

    private IEnumerator InitVolumeSliders()
    {
        // Wait one frame so the mixer loads
        yield return null;

        float mixerMusicDb, mixerSfxDb;

        // Read from mixer (should now work properly)
        audioMixer.GetFloat("MusicVolume", out mixerMusicDb);
        audioMixer.GetFloat("SFXVolume", out mixerSfxDb);

        // Convert dB to 0–1 range
        float defaultMusic = Mathf.Pow(10, mixerMusicDb / 20);
        float defaultSFX = Mathf.Pow(10, mixerSfxDb / 20);

        // Load saved or default
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", defaultMusic);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", defaultSFX);

        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }


    public void SetMusicVolume(float value)
    {
        // Convert 0–1 slider value to decibels
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SmallerResolution()
    {
        selectedRes--;
        if (selectedRes < 0)
            selectedRes = 0;

        UpdateResolutionText();
    }
    public void BiggerResolution()
    {
        selectedRes++;
        if (selectedRes >= resolutions.Count)
            selectedRes = resolutions.Count - 1;

        UpdateResolutionText();
    }
    private void UpdateResolutionText(bool autoApply = true)
    {
        resolutionTxt.text = $"{resolutions[selectedRes].horizontal} x {resolutions[selectedRes].vertical}";

        // Restart the timer every time the resolution changes
        if (applyCoroutine != null)
            StopCoroutine(applyCoroutine);

        if (autoApply)
            applyCoroutine = StartCoroutine(ApplyResolutionAfterDelay());
    }

    private IEnumerator ApplyResolutionAfterDelay()
    {
        yield return new WaitForSeconds(applySettingsDelay);

        ApplyResolution(selectedRes);
        StartCoroutine(PlayConfirmationAnimation());
    }

    public static void ApplyResolution(int desiredSelectRes)
    {
        ResItem res = resolutions[desiredSelectRes];
        Screen.SetResolution(res.horizontal, res.vertical, Screen.fullScreenMode);

        // Save the selected resolution index
        PlayerPrefs.SetInt("ResolutionIndex", desiredSelectRes);
        PlayerPrefs.Save();
    }

    private IEnumerator PlayConfirmationAnimation()
    {
        RectTransform rect = confirmationText.rectTransform;

        Vector3 targetPos = initialPos + new Vector3(0, yOffset, 0);
        Vector3 targetScale = initialScale * scaleMultiplier;

        Color color = confirmationText.color;
        color.a = 0;
        confirmationText.color = color;

        float elapsed = 0f;

        // fade in and move up
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            rect.anchoredPosition = Vector3.Lerp(initialPos, targetPos, t);
            rect.localScale = Vector3.Lerp(initialScale, targetScale, t);

            color.a = Mathf.Lerp(0, 1, t);
            confirmationText.color = color;

            yield return null;
        }

        // hold visible for a moment
        yield return new WaitForSeconds(1f);

        // fade out and return to initial
        elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            rect.anchoredPosition = Vector3.Lerp(targetPos, initialPos, t);
            rect.localScale = Vector3.Lerp(targetScale, initialScale, t);

            color.a = Mathf.Lerp(1, 0, t);
            confirmationText.color = color;

            yield return null;
        }
    }

    static public void OnClickGoToMainMenu()
    {
        SceneManager.LoadScene("Start_Menu");
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal, vertical;
}
