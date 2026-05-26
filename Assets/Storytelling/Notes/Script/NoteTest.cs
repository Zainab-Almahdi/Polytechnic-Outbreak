using UnityEngine;

public class NoteTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NoteUI.instance.OpenNote("I don't know how much longer I can stay here. The sounds at night are getting closer. I think something got into the ventilation system.");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            NoteUI.instance.CloseNote();
        }
    }
}