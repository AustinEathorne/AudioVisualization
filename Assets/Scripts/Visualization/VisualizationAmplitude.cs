using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationAmplitude : VisualizationBase
{
    public GameObject parentContainer;

    public Transform sphereTransform;
    private Material sphereMaterial;

    public float minMinScale;
    public float maxMinScale;
    public float minScale;

    public float minMaxScale;
    public float maxMaxScale;
    public float maxScale;

    public float minScaleMultiplier;
    public float maxScaleMultiplier;
    public float scaleMultiplier;

    public bool isUsingBuffer;


    #region Main

    public override IEnumerator Initialize()
    {
        this.sphereMaterial = this.sphereTransform.GetComponent<MeshRenderer>().material;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        this.parentContainer.SetActive(true);

        while (this.isRunning)
        {
            yield return this.StartCoroutine(this.ScaleSphere());
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

    public IEnumerator ScaleSphere()
    {
        float scale = this.minScale;

        if (this.isUsingBuffer)
        {
            scale += AudioPeer.Instance.currentAmplitudeBuffer * this.scaleMultiplier;
            scale = scale > this.maxScale ? this.maxScale : scale;
        }
        else
        {
            scale += AudioPeer.Instance.currentAmplitude * this.scaleMultiplier;
            scale = scale > this.maxScale ? this.maxScale : scale;
        }

        this.sphereTransform.localScale = new Vector3(scale, scale, scale);

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
