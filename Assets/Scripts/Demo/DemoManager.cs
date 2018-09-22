using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoSingleton<DemoManager>
{
    public VisualizationBase visualizationManager;
    
    #region Main

    public IEnumerator Start()
    {
        // Yield initialization
        yield return this.StartCoroutine(this.Initialize());

        // Start Run routine
        this.StartCoroutine(this.Run());

        yield return null;
    }

    public override IEnumerator Initialize()
    {
        // Initialize other singleton managers
        yield return AudioManager.Instance.StartCoroutine(AudioManager.Instance.Initialize());
        yield return ObjectInstantiater.Instance.StartCoroutine(ObjectInstantiater.Instance.Initialize());

        // Get and initilialize scene managers
        this.visualizationManager = GameObject.FindGameObjectWithTag("VisualizationManager").GetComponent<VisualizationBase>();
        yield return this.visualizationManager.StartCoroutine(this.visualizationManager.Initialize());

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        // Run other singleton managers
        AudioManager.Instance.StartCoroutine(AudioManager.Instance.Run());

        // Run scene manager
        this.visualizationManager.StartCoroutine(this.visualizationManager.Run());

        while (this.isRunning)
        {
            
            yield return null;
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        this.isRunning = false;

        // Stop other singleton managers

        yield return null;
    }

    #endregion
}
