using UnityEngine;

public class ScrapImpactEffect : MonoBehaviour
{
    [SerializeField] private GameObject groundEffectPrefab;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private LayerMask groundLayer;         // assign the ground layer

    private bool hasHit = false;
    private Rigidbody rb;
    private Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Cache velocity every physics frame
        lastVelocity = rb.velocity;

        rb.AddForce(Physics.gravity, ForceMode.Acceleration);
    }


    private void OnTriggerEnter(Collider other)
    {
        bool Launched = gameObject.GetComponent<Rigidbody>().useGravity;

        // Only trigger once
        if (hasHit || !Launched) return;

        // Check if we hit the ground layer
        if (((1 << other.gameObject.layer) & groundLayer.value) != 0)
        {
            bool chaoHit = other.name.ToLower().Contains("chao");

            hasHit = true;

            // Spawn the effect at the contact point
            Vector3 spawnPos = transform.position;
            Vector3 direction = lastVelocity.normalized;

            // Face the direction it was moving:
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Convert to Euler, modify, then convert back
            Vector3 euler = rotation.eulerAngles;
            euler.x = 0;          // force flat on X axis

            rotation = Quaternion.Euler(euler);

            //player hit
            if (!chaoHit)
            {
                AudioManager.Instance.PlayOneShotAtPosition(AudioManager.Instance.hitSound, spawnPos);
                Instantiate(impactEffectPrefab, spawnPos, rotation);
            }

            AudioManager.Instance.PlayOneShotAtPosition(AudioManager.Instance.scrapDropperSound, new Vector3(spawnPos.x, 0.1f, spawnPos.z));
            Instantiate(groundEffectPrefab, new Vector3(spawnPos.x, 0.1f, spawnPos.z), rotation);

            Destroy(gameObject);
        }
    }
}
