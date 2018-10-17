using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingsPanelBase : Mono<SettingsPanelBase>
{
    [Header("Panel")]
    public GameObject settingsPanel;
    public CanvasGroup canvasGroup;

    [Header("ColourPicker")]
    public CanvasGroup colourPickerGroup;



    public void TogglePanel()
    {
        // Turn off
        if (this.settingsPanel.activeSelf)
        {
            // Disable panel
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;

            // Fade out panel
            UIUtility.Instance.StartCoroutine(
                UIUtility.Instance.FadeOverTime(this.canvasGroup, CanvasManager.Instance.sidePanelFadeOutTime, 0.0f));

            // Turn off panel
            this.settingsPanel.SetActive(false);
        }
        // Turn on
        else
        {
            // Turn on panel
            this.settingsPanel.SetActive(true);

            // Fade in panel
            UIUtility.Instance.StartCoroutine(
                UIUtility.Instance.FadeOverTime(this.canvasGroup, CanvasManager.Instance.sidePanelFadeInTime, 1.0f));

            // Enable panel
            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = true;
        }
    }

    public IEnumerator ToggleColourPanel(bool _isActive)
    {
        // Turn on colour panel
        if (_isActive)
        {
            // Fade out and turn off main panel
            yield return UIUtility.Instance.StartCoroutine(
                UIUtility.Instance.FadeOverTime(this.canvasGroup, CanvasManager.Instance.sidePanelFadeInTime, 0.0f));
            this.canvasGroup.gameObject.SetActive(false);

            // Enable and turn on colour panel
            this.colourPickerGroup.gameObject.SetActive(true);
            this.colourPickerGroup.blocksRaycasts = true;
            this.colourPickerGroup.interactable = true;
            this.colourPickerGroup.alpha = 1;
        }
        // Turn off colour panel
        else
        {
            // Disable and turn off colour panel
            this.colourPickerGroup.blocksRaycasts = false;
            this.colourPickerGroup.interactable = false;
            this.colourPickerGroup.alpha = 0;
            this.colourPickerGroup.gameObject.SetActive(false);

            // Tur on and fade in main panel
            this.canvasGroup.gameObject.SetActive(true);
            yield return UIUtility.Instance.StartCoroutine(
                UIUtility.Instance.FadeOverTime(this.canvasGroup, CanvasManager.Instance.sidePanelFadeOutTime, 1.0f));

            // Enable main panel
            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = true;
        }

        yield return null;
    }
}
