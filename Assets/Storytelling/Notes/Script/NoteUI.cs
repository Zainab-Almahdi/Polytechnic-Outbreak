using System.Collections;
using UnityEngine;
using TMPro;

public class NoteUI : MonoBehaviour
{
    public static NoteUI instance;

    public GameObject notePanel;
    public TextMeshProUGUI noteText;

    private bool isOpen = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        notePanel.SetActive(false);
    }

    void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
        }
    }

    public void OpenNote(string text)
    {
        noteText.text = text;
        notePanel.SetActive(true);
        isOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);
        isOpen = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }
}