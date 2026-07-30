using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField, Tooltip("Multiplier for how fast the indicator pulses. Larger = faster")]
    private float pulseSpeed = 1f;

    [SerializeField, Tooltip("Minimum scale multiplier relative to original localScale")]
    private float minScale = 0.9f;

    [SerializeField, Tooltip("Maximum scale multiplier relative to original localScale")]
    private float maxScale = 1.1f;

    private Vector3 originalScale;
    private float pulseTimer;
    private bool isPulsing;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // restart pulse from original scale -> minimum
        pulseTimer = 0f;
        isPulsing = true;

        // apply immediate min scale so it starts pulsing from min
        transform.localScale = originalScale * minScale;
    }

    private void OnDisable()
    {
        // stop pulsing and restore original scale
        isPulsing = false;
        transform.localScale = originalScale;
    }

    private void Update()
    {
        if (!isPulsing)
            return;

        // advance timer
        pulseTimer += Time.deltaTime;

        // PingPong produces a smooth triangle from 0..1..0 with period = 2/pulseSpeed
        float t = Mathf.PingPong(pulseTimer * pulseSpeed, 1f);

        float s = Mathf.Lerp(minScale, maxScale, t);

        transform.localScale = originalScale * s;
    }
}
