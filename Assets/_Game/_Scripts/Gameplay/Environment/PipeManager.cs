using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [SerializeField] List<PipeVisual> pipes;

    private void Start()
    {
        foreach (PipeVisual pipe in pipes)
        {
            pipe.SetFill(0f);
        }
    }

    public void FillPipe(int index)
    {
        if (index < 0 || index >= pipes.Count)
            return;

        pipes[index].SetFill(1f);
    }
}
