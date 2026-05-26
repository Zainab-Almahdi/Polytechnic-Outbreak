using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light targetLight;
    public float minIntensity = 0.2f;
    public float maxIntensity = 3f;
    public float flickerSpeed = 0.08f;

    private float targetIntensity;

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        targetIntensity = targetLight.intensity;
    }

    void Update()
    {
        targetLight.intensity = Mathf.Lerp(
            targetLight.intensity,
            targetIntensity,
            Time.deltaTime * 15f
        );

        flickerSpeed -= Time.deltaTime;

        if (flickerSpeed <= 0f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            flickerSpeed = Random.Range(0.03f, 0.1f);
        }
    }
}