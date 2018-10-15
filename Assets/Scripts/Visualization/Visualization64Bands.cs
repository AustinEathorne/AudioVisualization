using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization64Bands : VisualizationBase
{
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

    [Header("Cube Positioning")]
    public float cubeSpacing;

    [Header("Cubes")]
    public GameObject[] cubeArray = new GameObject[64];
    public Material[] materialArray = new Material[64];    


    #region Main

    public override IEnumerator Initialize()
    {
        this.isInitialized = true;

        yield return this.StartCoroutine(this.InstantiateCubes());

        this.isInitialized = false;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        this.parentTransform.gameObject.SetActive(true);

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

        this.parentTransform.gameObject.SetActive(false);

        yield return null;
    }

    #endregion

    #region Visualization

    public IEnumerator InstantiateCubes()
    {
        float totalLength = (1.0f * 64.0f) + (this.cubeSpacing * 63);
        float xPosition = -(totalLength * 0.5f);
        float offset = 1.0f + this.cubeSpacing;


        for (int i = 0; i < this.cubeArray.Length; i++)
        {
            // Instantiate cube
            ObjectInstantiater.Instance.StartCoroutine(ObjectInstantiater.Instance.InstantiatePrefab(
                PrefabType.CubeVerticallyScaling, this.parentTransform, this.parentTransform.position, new Quaternion(), _obj => {
                    if (_obj != null)
                    {
                        this.cubeArray[i] = _obj;
                        this.materialArray[i] = _obj.GetComponentInChildren<MeshRenderer>().material;
                    }
                }));

            this.cubeArray[i].transform.localPosition = new Vector3(xPosition, 0, 0);
            xPosition += offset;
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

            if (this.isUsingBandBuffers)
            {
                scale = AudioPeer.Instance.bandBufferNormalized64[i] * this.scaleMultiplier < this.minScale ?
                this.minScale : this.minScale + (AudioPeer.Instance.bandBufferNormalized64[i] * this.scaleMultiplier);

                float colorScale = AudioPeer.Instance.bandBufferNormalized64[i];
                this.materialArray[i].SetColor("_EmissionColor", DemoManager.Instance.baseEmissionColor * Mathf.LinearToGammaSpace(colorScale));
            }
            else
            {
                scale = AudioPeer.Instance.frequencyBandNormalized64[i] * this.scaleMultiplier < this.minScale ?
                this.minScale : this.minScale + (AudioPeer.Instance.frequencyBandNormalized64[i] * this.scaleMultiplier);

                float colorScale = AudioPeer.Instance.frequencyBandNormalized64[i];
                this.materialArray[i].SetColor("_EmissionColor", DemoManager.Instance.baseEmissionColor * Mathf.GammaToLinearSpace(colorScale));
            }

            scale = scale > this.maxScale ? this.maxScale : scale;

            this.cubeArray[i].transform.localScale = new Vector3(
                this.cubeArray[i].transform.localScale.x, scale, this.cubeArray[i].transform.localScale.z);
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
