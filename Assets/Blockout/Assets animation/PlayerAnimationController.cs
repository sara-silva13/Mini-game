using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private int hands = 0; // 0 = sem item, 1 = uma m�o, 2 = duas m�os
    private float moveSpeed;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- Movimento ---
        moveSpeed = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;
        anim.SetFloat("speed", moveSpeed);

        // --- Simular pegar/largar objetos ---
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            hands = 0;
            anim.SetInteger("hands", hands);
            Debug.Log("Sem objetos");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hands = 1;
            anim.SetInteger("hands", hands);
            Debug.Log("Pegou 1 objeto");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hands = 2;
            anim.SetInteger("hands", hands);
            Debug.Log("Pegou 2 objetos");
        }

        // --- Ataque ---
        if (Input.GetMouseButtonDown(0)) // Bot�o esquerdo do rato
        {
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }

        // --- Dano (s� exemplo) ---
        if (Input.GetKeyDown(KeyCode.H))
        {
            anim.SetBool("Damage", true);
        }
        else
        {
            anim.SetBool("Damage", false);
        }
    }
}
