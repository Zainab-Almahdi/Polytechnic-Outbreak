using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light targetLight;
    public float minIntensity = 0.2f;
    public float maxIntensity = 3f;
    public float flickerSpeed = 0.08f;

    private float timer;

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);
            timer = flickerSpeed;
        }
    }
}