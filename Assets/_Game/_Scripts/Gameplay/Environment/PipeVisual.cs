using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PipeVisual : MonoBehaviour
{
    private static readonly int FillID = Shader.PropertyToID("_Fill");
    private static readonly int DirectionID = Shader.PropertyToID("_Direction");

    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine fillRoutine;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        ResetPipe();
    }

    public void FillPipe()
    {
        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(FillRoutine());
    }

    public void ResetPipe()
    {
        SetFill(0f);
    }

    private IEnumerator FillRoutine()
    {
        float currentFill = GetCurrentFill();
        float targetFill = 1f;

        while (Mathf.Abs(currentFill - targetFill) > 0.001f)
        {
            currentFill = Mathf.Lerp(
                currentFill,
                targetFill,
                GameManager.Instance.PipeLerpSpeed * Time.deltaTime);

            SetFill(currentFill);

            yield return null;
        }

        SetFill(targetFill);

        fillRoutine = null;
    }

    private void SetFill(float value)
    {
        rend.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FillID, value);
        rend.SetPropertyBlock(propertyBlock);
    }

    private float GetCurrentFill()
    {
        rend.GetPropertyBlock(propertyBlock);
        return propertyBlock.GetFloat(FillID);
    }

    public void SetDirection (float direction)
    {
        Debug.Log($"Direction = {direction}");

        rend.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(DirectionID, direction);

        rend.SetPropertyBlock(propertyBlock);
    }
}