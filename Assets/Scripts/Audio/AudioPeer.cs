using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPeer : MonoSingleton<AudioPeer>
{
    private AudioSource audioSource;

    public float[] audioSamples = new float[512];



    public IEnumerator Initialize()
    {
        this.audioSource = AudioManager.Instance.audioSource;

        yield return null;
    }

    public IEnumerator Run()
    {
        while (Application.isPlaying)
        {
            yield return this.GetSpectrumData();

            yield return null;
        }

        yield return null;
    }


    private IEnumerator GetSpectrumData()
    {
        this.audioSource.GetSpectrumData(this.audioSamples, 0, FFTWindow.Blackman);

        yield return null;
    }


}
