using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TankVisual : MonoBehaviour
{
    [SerializeField] private float fillSpeed = 2f;

    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;

    private static readonly int FillID = Shader.PropertyToID("_Fill");

    private Coroutine fillRoutine;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void SetFill(float targetFill)
    {
        targetFill = Mathf.Clamp01(targetFill);

        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(FillRoutine(targetFill));
    }

    private IEnumerator FillRoutine(float targetFill)
    {
        rend.GetPropertyBlock(propertyBlock);

        float currentFill = propertyBlock.GetFloat(FillID);

        float timer = 0f;

        while (timer < fillSpeed)
        {
            timer += Time.deltaTime;

            float fill = Mathf.Lerp(currentFill, targetFill, timer / fillSpeed);

            propertyBlock.SetFloat(FillID, fill);
            rend.SetPropertyBlock(propertyBlock);

            yield return null;
        }

        propertyBlock.SetFloat(FillID, targetFill);
        rend.SetPropertyBlock(propertyBlock);

        fillRoutine = null;
    }
}