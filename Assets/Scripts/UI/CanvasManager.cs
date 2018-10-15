﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("StartScreen")]
    public CanvasGroup startScreenContentGroup;
    public GameObject startSceenContainer;
    public Text startScreenErrorText;
    public Image startScreenBG;

    public float startScreenFadeTime;

    [Header("SidePanels")]
    public GameObject visualizationPanel;
    public GameObject libraryPanel;
    public List<SettingsPanelBase> settingsPanelList;

    [Header("SideButtons")]
    public float sideButtonFadeTime;
    public float sideButtonMoveTime;
    public List<Vector2> sideButtonPositions;
    public List<RectTransform> sideButtonTransforms;
    public List<Image> sideButtonImages;

    [Header("Search Bar")]
    public Slider searchBar;
    public bool isSearching;

    [Header("Play Button")]
    public Image playButtonImage;
    public List<Sprite> playButtonSprites;

    [Header("Time Text")]
    public Text currentTimeText;
    public Text totalTimeText;

    [Header("Volume")]
    public float[] volumeImageThresholds = new float[3];
    public List<Sprite> volumeSpriteList;
    public Image volumeImage;
    public Slider volumeSlider;

    [Header("Song Select")]
    public RectTransform songSelectContainer;
    public List<GameObject> songSelectButtonList;
    public List<Text> songSelectTextList;

    [Header("Escape Container")]
    public GameObject escapeContainer;

    [Header("ToggledContainers")]
    public CanvasGroup sideButtonGroup;
    public CanvasGroup mediaControlsGroup;
    public CanvasGroup visualizationGroup;
    public CanvasGroup libraryGroup;
    public CanvasGroup titleGroup;
    public CanvasGroup colourPickerGroup;

    public float sidePanelFadeTime;
    public float demoUiFadeOutTime;
    public float demoUiFadeInTime;
    public bool isDemoUiActive;

    [Header("DemoTitles")]
    public Text songTitleText;
    public Text visTitleText;
    public List<string> visNameList;

    [Header("FilePath")]
    public InputField filePathInputField;
    public Button exampleAudioButton;
    public Text errorText;
    public string lastFilePath = "";



    #region Main

    public override IEnumerator Initialize()
    {
        this.isInitialized = false;

        this.searchBar.value = 0.0f;

        this.isInitialized = true;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        yield return this.StartCoroutine(this.SetupUI());

        while (this.isRunning)
        {
            // update search bar if we're not searching
            if (!isSearching)
            {
                this.searchBar.value = AudioManager.Instance.audioSource.time;
            }

            this.SetTimeText();

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

    #region Events

    public void OnSettingsClick()
    {
        this.StartCoroutine(this.ToggleSettingsPanel());
    }

    public void OnVisualizationClick()
    {
        this.StartCoroutine(this.ToggleVisualizationPanel());
    }

    public void OnLibraryClick()
    {
        this.StartCoroutine(this.ToggleLibraryPanel());
    }


    public void OnSearchBarSelect()
    {
        this.StartCoroutine(this.SearchRoutine());
    }

    public void OnSearchBarRelease()
    {
        this.isSearching = false;
    }


    public void OnVisualizationSelectClick(int _index)
    {
        if (_index == DemoManager.Instance.currentVisualization)
            return;

        DemoManager.Instance.StartCoroutine(DemoManager.Instance.ChangeVisualization(_index));
    }


    public void OnPlayClick()
    {
        if (AudioManager.Instance.audioSource.isPlaying)
        {
            AudioManager.Instance.PauseMusic();
        }
        else
        {
            AudioManager.Instance.PlayMusic();
        }

        this.SetPlayButtonSprite();
    }

    public void OnVolumeClick()
    {
        AudioManager.Instance.audioSource.mute = !AudioManager.Instance.audioSource.mute;
        this.SetVolumeSprite();
    }

    public void OnVolumeUpdate(float _value)
    {
        // Update volume
        AudioManager.Instance.SetVolume(_value);

        // Update volume image
        this.SetVolumeSprite();
    }

    public void OnSongChangeClick(int _direction)
    {
        AudioManager.Instance.ChangeSong(AudioManager.Instance.currentSong + _direction);
        this.ResetSearchBar();
        this.UpdateDemoTitleText();
    }


    public void OnQuitClick()
    {
        DemoManager.Instance.StartCoroutine(DemoManager.Instance.Quit());
    }


    public void OnFilePathUpdate(string _filePath)
    {
        if (_filePath == this.lastFilePath)
            return;

        this.lastFilePath = _filePath;

        this.StartCoroutine(this.ClearSongSelection());

        // Check for file path
        if (!DemoManager.Instance.IsValidPath(_filePath))
        {
            // Set error text
            this.errorText.text = "Please enter a valid file path";

            // Bring up error text
            this.errorText.gameObject.SetActive(true);

            return;
        }

        // Check for audio files at path
        if (!DemoManager.Instance.HasAudioFiles(_filePath))
        {
            // Set error text
            this.errorText.text = "Please enter a path to a folder that contains .wav files";

            // Bring up error text
            this.errorText.gameObject.SetActive(true);

            return;
        }

        this.errorText.gameObject.SetActive(false);
        DemoManager.Instance.StartCoroutine(DemoManager.Instance.LoadFiles(_filePath));
    }

    public void OnExampleAudioClick()
    {
        this.lastFilePath = "";
        this.errorText.gameObject.SetActive(false);
        DemoManager.Instance.StartCoroutine(DemoManager.Instance.UseDemoFiles());
    }

    public void OnSongSelectClick(int _index)
    {
        //Debug.Log("Select Song: " + _index.ToString());
        AudioManager.Instance.ChangeSong(_index);
        this.ResetSearchBar();
        this.UpdateDemoTitleText();
    }


    public void OnStartFilePathUpdate(string _filePath)
    {
        if (_filePath == this.lastFilePath)
            return;

        this.lastFilePath = _filePath;

        // Check for file path
        if (!DemoManager.Instance.IsValidPath(_filePath))
        {
            // Set error text
            this.startScreenErrorText.text = "Please enter a valid file path";

            // Bring up error text
            this.startScreenErrorText.gameObject.SetActive(true);

            return;
        }

        // Check for audio files at path
        if (!DemoManager.Instance.HasAudioFiles(_filePath))
        {
            // Set error text
            this.startScreenErrorText.text = "Please enter a path to a folder that contains .wav files";

            // Bring up error text
            this.startScreenErrorText.gameObject.SetActive(true);

            return;
        }

        DemoManager.Instance.StartCoroutine(DemoManager.Instance.StartDemo(_filePath));
    }

    public void OnStartExampleAudioClick()
    {
        this.lastFilePath = "";
        DemoManager.Instance.StartCoroutine(DemoManager.Instance.StartDemo());
    }


    public void OnChannelBarUpdate(float _value)
    {
        AudioManager.Instance.audioSource.panStereo = _value;
    }


    public void OnColourPanelToggle(bool _isActive)
    {
        this.sideButtonImages[0].enabled = _isActive ? false : true;

        this.settingsPanelList[DemoManager.Instance.currentVisualization].OnColourPanelToggle(_isActive);
    }

    public void OnBaseColourChange(Color _color)
    {
        DemoManager.Instance.baseEmissionColor = _color;
    }

    #endregion

    #region UI

    public IEnumerator SetupUI()
    {
        // Title text
        this.UpdateDemoTitleText();

        // Search bar
        this.ResetSearchBar();

        // Volume
        this.volumeSlider.value = AudioManager.Instance.audioSource.volume;
        this.SetVolumeSprite();

        // Settings panel
        yield return this.settingsPanelList[DemoManager.Instance.currentVisualization].StartCoroutine(this.settingsPanelList[DemoManager.Instance.currentVisualization].Initialize());

        this.isDemoUiActive = true;

        yield return null;
    }


    public IEnumerator CloseStartScreen(bool _isUsingExampleAudio, string _filePath)
    {
        this.startScreenContentGroup.interactable = false;

        UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.startScreenContentGroup, this.startScreenFadeTime, 0.0f));
        this.startScreenErrorText.gameObject.SetActive(false);

        yield return new WaitForSeconds(this.startScreenFadeTime);

        UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.startScreenBG, this.startScreenFadeTime, 0.0f));
        this.startScreenBG.raycastTarget = false;

        this.startScreenContentGroup.blocksRaycasts = false;

        yield return null;
    }
    

    public IEnumerator SearchRoutine()
    {
        if (this.isSearching)
            yield break;

        this.isSearching = true;

        AudioManager.Instance.PauseMusic();
        this.SetPlayButtonSprite();

        while (this.isSearching)
        {
            AudioManager.Instance.Search(this.searchBar.value);

            yield return null;
        }

        AudioManager.Instance.PlayMusic();
        this.SetPlayButtonSprite();

        yield return null;
    }

    public void ResetSearchBar()
    {
        this.searchBar.maxValue = AudioManager.Instance.songs[AudioManager.Instance.currentSong].length;
        this.searchBar.value = 0.0f;

        float totalTime = this.searchBar.maxValue;
        int minutes = Mathf.FloorToInt(totalTime / 60F);
        int seconds = Mathf.FloorToInt(totalTime - minutes * 60);
        this.totalTimeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void SetVolumeSprite()
    {
        // Check if the audio is muted
        if (AudioManager.Instance.audioSource.mute)
        {
            this.volumeImage.sprite = this.volumeSpriteList[0];
            return;
        }

        float volume = AudioManager.Instance.audioSource.volume;
        for (int i = 0; i < this.volumeImageThresholds.Length; i++)
        {
            if (volume <= this.volumeImageThresholds[i])
            {
                this.volumeImage.sprite = this.volumeSpriteList[i + 1]; // 0 is volume mute
                i = this.volumeImageThresholds.Length;
            }
        }
    }

    public void SetPlayButtonSprite()
    {
        if (AudioManager.Instance.audioSource.isPlaying)
        {
            this.playButtonImage.sprite = this.playButtonSprites[1];
        }
        else
        {
            this.playButtonImage.sprite = this.playButtonSprites[0];
        }
    }

    public void SetTimeText()
    {
        float time = AudioManager.Instance.audioSource.time;
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        this.currentTimeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }


    public void ToggleEscapeContainer()
    {
        this.escapeContainer.SetActive(!this.escapeContainer.activeSelf);
    }


    public IEnumerator ToggleSettingsPanel()
    {
        if (this.settingsPanelList[DemoManager.Instance.currentVisualization].settingsPanel.activeSelf)
        {
            this.settingsPanelList[DemoManager.Instance.currentVisualization].TogglePanel();
            yield return this.StartCoroutine(this.ShiftSideButtons(0, false));
        }
        else
        {
            yield return this.StartCoroutine(this.ShiftSideButtons(0, true));
            this.settingsPanelList[DemoManager.Instance.currentVisualization].TogglePanel();
        }

        yield return null;
    }

    public IEnumerator ToggleVisualizationPanel()
    {
        if (this.visualizationPanel.activeSelf)
        {
            
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.visualizationGroup, this.sidePanelFadeTime, 0.0f));
            this.visualizationPanel.SetActive(false);

            yield return this.StartCoroutine(this.ShiftSideButtons(1, false));
        }
        else
        {
            yield return this.StartCoroutine(this.ShiftSideButtons(1, true));

            this.visualizationPanel.SetActive(true);
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.visualizationGroup, this.sidePanelFadeTime, 1.0f));
        }

        yield return null;
    }

    public IEnumerator ToggleLibraryPanel()
    {
        if (this.libraryPanel.activeSelf)
        {
            this.libraryPanel.SetActive(false);
            yield return this.StartCoroutine(this.ShiftSideButtons(2, false));
        }
        else
        {
            yield return this.StartCoroutine(this.ShiftSideButtons(2, true));
            this.libraryPanel.SetActive(true);
        }

        yield return null;
    }

    public IEnumerator ShiftSideButtons(int _selectedButton, bool _isOpeningPanel)
    {
        if (_isOpeningPanel)
        {
            // Fade out not selected buttons
            for (int i = 0; i < this.sideButtonImages.Count; i++)
            {
                this.sideButtonTransforms[i].GetComponent<Button>().interactable = false;

                if (i == _selectedButton)
                    continue;

                yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.sideButtonImages[i], this.sideButtonFadeTime, 0.0f));
            }

            // move selected button to selected position
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.MoveTransformOverTime(this.sideButtonTransforms[_selectedButton], this.sideButtonPositions[0], this.sideButtonMoveTime));

            this.sideButtonTransforms[_selectedButton].GetComponent<Button>().interactable = true;
        }
        else
        {
            this.sideButtonTransforms[_selectedButton].GetComponent<Button>().interactable = false;

            // Move selected button back to original position
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.MoveTransformOverTime(this.sideButtonTransforms[_selectedButton], this.sideButtonPositions[_selectedButton], this.sideButtonMoveTime));

            // Fade in not selected buttons
            for (int i = 0; i < this.sideButtonImages.Count; i++)
            {
                if (i != _selectedButton)
                {
                    yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.sideButtonImages[i], this.sideButtonFadeTime, 1.0f));
                }

                this.sideButtonTransforms[i].GetComponent<Button>().interactable = true;
            }
        }

        yield return null;
    }


    public IEnumerator ChangeVisualization(int _index)
    {
        this.settingsPanelList[DemoManager.Instance.currentVisualization].StartCoroutine(
            this.settingsPanelList[DemoManager.Instance.currentVisualization].Stop());

        this.settingsPanelList[_index].StartCoroutine(this.settingsPanelList[_index].Initialize());

        this.UpdateDemoTitleText(_index);

        yield return null;
    }


    public IEnumerator ClearSongSelection()
    {
        foreach (GameObject obj in this.songSelectButtonList)
        {
            ObjectPoolManager.Instance.ReturnObject(PooledObject.SongSelectButton, obj);
        }

        this.songSelectButtonList.Clear();
        this.songSelectTextList.Clear();

        yield return null;
    }

    public IEnumerator AddSongSelectButton(int _index, string _songTitle)
    {
        // Get new song select button and its text component
        this.songSelectButtonList.Add(ObjectPoolManager.Instance.GetPooledObject(PooledObject.SongSelectButton));
        this.songSelectTextList.Add(this.songSelectButtonList[_index].GetComponentInChildren<Text>());

        // Set text
        this.songSelectTextList[_index].text = _songTitle;

        // Set position
        RectTransform rt = this.songSelectButtonList[_index].GetComponent<RectTransform>();
        rt.SetParent(this.songSelectContainer);
        rt.localPosition = Vector3.zero;

        // Update button OnClick event
        Button button = this.songSelectButtonList[_index].GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => this.OnSongSelectClick(_index));

        //Debug.Log("Added button for song: " + _index.ToString());

        yield return null;
    }


    public IEnumerator ToggleDemoUI(bool _isActive)
    {
        if (_isActive)
        {
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.titleGroup, this.demoUiFadeInTime, 1.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.sideButtonGroup, this.demoUiFadeInTime, 1.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.mediaControlsGroup, this.demoUiFadeInTime, 1.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.settingsPanelList[DemoManager.Instance.currentVisualization].canvasGroup, this.demoUiFadeInTime, 1.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.visualizationGroup, this.demoUiFadeInTime, 1.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
               this.colourPickerGroup, this.demoUiFadeInTime, 1.0f));
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.libraryGroup, this.demoUiFadeInTime, 1.0f));
        }
        else
        {
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.titleGroup, this.demoUiFadeOutTime, 0.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.sideButtonGroup, this.demoUiFadeOutTime, 0.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.mediaControlsGroup, this.demoUiFadeOutTime, 0.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.settingsPanelList[DemoManager.Instance.currentVisualization].canvasGroup, this.demoUiFadeOutTime, 0.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.visualizationGroup, this.demoUiFadeOutTime, 0.0f));
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.colourPickerGroup, this.demoUiFadeOutTime, 0.0f));
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(
                this.libraryGroup, this.demoUiFadeOutTime, 0.0f));
        }

        this.isDemoUiActive = _isActive;

        yield return null;
    }


    public void UpdateDemoTitleText(int _visIndex)
    {
        this.songTitleText.text = AudioManager.Instance.audioSource.clip.name;
        this.visTitleText.text = this.visNameList[_visIndex];
    }

    public void UpdateDemoTitleText()
    {
        this.songTitleText.text = AudioManager.Instance.audioSource.clip.name;
        this.visTitleText.text = this.visNameList[DemoManager.Instance.currentVisualization];
    }

    #endregion
}
