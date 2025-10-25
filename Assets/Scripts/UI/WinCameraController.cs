using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinCameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera winCamera;
    [SerializeField] private Transform drawCamSpotTrans;
    [SerializeField] private Transform drawLookAtSpotTrans;

    [Header("UI References")]
    [SerializeField] private GameObject UIGamePlay;
    [SerializeField] private TextMeshProUGUI winnerNameText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button leaveButton;

    [Header("Timing")]
    [SerializeField] private float showNameDelay;
    [SerializeField] private float showWinDelay;
    [SerializeField] private float nameAnimDuration;
    [SerializeField] private float winAnimDuration;

    [Header("Rematch Timing")]
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float rematchFadeInDuration;

    public void FocusOnWinner(Transform winner)
    {
        UIGamePlay.SetActive(false);

        Transform winnerSpot = null;
        foreach (Transform t in winner)
        {
            if (t.name.ToLower().Contains("winnerspot"))
            {
                winnerSpot = t;
                break;
            }
        }

        // Don't move the camera manually — just tell Cinemachine what to follow/look at
        winCamera.Follow = winnerSpot; // follow the spot (so it stays stable)
        winCamera.LookAt = winner;     // look at the winner

        // Enable the camera
        winCamera.Priority = 20;

        // Trigger animation on winner
        Animator anim = winner.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Win");

        // Start UI sequence
        StartCoroutine(WinSequence(winner));
    }

    private IEnumerator WinSequence(Transform winner)
    {
        // Reset UI instantly
        ResetText(winnerNameText);
        winnerNameText.text = winner.name;
        ResetText(winText);
        winText.text = "Win!";

        SetButtonVisible(rematchButton, false);
        SetButtonVisible(leaveButton, false);
        
        // Show the winner name
        yield return new WaitForSeconds(showNameDelay);
        StartCoroutine(AnimateTextFromBelow(winnerNameText, nameAnimDuration));

        // Show the Win text
        yield return new WaitForSeconds(showWinDelay - showNameDelay);
        StartCoroutine(AnimateBounceFade(winText, winAnimDuration));

        // Wait before fade-out
        yield return new WaitForSeconds(showNameDelay + showWinDelay);

        // Fade everything out
        yield return FadeOutTexts(fadeOutDuration);

        // Fade in rematch button
        yield return FadeInButton(rematchButton, rematchFadeInDuration);
        yield return FadeInButton(leaveButton, rematchFadeInDuration);
    }

    private void ResetText(TextMeshProUGUI text)
    {
        if (text == null) return;
        text.alpha = 0f;
        text.rectTransform.localScale = Vector3.one * 1.5f;

        text.gameObject.SetActive(true);
    }

    private IEnumerator AnimateTextFromBelow(TextMeshProUGUI text, float duration)
    {
        RectTransform rt = text.rectTransform;
        CanvasGroup cg = text.GetComponent<CanvasGroup>();

        Vector3 startPos = rt.anchoredPosition + new Vector2(0, -200f);
        Vector3 endPos = rt.anchoredPosition;
        Vector3 startScale = Vector3.one * 1.5f;
        Vector3 endScale = Vector3.one;

        rt.anchoredPosition = startPos;
        rt.localScale = startScale;
        text.alpha = 1f; //reset this and control alpha with cg
        cg.alpha = 0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float eased = Mathf.SmoothStep(0, 1, t);

            rt.anchoredPosition = Vector3.Lerp(startPos, endPos, eased);
            rt.localScale = Vector3.Lerp(startScale, endScale, eased);
            cg.alpha = eased;

            yield return null;
        }

        rt.anchoredPosition = endPos;
        rt.localScale = endScale;
        cg.alpha = 1f;
    }
    private IEnumerator AnimateBounceFade(TextMeshProUGUI text, float duration)
    {
        RectTransform rt = text.rectTransform;
        CanvasGroup cg = text.GetComponent<CanvasGroup>();
        if (cg == null) cg = text.gameObject.AddComponent<CanvasGroup>();

        // Start values
        Vector3 startScale = Vector3.one * 1.5f;
        Vector3 midScale = Vector3.one;      // target
        Vector3 bounceScale = Vector3.one * 1.06f; // slight overshoot
        Vector3 endScale = Vector3.one;

        text.alpha = 1f; //reset this and control alpha with cg
        cg.alpha = 0f;
        rt.localScale = startScale;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float eased = Mathf.SmoothStep(0, 1, t);

            // Alpha fade-in
            cg.alpha = eased;

            // Split animation into two parts: shrink, then bounce
            if (eased < 0.7f)
            {
                // First 80%: go from 1.5x → 1x
                float phase = eased / 0.8f;
                rt.localScale = Vector3.Lerp(startScale, midScale, phase);
            }
            else
            {
                // Last 20%: bounce up slightly (1.0 → 1.2 → 1.0)
                float phase = (eased - 0.8f) / 0.2f; // normalized 0–1
                float bounce = Mathf.Sin(phase * Mathf.PI); // ease in/out curve
                rt.localScale = Vector3.Lerp(midScale, bounceScale, bounce);
            }

            yield return null;
        }

        rt.localScale = endScale;
        cg.alpha = 1f;
    }

    private IEnumerator FadeOutTexts(float duration)
    {
        CanvasGroup[] groups = {
            winnerNameText.GetComponent<CanvasGroup>(),
            winText.GetComponent<CanvasGroup>()
        };

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float alpha = Mathf.Lerp(1, 0, t);
            foreach (var g in groups)
                if (g != null) g.alpha = alpha;
            yield return null;
        }

        foreach (var g in groups)
            if (g != null) g.alpha = 0;
    }

    private IEnumerator FadeInButton(Button button, float duration)
    {
        if (button == null) yield break;

        CanvasGroup cg = button.GetComponent<CanvasGroup>();

        button.gameObject.SetActive(true);
        cg.alpha = 0f;
        button.interactable = false;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cg.alpha = Mathf.SmoothStep(0, 1, t);
            yield return null;
        }

        cg.alpha = 1f;
        button.interactable = true;
    }

    private void SetButtonVisible(Button button, bool visible)
    {
        if (button == null) return;
        button.gameObject.SetActive(visible);
        var cg = button.GetComponent<CanvasGroup>();
        cg.alpha = visible ? 1f : 0f;
        button.interactable = visible;
    }

    public void OnRematchClicked()
    {
        //!!! Play a sound

        // Reload the current scene
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void DrawGameCam(Transform[] players)
    {
        UIGamePlay.SetActive(false);

        winCamera.Follow = drawCamSpotTrans;
        winCamera.LookAt = drawLookAtSpotTrans;     // look at the center

        winCamera.Priority = 20;

        foreach (var player in players)
        {
            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Win");
        }

        StartCoroutine(DrawSequence());
    }

    private IEnumerator DrawSequence()
    {
        // Reset UI instantly
        ResetText(winText);
        winText.text = "Draw!";

        SetButtonVisible(rematchButton, false);
        SetButtonVisible(leaveButton, false);

        // Show the Draw text
        yield return new WaitForSeconds((showWinDelay + showNameDelay) / 2);
        StartCoroutine(AnimateBounceFade(winText, winAnimDuration));

        // Wait before fade-out
        yield return new WaitForSeconds(showWinDelay + showNameDelay);

        // Fade everything out
        yield return FadeOutTexts(fadeOutDuration);

        // Fade in rematch button
        yield return FadeInButton(rematchButton, rematchFadeInDuration);
        yield return FadeInButton(leaveButton, rematchFadeInDuration);
    }

    public void LeaveGame()
    {
        OptionsMenu.OnClickGoToMainMenu();
    }
}
