using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private WinCameraController winCamCtrlr;
    [SerializeField] private Enemy_Health player1Health;
    [SerializeField] private Enemy_Health player2Health;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform clockPointerTrans;
    private float maxTimer;
    [SerializeField] private float timer;
    private bool running = true;


    public static bool useTextToDisplayTime = false;

    private void Start()
    {
        maxTimer = timer;
        useTextToDisplayTime = timerText.enabled;
    }

    void Update()
    {
        if (!running) return;

        timer -= Time.deltaTime;

        int time = Mathf.CeilToInt(timer);
        if (useTextToDisplayTime)
            timerText.text = time.ToString();

        clockPointerTrans.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 360f, time / maxTimer));

        if (timer <= 0f)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.startEndRoundSound);

            timer = 0f;
            running = false;

            EndGame();
        }
    }

    private void EndGame()
    {
        int player1Points = player1Health.pointsQty;
        int player2Points = player2Health.pointsQty;

        Transform winnerTrans = null;

        if (player1Points != player2Points)
        {
            if (player1Points > player2Points)
                winnerTrans = player2Health.transform;
            else
                winnerTrans = player1Health.transform;
        }

        if (winnerTrans == null)
        {
            Transform[] players = {
                player2Health.transform,
                player1Health.transform
            };
            winCamCtrlr.DrawGameCam(players);
        }
        else
            winCamCtrlr.FocusOnWinner(winnerTrans);
    }

    public void RestartTimer(float duration = 60f)
    {
        timer = duration;
        running = true;
    }
}
