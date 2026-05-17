using TMPro;
using UnityEngine;

public class ReadableNote : MonoBehaviour
{
    [TextArea(3, 10)]
    public string noteMessage;

    public GameObject notePanel;
    public TextMeshProUGUI noteText;

    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            notePanel.SetActive(true);
            noteText.text = noteMessage;
        }

        if (notePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            notePanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            notePanel.SetActive(false);
        }
    }
}