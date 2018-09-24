﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPeer : MonoSingleton<AudioPeer>
{
    private AudioSource audioSource;

    public float[] audioSamples = new float[512];

    public float[] frequencyBands = new float[8];
    int sampleId = 0;
    int sampleCount = 0;
    float averageFrequency = 0.0f;

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
            yield return this.MakeFrequencyBands();

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

    private IEnumerator MakeFrequencyBands()
    {
        /* 22050 / 512 = 43 hertz per sample
        
            20-60 hertz
            60-250 hertz
            250-500 hertz
            500-2000 hertz
            2000-4000 hertz
            4000-6000 hertz
            6000-20000 hertz

            0: 2 = 86 hertz
            1: 4 = 172 hertz
            2: 8 = 344 hertz
            3: 16 = 688 hertz
            4: 32 = 1376 hertz
            5: 64 = 2752 hertz
            6: 128 = 5504 hertz
            7: 256 = 11008 hertz
            
            Total 510 samples - Add extra two to the last band
        */

        this.sampleId = 0;
        this.sampleCount = 0;

        for (int i = 0; i < 8; i++)
        {
            this.sampleCount = (int)Mathf.Pow(2, i) * 2;

            // Add last two samples
            if (i == 7)
                this.sampleCount += 2;

            // Calc average amplitude of all samples in band
            this.averageFrequency = 0.0f;
            for (int j = 0; j < sampleCount; j++)
            {
                this.averageFrequency += this.audioSamples[sampleId] * (sampleId + 1);
                sampleId++;
            }

            this.averageFrequency /= sampleId;

            this.frequencyBands[i] = this.averageFrequency;
        }

        yield return null;
    }

    #endregion
}
