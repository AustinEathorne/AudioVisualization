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

        // Get and initilialize scene managers
        foreach (VisualizationBase manager in this.visManagerList)
        {
            yield return manager.StartCoroutine(manager.Initialize());
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
        this.visManagerList[0].StartCoroutine(this.visManagerList[0].Run());
        this.currentVisualization = 0;

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
        yield return new WaitUntil(() => this.isChangingVisualization == true);

        // Pause audio
        AudioManager.Instance.PauseMusic();

        // Stop visualization thats running
        yield return this.visManagerList[this.currentVisualization].StartCoroutine(this.visManagerList[this.currentVisualization].Stop());

        // Update UI
        yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.ChangeVisualization(_index));

        // Start new visualization
        yield return this.visManagerList[_index].StartCoroutine(this.visManagerList[_index].Stop());
        this.currentVisualization = _index;

        // Play audio
        AudioManager.Instance.PlayMusic();

        yield return null;
    }

    #endregion
}
