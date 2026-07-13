using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [SerializeField] private List<PipeVisual> pipes;

    private void Start()
    {
        foreach (PipeVisual pipe in pipes)
        {
            pipe.ResetPipe();
        }
    }

    public void FillPipe(int index)
    {
        if (index < 0 || index >= pipes.Count)
            return;

        pipes[index].FillPipe();
    }
}