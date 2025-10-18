using UnityEngine;

public class Movement : MonoBehaviour
{
    
    [Header("Configurações de Jogador")]
    [Tooltip("1 = usa WASD | 2 = usa setas")]
    public int playerNumber = 1;

    private CharacterController controller;
    
    private float moveSpeed = 5f;
    private Enemy_Health player_Charger;
    private Vector3 moveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    
        player_Charger = GetComponent<Enemy_Health>();
    }

    void Update()
    {
        // Leitura de inputs dependendo do jogador
        float horizontal = 0f;
        float vertical = 0f;

        if (playerNumber == 1)
        {
            // Player 1 → WASD
            horizontal = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
            vertical = (Input.GetKey(KeyCode.S) ? 1 : 0) - (Input.GetKey(KeyCode.W) ? 1 : 0);
        }
        else if (playerNumber == 2)
        {
            // Player 2 → setas
            horizontal = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
            vertical = (Input.GetKey(KeyCode.DownArrow) ? 1 : 0) - (Input.GetKey(KeyCode.UpArrow) ? 1 : 0);
        }

        moveDirection = new Vector3(vertical, 0f, horizontal).normalized;

        // Mover o jogador
        if (moveDirection.magnitude > 0f)
        {
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

            if (controller != null)
                controller.Move(movement);
            else
                transform.Translate(movement, Space.World);

            // Faz o jogador olhar na direção do movimento
            transform.forward = -moveDirection;
        }
    }
}
