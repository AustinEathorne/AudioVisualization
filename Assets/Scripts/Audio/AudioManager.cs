using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    // Members are public for editor scripts
    public AudioSource audioSource;

    

    public IEnumerator Initialize()
    {
        // Yield audio peer initialization
        yield return AudioPeer.Instance.StartCoroutine(AudioPeer.Instance.Initialize());

        yield return null;
    }

    public IEnumerator Run()
    {
        // Start audio peer run routine
        AudioPeer.Instance.StartCoroutine(AudioPeer.Instance.Run());

        while (Application.isPlaying)
        {
            
            yield return null;
        }

        yield return null;
    }

}
