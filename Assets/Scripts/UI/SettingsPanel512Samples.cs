using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel512Samples : SettingsPanelBase
{
    [Header("Vis Manager")]
    public Visualization512Samples visualization;

    [Header("SettingsPanel")]
    public Slider minScaleSlider;
    public Text minScaleText;
    public Slider maxScaleSlider;
    public Text maxScaleText;
    public Slider scaleMultiplierSlider;
    public Text scaleMultiplierText;
    public Dropdown channelDropdown;

    #region Main

    public override IEnumerator Initialize()
    {
        this.minScaleSlider.minValue = this.visualization.minMinScale;
        this.minScaleSlider.maxValue = this.visualization.maxMinScale;
        this.minScaleSlider.value = this.visualization.minScale;
        this.minScaleText.text = this.visualization.minScale.ToString();

        this.maxScaleSlider.minValue = this.visualization.minMaxScale;
        this.maxScaleSlider.maxValue = this.visualization.maxMaxScale;
        this.maxScaleSlider.value = this.visualization.maxScale;
        this.maxScaleText.text = this.visualization.maxScale.ToString();

        this.scaleMultiplierSlider.minValue = this.visualization.minScaleMultiplier;
        this.scaleMultiplierSlider.maxValue = this.visualization.maxScaleMultiplier;
        this.scaleMultiplierSlider.value = this.visualization.scaleMultiplier;
        this.scaleMultiplierText.text = this.visualization.scaleMultiplier.ToString();

        if (AudioManager.Instance.audioSource.panStereo == 0)
        {
            this.channelDropdown.value = 0;
        }
        else if (AudioManager.Instance.audioSource.panStereo == -1)
        {
            this.channelDropdown.value = 1;
        }
        else
        {
            this.channelDropdown.value = 2;
        }

        yield return null;
    }

    public override IEnumerator Run()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Stop()
    {
        this.settingsPanel.SetActive(false);
        yield return null;
    }

    #endregion

    #region Events

    public void OnMinScaleUpdate(float _value)
    {
        this.minScaleText.text = _value.ToString();
        this.visualization.SetMinScale(_value);
    }

    public void OnMaxScaleUpdate(float _value)
    {
        this.maxScaleText.text = _value.ToString();
        this.visualization.SetMaxScale(_value);
    }

    public void OnMultiplierUpdate(float _value)
    {
        this.scaleMultiplierText.text = _value.ToString();
        this.visualization.SetScaleMultiplier(_value);
    }

    public void OnChannelSelect(int _channel)
    {
        this.visualization.channel = (Channel)_channel;
        AudioManager.Instance.SetStereoPan((Channel)_channel);
    }

    #endregion
}
