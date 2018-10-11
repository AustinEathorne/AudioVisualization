using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DemoManager : MonoSingleton<DemoManager>
{
    public List<VisualizationBase> visManagerList;
    public int currentVisualization;
    public bool isChangingVisualization;

    public Vector2 lastMousePosition = Vector3.zero;
    public float timeToIdle;
    public float idleTimeCount = 0.0f;

    #region Main

    public IEnumerator Start()
    {
        // Yield initialization
        yield return this.StartCoroutine(this.Initialize());

        // Start Run routine
        //this.StartCoroutine(this.Run());

        yield return null;
    }

    // Using example audio
    public IEnumerator StartDemo()
    {
        yield return this.StartCoroutine(this.UseDemoFiles());

        // Run
        this.StartCoroutine(this.Run());

        // Close start screen
        CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.CloseStartScreen(true, ""));

        yield return null;
    }

    // Using file path
    public IEnumerator StartDemo(string _filePath)
    {
        // Load in files
        yield return this.StartCoroutine(this.LoadFiles(_filePath));

        // Run
        this.StartCoroutine(this.Run());

        // Close start screen
        CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.CloseStartScreen(false, _filePath));

        yield return null;
    }


    public override IEnumerator Initialize()
    {
        this.isInitialized = false;

        // Initialize other singleton managers
        yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.Initialize());
        yield return AudioManager.Instance.StartCoroutine(AudioManager.Instance.Initialize());
        yield return ObjectInstantiater.Instance.StartCoroutine(ObjectInstantiater.Instance.Initialize());
        yield return ObjectPoolManager.Instance.StartCoroutine(ObjectPoolManager.Instance.CreatePools());

        // Initilialize and turn off visualization managers
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

        // Run current visualization manager
        this.visManagerList[this.currentVisualization].StartCoroutine(this.visManagerList[this.currentVisualization].Run());

        while (this.isRunning)
        {
            if (this.CheckForEscapeInput())
            {
                CanvasManager.Instance.ToggleEscapeContainer();
            }

            this.CheckForMouseInput();
            this.CheckForKeyInput();


            if (this.IsIdle())
            {
                // Check if the demo UI is currently active
                if (CanvasManager.Instance.isDemoUiActive)
                {
                    Cursor.visible = false;
                    yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.ToggleDemoUI(false));
                }
            }
            // Mouse input is not idle
            else
            {
                // Check if the demo UI is currently inactive
                if (!CanvasManager.Instance.isDemoUiActive)
                {
                    Cursor.visible = true;
                    yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.ToggleDemoUI(true));
                }
            }

            yield return null;
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        // Stop visualization thats running
        yield return this.visManagerList[this.currentVisualization].StartCoroutine(this.visManagerList[this.currentVisualization].Stop());

        // Stop other singleton managers
        yield return AudioManager.Instance.StartCoroutine(AudioManager.Instance.Stop());
        yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.Stop());

        this.isRunning = false;

        yield return null;
    }


    public IEnumerator Quit()
    {
        yield return this.StartCoroutine(this.Stop());

        Application.Quit();

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

    #region Input

    public bool CheckForEscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            return true;
        }

        return false;
    }

    public void CheckForMouseInput()
    {
        Vector2 mousePos = Input.mousePosition;

        if (mousePos != this.lastMousePosition)
        {
            this.lastMousePosition = mousePos;
            this.idleTimeCount = 0.0f;
        }
    }

    public void CheckForKeyInput()
    {
        if (Input.anyKey) // includes mouse clicks
        {
            this.idleTimeCount = 0.0f;
        }
    }

    public bool IsIdle()
    {
        this.idleTimeCount += Time.deltaTime;

        if (this.idleTimeCount >= this.timeToIdle)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region File

    public IEnumerator LoadFiles(string _filepath)
    {
        // Stop everything
        //yield return this.StartCoroutine(this.Stop());

        CanvasManager.Instance.filePathInputField.interactable = false;
        CanvasManager.Instance.exampleAudioButton.interactable = false;

        // Get all mp3 file paths
        List<string> filePathList = new List<string>();
        if (this.IsValidPath(_filepath))
        {
            filePathList.AddRange(Directory.GetFiles(_filepath, "*.WAV"));
        }

        AudioManager.Instance.songs.Clear();
        yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.ClearSongSelection());

        int count = 0;

        foreach (string path in filePathList)
        {
            // Get & create audio clip
            WWW request = new WWW(path);
            yield return request;

            AudioManager.Instance.songs.Add(request.GetAudioClip());

            Debug.Log("Path: " + path);
            yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.AddSongSelectButton(count, path));
            count++;
        }

        CanvasManager.Instance.filePathInputField.interactable = true;
        CanvasManager.Instance.exampleAudioButton.interactable = true;

        // Re-start run routine
        //this.StartCoroutine(this.Run());

        yield return null;
    }

    public IEnumerator UseDemoFiles()
    {
        CanvasManager.Instance.filePathInputField.interactable = false;
        CanvasManager.Instance.exampleAudioButton.interactable = false;

        AudioManager.Instance.songs.Clear();
        yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.ClearSongSelection());

        // Set audio manager song list and add song select buttons
        for (int i = 0; i < AudioManager.Instance.exampleSongs.Count; i++)
        {
            AudioManager.Instance.songs.Add(AudioManager.Instance.exampleSongs[i]);

            yield return CanvasManager.Instance.StartCoroutine(CanvasManager.Instance.AddSongSelectButton(
                i, AudioManager.Instance.exampleSongs[i].name));
        }

        CanvasManager.Instance.filePathInputField.interactable = true;
        CanvasManager.Instance.exampleAudioButton.interactable = true;

        yield return null;
    }

    // Check if the file path is valid
    public bool IsValidPath(string _path)
    {
        if (Directory.Exists(_path))
            return true;

        return false;
    }

    // Check if the the file path contains audio files
    public bool HasAudioFiles(string _path)
    {
        if (Directory.GetFiles(_path, "*.WAV").Length != 0)
        {
            return true;
        }

        return false;
    }

    #endregion
}