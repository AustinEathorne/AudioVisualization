﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    // Members are public for editor scripts
    public AudioSource audioSource;

    public List<AudioClip> songs;


    #region Main

    public override IEnumerator Initialize()
    {
        this.isInitialized = false;

        // Set clip
        this.audioSource.clip = this.songs[0];

        // Yield audio peer initialization
        yield return AudioPeer.Instance.StartCoroutine(AudioPeer.Instance.Initialize());

        this.isInitialized = true;

        yield return null;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        // Start playing music
        this.audioSource.Play();

        // Start audio peer run routine
        AudioPeer.Instance.StartCoroutine(AudioPeer.Instance.Run());

        while (this.isRunning)
        {
            if(this.CheckForSongEnd())
            {
                this.audioSource.Stop();
                this.audioSource.time = 0.0f;
                CanvasManager.Instance.SetPlayButtonSprite();
            }

            yield return null;
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        this.audioSource.Stop();

        this.isRunning = false;
        yield return null;
    }

    #endregion

    #region Audio

    public void ChangeSong(int _index)
    {
        this.audioSource.Stop();

        AudioPeer.Instance.StartCoroutine(AudioPeer.Instance.ResetValues());

        this.audioSource.clip = this.songs[_index];
        this.audioSource.time = 0.0f;

        this.audioSource.Play();
    }

    public void PlayMusic()
    {
        this.audioSource.Play();
    }

    public void PauseMusic()
    {
        this.audioSource.Pause();
    }

    public void Search(float _time)
    {
        if (_time >= this.audioSource.clip.length)
            return;

        this.audioSource.time = _time;
    }

    public void SetStereoPan(Channel _channel)
    {
        switch (_channel)
        {
            case Channel.Stereo:
                this.audioSource.panStereo = 0;
                break;

            case Channel.Left:
                this.audioSource.panStereo = -1;
                break;

            case Channel.Right:
                this.audioSource.panStereo = 1;
                break;

            default:
                break;
        }
    }

    public void SetVolume(float _value)
    {
        this.audioSource.volume = _value;
    }

    public bool CheckForSongEnd()
    {
        if (this.audioSource.time >= this.audioSource.clip.length)
        {
            return true;
        }

        return false;
    }

    #endregion
}
