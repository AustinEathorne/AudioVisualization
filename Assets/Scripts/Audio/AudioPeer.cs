using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPeer : MonoSingleton<AudioPeer>
{
    private AudioSource audioSource;

    public float[] audioSamples = new float[512];


    #region Main

    public override IEnumerator Initialize()
    {
        this.audioSource = AudioManager.Instance.audioSource;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        while (this.isRunning)
        {
            yield return this.GetSpectrumData();

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

    #region Audio

    private IEnumerator GetSpectrumData()
    {
        this.audioSource.GetSpectrumData(this.audioSamples, 0, FFTWindow.Blackman);

        yield return null;
    }

    #endregion
}
