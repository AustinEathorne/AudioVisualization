using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingsPanelBase : Mono<SettingsPanelBase>
{
    [Header("Panel")]
    public GameObject settingsPanel;
    public CanvasGroup canvasGroup;
    public float panelFadeTime;

    public void TogglePanel()
    {
        // Turn on
        if (!this.settingsPanel.activeSelf)
        {
            // Turn on panel
            this.settingsPanel.SetActive(true);

            // Fade in panel
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.canvasGroup, this.panelFadeTime, 1.0f));

            // Enable panel
            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = true;
        }
        // Turn off
        else
        {
            // Disable panel
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;

            // Fade out panel
            UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.canvasGroup, this.panelFadeTime, 0.0f));

            // Turn off panel
            this.settingsPanel.SetActive(false);
        }
    }
}
