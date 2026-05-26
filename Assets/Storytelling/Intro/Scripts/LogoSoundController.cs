using System.Collections;
using UnityEngine;

public class LogoSoundController : MonoBehaviour
{
    public AudioSource logoSound;
    public float delayBeforePlay = 1.5f;

    void Start()
    {
        StartCoroutine(PlayAfterDelay());
    }

    IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforePlay);
        logoSound.Play();
    }
}