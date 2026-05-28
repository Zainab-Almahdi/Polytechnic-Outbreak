using UnityEngine;

public class AntidoteManager : MonoBehaviour
{
    public static AntidoteManager Instance;

    private bool activated = false;

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateAntidote()
    {
        if (activated) return;
        activated = true;

        ZombieHealth[] zombies = FindObjectsOfType<ZombieHealth>();
        foreach (ZombieHealth z in zombies)
        {
            z.KillInstantly();
        }

        Debug.Log("Antidote activated – all zombies eliminated");
    }
}