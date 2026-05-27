using UnityEngine;
using TMPro;

public class NoteInteraction : MonoBehaviour
{
    [TextArea(3, 10)]
    public string noteContent;

    // TODO: This uses "Player" tag - make sure player object has Player tag set
    private bool playerNearby = false;
    private PlayerInputHandler inputHandler;

    public GameObject worldPrompt;

    void Start()
    {
        // Hide world prompt at start
        worldPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && inputHandler != null && inputHandler.InteractPressed)
        {
            NoteUI.instance.OpenNote(noteContent);
            // Hide prompt when note opens
            worldPrompt.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            inputHandler = other.GetComponent<PlayerInputHandler>();
            // Show world prompt when player is near
            worldPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            inputHandler = null;
            // Hide world prompt when player leaves
            worldPrompt.SetActive(false);
        }
    }
}