using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoSingleton<DemoManager>
{

    public IEnumerator Start()
    {
        // Yield initialization
        yield return this.StartCoroutine(this.Initialize());

        // Start Run routine
        this.StartCoroutine(this.Run());

        yield return null;
    }

    public IEnumerator Initialize()
    {
        // Initialize audio manager
        yield return AudioManager.Instance.StartCoroutine(AudioManager.Instance.Initialize());

        yield return null;
    }


    private IEnumerator Run()
    {
        // Run other managers
        AudioManager.Instance.StartCoroutine(AudioManager.Instance.Run());

        while (Application.isPlaying)
        {
            
            yield return null;
        }

        yield return null;
    }

}
