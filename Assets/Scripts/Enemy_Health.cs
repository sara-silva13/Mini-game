using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemyPointsUI;

    private int enemyPointsQty = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.ToLower().Contains("scrap"))
        {
            enemyPointsQty++;
            enemyPointsUI.text = enemyPointsQty == 1 ? "I" : enemyPointsQty.ToString();

            Destroy(other.gameObject);
        }
    }
}
