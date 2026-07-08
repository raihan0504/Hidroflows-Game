using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PipeVisual : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;

    private static readonly int FillID = Shader.PropertyToID("_Fill");

    private Coroutine fillCoroutine;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    /// <summary>
    /// Mengubah nilai Fill pada shader.
    /// </summary>
    public void SetFill(float fill)
    {
        rend.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FillID, Mathf.Clamp01(fill));
        rend.SetPropertyBlock(propertyBlock);
    }

    /// <summary>
    /// Memulai animasi pengisian air.
    /// </summary>
    public void FillPipe()
    {
        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        fillCoroutine = StartCoroutine(FillRoutine());
    }

    private IEnumerator FillRoutine()
    {
        float fill = 0f;

        while (fill < 1f)
        {
            fill += GameManager.Instance.PipeFillSpeed * Time.deltaTime;

            SetFill(fill);

            yield return null;
        }

        SetFill(1f);

        fillCoroutine = null;
    }

    /// <summary>
    /// Mengosongkan kembali pipa.
    /// </summary>
    public void ResetPipe()
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }

        SetFill(0f);
    }
}