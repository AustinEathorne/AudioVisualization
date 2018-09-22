using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization512Samples : VisualizationBase
{
    public Transform dynamicObjContainer;

    public GameObject[] cubeArray = new GameObject[512];

    public float minScale;
    public float scaleMultiplier;


    #region Main

    public override IEnumerator Initialize()
    {
        yield return this.StartCoroutine(this.InstantiateCubes());

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

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
                    }
                }));

            // Re-position cube once rotation has been set
            this.cubeArray[i].transform.localPosition = this.cubeArray[i].transform.forward * 100.0f;
        }

        yield return null;
    }

    public IEnumerator ScaleCubes()
    {
        for (int i = 0; i < this.cubeArray.Length; i++)
        {
            if (this.cubeArray[i] == null)
                continue;

            this.cubeArray[i].transform.localScale = new Vector3(
                1.0f, this.minScale + (AudioPeer.Instance.audioSamples[i] * this.scaleMultiplier), 1.0f);
        }

        yield return null;
    }

    #endregion
}
