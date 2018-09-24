﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization8Bands : VisualizationBase
{
    public Transform parentContainer;

    public float minMinScale;
    public float maxMinScale;
    public float minScale;

    public float minMaxScale;
    public float maxMaxScale;
    public float maxScale;

    public float minScaleMultiplier;
    public float maxScaleMultiplier;
    public float scaleMultiplier;

    public GameObject[] cubeArray = new GameObject[8];
    public Material[] materialArray = new Material[8];


    #region Main

    public override IEnumerator Initialize()
    {
        this.isInitialized = true;

        for (int i = 0; i < this.cubeArray.Length; i++)
        {
            this.materialArray[i] = this.cubeArray[i].GetComponent<MeshRenderer>().material;
        }

        this.isInitialized = false;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        this.parentContainer.gameObject.SetActive(true);

        while (this.isRunning)
        {
            yield return this.StartCoroutine(this.ScaleCubes());

            yield return null;
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        this.isRunning = false;

        this.parentContainer.gameObject.SetActive(false);

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

            scale = AudioPeer.Instance.frequencyBands[i] * this.scaleMultiplier < this.minScale ?
                this.minScale : this.minScale + (AudioPeer.Instance.frequencyBands[i] * this.scaleMultiplier);

            scale = scale > this.maxScale ? this.maxScale : scale;

            this.cubeArray[i].transform.localScale = new Vector3(
                this.cubeArray[i].transform.localScale.x, scale, this.cubeArray[i].transform.localScale.z);

            float color = scale / this.maxScale;
            this.materialArray[i].color = new Color(color, 0.5f, 0.0f);
        }

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