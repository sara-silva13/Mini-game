using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Global SFX (UI / Rare)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Player / Ambient Sounds")]
    public AudioClip hitSound; 
    public AudioClip walkSound;
    public AudioClip startEndRoundSound; 
    public AudioClip scrapDropperSound; 
    public AudioClip groundScrapEffectSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -----------------------
    // ONE-SHOT SFX (3D world position)
    // -----------------------
    public void PlayOneShotAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        // Create temporary GameObject at the position
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        // Add AudioSource and configure
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.outputAudioMixerGroup = sfxGroup; // your mixer group
        tempSource.spatialBlend = 1f; // 3D sound
        tempSource.volume = volume;
        tempSource.Play();

        // Destroy the temporary object after the clip finishes
        Destroy(tempGO, clip.length);
    }


    // -----------------------
    // FREQUENT / LOCAL SFX (2D / non-spatial)
    // -----------------------
    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }
}
