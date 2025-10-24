using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AssetImporters;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private TextMeshProUGUI enemyPointsUI;

    public int pointsQty = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.ToLower().Contains("scrap"))
        {
            anim.SetBool("Damage", true);

            pointsQty++;
            string pointsText = pointsQty.ToString();
            pointsText = pointsText.Replace("1", "I");
            enemyPointsUI.text = pointsText;

            Destroy(other.gameObject);
        }
    }
}
