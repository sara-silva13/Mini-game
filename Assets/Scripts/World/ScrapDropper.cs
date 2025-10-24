using UnityEngine;
using System.Collections;

public class ScrapDropper : MonoBehaviour
{
    [SerializeField] private GameObject prefab;  // The object to spawn
    [SerializeField] private float heightOffset = 3f;  // How much higher to spawn above final position

    // Call this method to trigger a new drop
    public void SpawnAndDrop(Vector3 finalPosition)
    {
        AudioManager.Instance.PlayOneShotAtPosition(AudioManager.Instance.scrapDropperSound, finalPosition);

        // Start a coroutine — each call runs independently
        StartCoroutine(SpawnAndMove(finalPosition));
    }

    private IEnumerator SpawnAndMove(Vector3 finalPosition)
    {
        // 1. Calculate spawn position above the final one
        Vector3 spawnPos = finalPosition + Vector3.up * heightOffset;

        // 2. Instantiate the object
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.Euler(-90f, 0f, 0f));

        BoxCollider boxCollider = obj.transform.GetChild(0).GetComponent<BoxCollider>();
        boxCollider.enabled = false;

        // 3. Choose a random duration (between 2 and 3 seconds)
        float duration = Random.Range(2f, 3f);

        // 4. Animate movement from spawnPos → finalPosition
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Pow(elapsed / duration, 2);
            obj.transform.position = Vector3.Lerp(spawnPos, finalPosition, t);
            yield return null;
        }

        // 5. Ensure final position is exact
        obj.transform.position = finalPosition;
        boxCollider.enabled = true;
    }
}
