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

    private const float MinDb = -80f;

    private void Start()
    {
        var masterVolume = PlayerPrefs.GetFloat("MasterVolume", 20f);
        var musicVolume = PlayerPrefs.GetFloat("MusicVolume", 20f);
        var sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 20f);

        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;
    }

    public void UpdateMasterVolume(float value)
    {
        masterMixerGroup.audioMixer.SetFloat("MasterVolume", ConvertSliderToDb(value));
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void UpdateMusicVolume(float value)
    {
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", ConvertSliderToDb(value));
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void UpdateSFXVolume(float value)
    {
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", ConvertSliderToDb(value));
        PlayerPrefs.SetFloat("SFXVolume", value);
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