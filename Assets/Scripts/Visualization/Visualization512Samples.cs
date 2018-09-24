﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization512Samples : VisualizationBase
{
    public Transform dynamicObjContainer;

    public float minMinScale;
    public float maxMinScale;
    public float minScale;

    public float minMaxScale;
    public float maxMaxScale;
    public float maxScale;

    public float scaleMultiplier;
    public float minScaleMultiplier;
    public float maxScaleMultiplier;

    public GameObject[] cubeArray = new GameObject[512];
    public Material[] materialArray = new Material[512];


    #region Main

    public override IEnumerator Initialize()
    {
        yield return this.canvas.StartCoroutine(this.canvas.Initialize());
        yield return this.StartCoroutine(this.InstantiateCubes());

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        // Start canvas run routine
        this.canvas.StartCoroutine(this.canvas.Run());

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
        yield return null;
    }

    #endregion

    #region Visualization

    public IEnumerator InstantiateCubes()
    {
        float deltaAngle = 360.0f / 512.0f;
        Quaternion tempRotation = new Quaternion();

        for (int i = 0; i < 512; i++)
        {
            // Update cube rotation
            tempRotation.eulerAngles = new Vector3(0.0f, -deltaAngle * i, 0.0f);

            // Instantiate cube
            ObjectInstantiater.Instance.StartCoroutine(ObjectInstantiater.Instance.InstantiatePrefab(
                PrefabType.CubeVerticallyScaling, this.dynamicObjContainer, this.dynamicObjContainer.position, tempRotation, _obj => {
                    if (_obj != null)
                    {
                        this.cubeArray[i] = _obj;
                        this.materialArray[i] = _obj.GetComponent<MeshRenderer>().material;
                    }
                }));

            // Re-position cube once rotation has been set
            this.cubeArray[i].transform.localPosition = this.cubeArray[i].transform.forward * 100.0f;
        }

        yield return null;
    }

    public IEnumerator ScaleCubes()
    {
        float scale = 0.0f;

        for (int i = 0; i < this.cubeArray.Length; i++)
        {
            if (this.cubeArray[i] == null)
                continue;

            scale = AudioPeer.Instance.audioSamples[i] * this.scaleMultiplier < this.minScale ? 
                this.minScale : this.minScale + (AudioPeer.Instance.audioSamples[i] * this.scaleMultiplier);

            scale = scale > this.maxScale ? this.maxScale : scale;

            this.cubeArray[i].transform.localScale = new Vector3(1.0f, scale, 1.0f);

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
