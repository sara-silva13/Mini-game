using UnityEngine;

public class LockMovementState : StateMachineBehaviour
{
    [SerializeField] private bool lockMovement = true;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lockMovement)
        {
            var playerMove = animator.GetComponent<Movement>();
            if (playerMove != null)
                playerMove.canMove = false;

            var playerScrapManager = animator.GetComponent<PlayerScrapManager>();
            if (playerScrapManager != null)
                playerScrapManager.canThrowAndPickUp = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lockMovement)
        {
            var playerMove = animator.GetComponent<Movement>();
            if (playerMove != null)
                playerMove.canMove = true;

            var playerScrapManager = animator.GetComponent<PlayerScrapManager>();
            if (playerScrapManager != null)
                playerScrapManager.canThrowAndPickUp = true;
        }
    }
}
