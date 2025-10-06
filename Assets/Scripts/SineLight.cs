using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class SineLight : MonoBehaviour
{
    private new Light light;
    [SerializeField]
    private float amplitude = 15f;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    public void StartPulse(float duration)
    {
        StartCoroutine(PulseLight(duration));
    }

    public void StopPulse()
    {
        StopAllCoroutines();
    }

    public IEnumerator PulseLight(float duration)
    {
        float timer = 0f;
        float startIntensity = light.intensity;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            light.intensity = ((Mathf.Sin(Time.time * amplitude) + 1f) / 2f) * startIntensity;

            yield return null;
        }
        light.intensity = startIntensity;
    }
}
