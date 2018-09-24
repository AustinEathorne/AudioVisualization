using System.Collections;
using UnityEngine;

public abstract class Mono<T> : MonoBehaviour where T: MonoBehaviour
{
    protected bool isRunning;
    public abstract IEnumerator Initialize();
    public abstract IEnumerator Run();
    public abstract IEnumerator Stop();
}
