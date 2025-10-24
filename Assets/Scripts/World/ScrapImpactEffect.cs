using UnityEngine;

public class ScrapImpactEffect : MonoBehaviour
{
    [SerializeField] private GameObject impactEffectPrefab; // your particle effect prefab

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger once
        if (hasHit) return;

        Debug.Log("touch");
        hasHit = true;

        // Spawn the effect at the contact point
        Vector3 spawnPos = transform.position;

        // Optional: align effect with surface normal if needed
        // Quaternion rotation = Quaternion.LookRotation(Vector3.up); 
        Instantiate(impactEffectPrefab, spawnPos, Quaternion.identity);

        Destroy(gameObject);
    }
}
