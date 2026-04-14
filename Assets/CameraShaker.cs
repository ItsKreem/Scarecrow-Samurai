using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public float duration = 0.2f;
    public AnimationCurve curve;

    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine shakeRoutine;

    public void TriggerShake(float intensity = 1f)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(Shake(intensity));
    }

    private IEnumerator Shake(float intensity)
    {
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float strength = curve.Evaluate(currentTime / duration) * intensity;

            Vector2 random = Random.insideUnitCircle * strength;
            shakeOffset = new Vector3(random.x, random.y, 0f);

            currentTime += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
    void LateUpdate()
    {
        transform.localPosition += shakeOffset;
    }
}