using UnityEngine;

public class AntidoteButton : MonoBehaviour
{
    public ParticleSystem antidoteGas;
    public AudioSource audioSource;
    public AudioClip activateSound;

    private bool used = false;
    private bool playerInRange = false; 

    void Update()
    {
        if (used) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E)) 
        {
            Activate();
        }
    }

    void Activate()
    {
        used = true;

        if (antidoteGas != null)
            antidoteGas.Play();

        if (audioSource != null && activateSound != null)
            audioSource.PlayOneShot(activateSound);

        AntidoteManager.Instance.ActivateAntidote();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}