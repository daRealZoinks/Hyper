using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    const float MinDb = -80f;

    public void UpdateMasterVolume(float value)
    {
        masterMixerGroup.audioMixer.SetFloat("MasterVolume", ConvertSliderToDb(value));
    }

    public void UpdateMusicVolume(float value)
    {
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", ConvertSliderToDb(value));
    }

    public void UpdateSFXVolume(float value)
    {
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", ConvertSliderToDb(value));
    }

    private float ConvertSliderToDb(float sliderValue)
    {
        if (sliderValue <= 0f)
        {
            return MinDb;
        }

        var normalized = Mathf.Clamp01(sliderValue / 20f);
        return Mathf.Log10(normalized) * 20f;
    }
}