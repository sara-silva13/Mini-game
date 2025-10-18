using UnityEngine;
using Cinemachine;
using System.Linq;

public class WinCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera winCamera;
    
    public void FocusOnWinner(Transform winner)
    {
        Transform winnerSpot = null;
        foreach (Transform t in winner)
        {
            if (t.gameObject.name.ToLower().Contains("winnerspot")) 
            {
                winnerSpot = t;
                break;
            }
        }

        // Move the camera rig in front of the winner
        winCamera.transform.position = winnerSpot.localPosition;
        winCamera.transform.rotation = Quaternion.LookRotation(winner.position - winnerSpot.position);

        // Set the camera to look at and follow the winner
        winCamera.Follow = winner;
        winCamera.LookAt = winner;

        // Activate this camera
        winCamera.Priority = 20; // higher than normal gameplay cam
    }

    public void DrawGameCam()
    {
    }
}
