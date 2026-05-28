using UnityEngine;
using System.Collections;

public class ZombieAudio : MonoBehaviour
{
    [SerializeField] private AudioClip moanClip;
    [SerializeField] private float minInterval = 3f;
    [SerializeField] private float maxInterval = 8f;

    private AudioSource audioSource;
    private bool isDead = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setup AudioSource for 3D sound
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        StartCoroutine(MoanRoutine());
    }

    private IEnumerator MoanRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            
            if (!isDead && moanClip != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(moanClip);
            }
        }
    }

    public void OnDeath()
    {
        isDead = true;
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
