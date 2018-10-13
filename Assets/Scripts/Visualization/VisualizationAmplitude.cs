using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationAmplitude : VisualizationBase
{
    [Header("Container")]
    public GameObject parentContainer;

    [Header("Scale")]
    public float minMinScale;
    public float maxMinScale;
    public float minScale;

    public float minMaxScale;
    public float maxMaxScale;
    public float maxScale;

    public float minScaleMultiplier;
    public float maxScaleMultiplier;
    public float scaleMultiplier;

    [Header("Buffer")]
    public bool isUsingBandBuffers;

    [Header("Cubes")]
    public GameObject[] cubeArray = new GameObject[8];
    public Material[] materialArray = new Material[8];
    public Transform amplitudeCube;
    public Material amplitudeCubeMaterial;

    #region Main

    public override IEnumerator Initialize()
    {
        for (int i = 0; i < this.cubeArray.Length; i++)
        {
            this.materialArray[i] = this.cubeArray[i].GetComponentInChildren<MeshRenderer>().material;
        }

        this.amplitudeCubeMaterial = this.amplitudeCube.GetComponentInChildren<MeshRenderer>().material;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        this.parentContainer.SetActive(true);

        while (this.isRunning)
        {
            yield return this.StartCoroutine(this.ScaleCubes());
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        this.isRunning = false;
        this.parentContainer.SetActive(false);
        yield return null;
    }

    #endregion

    #region Visualization

    public IEnumerator ScaleCubes()
    {
        float scale = 0.0f;

        for (int i = 0; i < this.cubeArray.Length; i++)
        {
            if (this.cubeArray[i] == null)
                continue;

            if (this.isUsingBandBuffers)
            {
                scale = AudioPeer.Instance.bandBufferNormalized[i] * this.scaleMultiplier < this.minScale ?
                this.minScale : this.minScale + (AudioPeer.Instance.bandBufferNormalized[i] * this.scaleMultiplier);

                float colorScale = AudioPeer.Instance.bandBufferNormalized[i];
                this.materialArray[i].SetColor("_EmissionColor", DemoManager.Instance.baseEmissionColor * Mathf.LinearToGammaSpace(colorScale));
            }
            else
            {
                scale = AudioPeer.Instance.frequencyBandNormalized[i] * this.scaleMultiplier < this.minScale ?
                this.minScale : this.minScale + (AudioPeer.Instance.frequencyBandNormalized[i] * this.scaleMultiplier);

                float colorScale = AudioPeer.Instance.frequencyBandNormalized[i];
                this.materialArray[i].SetColor("_EmissionColor", DemoManager.Instance.baseEmissionColor * Mathf.LinearToGammaSpace(colorScale));
            }

            scale = scale > this.maxScale ? this.maxScale : scale;

            this.cubeArray[i].transform.localScale = new Vector3(
                this.cubeArray[i].transform.localScale.x, scale, this.cubeArray[i].transform.localScale.z);
        }

        if (this.isUsingBandBuffers)
        {
            scale = AudioPeer.Instance.currentAmplitudeBuffer * this.scaleMultiplier < this.minScale ?
            this.minScale : this.minScale + (AudioPeer.Instance.currentAmplitudeBuffer * this.scaleMultiplier);

            float colorScale = AudioPeer.Instance.currentAmplitudeBuffer * 0.5f;
            this.amplitudeCubeMaterial.SetColor("_EmissionColor", DemoManager.Instance.baseEmissionColor * Mathf.LinearToGammaSpace(colorScale));
        }
        else
        {
            scale = AudioPeer.Instance.currentAmplitude * this.scaleMultiplier < this.minScale ?
            this.minScale : this.minScale + (AudioPeer.Instance.currentAmplitude * this.scaleMultiplier);

            float colorScale = AudioPeer.Instance.currentAmplitude;
            this.amplitudeCubeMaterial.SetColor("_EmissionColor", DemoManager.Instance.baseEmissionColor * Mathf.LinearToGammaSpace(colorScale));
        }

        scale = scale > this.maxScale ? this.maxScale : scale;

        this.amplitudeCube.localScale = new Vector3(
            this.amplitudeCube.transform.localScale.x, scale, amplitudeCube.transform.localScale.z);

        yield return null;
    }

    #endregion

    #region Get/Set Radio Future

    public void SetMinScale(float _scale)
    {
        this.minScale = _scale;
    }

    public void SetMaxScale(float _scale)
    {
        this.maxScale = _scale;
    }

    public void SetScaleMultiplier(float _multiplier)
    {
        this.scaleMultiplier = _multiplier;
    }

    #endregion
}
