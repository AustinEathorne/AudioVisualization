using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPeer : MonoSingleton<AudioPeer>
{
    private AudioSource audioSource;

    [Header("Samples")]
    public float[] audioSamples = new float[512];

    [Header("Bands")]
    private float[] frequencyBands = new float[8];
    private int sampleId = 0;
    private int sampleCount = 0;
    private float averageFrequency = 0.0f;

    [Header("Buffer")]
    public float minBufferDecrease = 0.005f;
    public float bufferDecreaseMultiplier = 1.2f;
    private float[] bandBuffer = new float[8];
    private float[] bufferDecrease = new float[8];

    // 0 - 1 values
    public float[] frequencyBandHighest = new float[8];
    public float[] frequencyBandNormalized = new float[8];
    public float[] bandBufferNormalized = new float[8];

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
            yield return this.CreateFrequencyBands();
            yield return this.BufferBands();
            yield return this.CreateNormalizedBands();

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

    private IEnumerator CreateFrequencyBands()
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

    private IEnumerator BufferBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (frequencyBands[i] > bandBuffer[i])
            {
                this.bandBuffer[i] = this.frequencyBands[i];
                this.bufferDecrease[i] = this.minBufferDecrease;
            }
            else if (frequencyBands[i] < bandBuffer[i])
            {
                this.bandBuffer[i] -= this.bufferDecrease[i];
                this.bufferDecrease[i] *= this.bufferDecreaseMultiplier;
            }
        }

        yield return null;
    }

    private IEnumerator CreateNormalizedBands()
    {
        for (int i = 0; i < 8; i++)
        {
            // Get the highest value for this frequency band
            if (this.frequencyBands[i] > this.frequencyBandHighest[i])
            {
                this.frequencyBandHighest[i] = this.frequencyBands[i];
            }

            // Normalize values
            this.frequencyBandNormalized[i] = this.frequencyBands[i] / this.frequencyBandHighest[i];
            this.bandBufferNormalized[i] = this.bandBuffer[i] / this.frequencyBandHighest[i];
        }

        yield return null;
    }

    #endregion
}
