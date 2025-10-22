using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private WinCameraController winCamCtrlr;
    [SerializeField] private Enemy_Health player1Health;
    [SerializeField] private Enemy_Health player2Health;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float timer = 5f;
    private bool running = true;

    void Update()
    {
        if (!running) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 0f;
            running = false;
            EndGame();
        }

        timerText.text = Mathf.CeilToInt(timer).ToString();
    }

    private void EndGame()
    {
        int player1Points = player1Health.pointsQty;
        int player2Points = player2Health.pointsQty;

        Transform winnerTrans = null;
        player1Points++; //!!! remove this
        if (player1Points != player2Points)
        {
            if (player1Points > player2Points)
                winnerTrans = player2Health.transform;
            else
                winnerTrans = player1Health.transform;
        }

        if (winnerTrans == null)
        {
            winCamCtrlr.DrawGameCam();
            return;
        }

        winCamCtrlr.FocusOnWinner(winnerTrans);
    }

    public void RestartTimer(float duration = 60f)
    {
        timer = duration;
        running = true;
    }
}
