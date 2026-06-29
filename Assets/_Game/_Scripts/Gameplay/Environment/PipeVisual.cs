using System.Collections;
using UnityEngine;

public class PipeVisual : MonoBehaviour
{
    private Renderer _rend;
    private MaterialPropertyBlock _propBlock;

    private static readonly int FillID = Shader.PropertyToID("_Fill");

    private void Awake()
    {
        _rend = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public void SetFill(float fill)
    {
        _rend.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(FillID, fill);
        _rend.SetPropertyBlock(_propBlock);
    }

    public void FIllPipe(float duration)
    {
        StartCoroutine(FillRotine(duration));
    }

    IEnumerator FillRotine(float duration)
    {
        float timer = 0f;
         
        while (timer < duration)
        {
            timer += Time.deltaTime;
            
            float fill = Mathf.Lerp(0f, 1f, timer / duration);

            SetFill(fill);
            yield return null;
        }

        SetFill(1f);
    }
}
