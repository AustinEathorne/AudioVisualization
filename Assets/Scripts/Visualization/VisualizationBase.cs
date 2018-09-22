using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualizationBase : MonoBehaviour
{

    protected bool isRunning;
    public abstract IEnumerator Initialize();
    public abstract IEnumerator Run();
    public abstract IEnumerator Stop();
}
