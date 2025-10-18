using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AssetImporters;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemyPointsUI;

    public int pointsQty = 0;

    private void Start()
    {
        if (gameObject.name.Contains("2"))
            pointsQty = 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.ToLower().Contains("scrap"))
        {
            pointsQty++;
            string pointsText = pointsQty.ToString();
            pointsText = pointsText.Replace("1", "I");
            enemyPointsUI.text = pointsText;

            Destroy(other.gameObject);
        }
    }
}
