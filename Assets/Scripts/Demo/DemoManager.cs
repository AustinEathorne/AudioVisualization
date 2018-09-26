using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoSingleton<DemoManager>
{
    public List<VisualizationBase> visManagerList;
    public int currentVisualization;
    public bool isChangingVisualization;

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
        this.isInitialized = false;

        // Initialize other singleton managers
        yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.Initialize());
        yield return AudioManager.Instance.StartCoroutine(AudioManager.Instance.Initialize());
        yield return ObjectInstantiater.Instance.StartCoroutine(ObjectInstantiater.Instance.Initialize());

        // Initilialize and turn off scene managers
        foreach (VisualizationBase manager in this.visManagerList)
        {
            yield return manager.StartCoroutine(manager.Initialize());
            yield return manager.StartCoroutine(manager.Stop());

        }



        this.isInitialized = true;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        // Run other singleton managers
        AudioManager.Instance.StartCoroutine(AudioManager.Instance.Run());
        CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.Run());

        // Run first visualization manager
        this.visManagerList[this.currentVisualization].StartCoroutine(this.visManagerList[this.currentVisualization].Run());

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

    #region Visualizations

    public IEnumerator ChangeVisualization(int _index)
    {
        yield return new WaitUntil(() => this.isChangingVisualization == false);

        this.isChangingVisualization = true;

        // Stop visualization thats running
        yield return this.visManagerList[this.currentVisualization].StartCoroutine(this.visManagerList[this.currentVisualization].Stop());

        // Update UI
        yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.ChangeVisualization(_index));

        // Start new visualization
        this.visManagerList[_index].StartCoroutine(this.visManagerList[_index].Run());
        this.currentVisualization = _index;

        this.isChangingVisualization = false;

        yield return null;
    }

    #endregion
}
