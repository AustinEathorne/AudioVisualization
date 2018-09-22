using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    // Members are public for editor scripts
    public AudioSource audioSource;


    #region Main

    public override IEnumerator Initialize()
    {
        // Yield audio peer initialization
        yield return AudioPeer.Instance.StartCoroutine(AudioPeer.Instance.Initialize());

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        // Start playing music
        this.audioSource.Play();

        // Start audio peer run routine
        AudioPeer.Instance.StartCoroutine(AudioPeer.Instance.Run());

        while (this.isRunning)
        {
            
            yield return null;
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        this.isRunning = false;
        yield return null;
    }

    #endregion
}
