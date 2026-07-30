using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankUI : MonoBehaviour
{
    [SerializeField] private WaterTank waterTank;
    [SerializeField] private Slider slider;
    [SerializeField] private float lerpSpeed = 6f;

    [Header("Delayed Chunk")]
    [SerializeField] private Slider delayedSlider;
    [SerializeField] private float delayTime = 0.2f;
    [SerializeField] private float delayedLerpSpeed = 4f;

    private Coroutine delayedRoutine;

    private void Awake()
    {
        if (waterTank == null)
            waterTank = Object.FindFirstObjectByType<WaterTank>();

        if (slider == null)
            slider = GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        if (waterTank != null)
        {
            // ensure event is initialized then subscribe to updates
            if (waterTank.OnWaterChanged == null)
                waterTank.OnWaterChanged = new UnityEngine.Events.UnityEvent<int, int>();

            waterTank.OnWaterChanged.AddListener(OnWaterChanged);

            // initialize slider to current values (Initialize may be called later and will fire the event)
            slider.maxValue = waterTank.MaxWater;
            slider.value = waterTank.CurrentWater;

            if (delayedSlider != null)
            {
                delayedSlider.maxValue = waterTank.MaxWater;
                delayedSlider.value = waterTank.CurrentWater;
            }
        }
    }

    private void OnDisable()
    {
        if (waterTank != null && waterTank.OnWaterChanged != null)
            waterTank.OnWaterChanged.RemoveListener(OnWaterChanged);
    }

    private void OnWaterChanged(int current, int max)
    {
        if (slider == null)
            return;

        // update max immediately
        slider.maxValue = max;

        // Current water updates immediately
        slider.value = current;

        // Delayed chunk logic (visual only)
        if (delayedSlider != null)
        {
            delayedSlider.maxValue = max;

            if (delayedRoutine != null)
                StopCoroutine(delayedRoutine);

            delayedRoutine = StartCoroutine(DelayedLerpTo(current));
        }
    }

    private IEnumerator DelayedLerpTo(int target)
    {
        float targetF = target;

        // wait for delay
        float t = 0f;
        while (t < delayTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // lerp delayed slider toward target
        while (Mathf.Abs(delayedSlider.value - targetF) > 0.01f)
        {
            delayedSlider.value = Mathf.Lerp(delayedSlider.value, targetF, delayedLerpSpeed * Time.deltaTime);
            yield return null;
        }

        delayedSlider.value = targetF;
        delayedRoutine = null;
    }
}
