using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Sensitivity Controls")]
    public MouseLook mouseLook;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;
    [Header("Master Volume Controls")]
    public AudioMixer mixer;
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    [Header("Music Volume Controls")]
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    [Header("Effects Volume Controls")]
    public Slider effectsVolumeSlider;
    public TextMeshProUGUI effectsVolumeText;

    // Start is called before the first frame update
    void Start()
    {
        float sens = mouseLook.sensitivity.x;
        SetSliderUIValue(sens, sensitivitySlider, sensitivityText);

        float masterVolume, musicVolume, effectsVolume;
        mixer.GetFloat("MasterVolume", out masterVolume);
        mixer.GetFloat("MusicVolume", out musicVolume);
        mixer.GetFloat("EffectsVolume", out effectsVolume);

        SetSliderUIValue(ConvertVolumeToSlider(masterVolume), masterVolumeSlider, masterVolumeText);
        SetSliderUIValue(ConvertVolumeToSlider(musicVolume), musicVolumeSlider, musicVolumeText);
        SetSliderUIValue(ConvertVolumeToSlider(effectsVolume), effectsVolumeSlider, effectsVolumeText);
    }

    void SetSliderUIValue(float value, Slider slider, TextMeshProUGUI text) {
        slider.value = value;
        text.SetText(value.ToString());
    }

    public void OnSensitivityChange() {
        float value = (float) System.Math.Round(sensitivitySlider.value, 2);
        sensitivityText.SetText(value.ToString());
        mouseLook.SetSensitivity(value);
    }

    public void OnMasterVolumeChange() {
        float value = masterVolumeSlider.value;
        masterVolumeText.SetText(value.ToString());
        mixer.SetFloat("MasterVolume", ConvertSliderToVolume(value));
    }

    public void OnMusicVolumeChange() {
        float value = musicVolumeSlider.value;
        musicVolumeText.SetText(value.ToString());
        mixer.SetFloat("MusicVolume", ConvertSliderToVolume(value));
    }
    public void OnEffectsVolumeChange() {
        float value = effectsVolumeSlider.value;
        effectsVolumeText.SetText(value.ToString());
        mixer.SetFloat("EffectsVolume", ConvertSliderToVolume(value));
    }

    float ConvertSliderToVolume(float value) {
        return Mathf.Round((float) NumberUtils.Map(value, 0, 150, -80, 20));
    }

    float ConvertVolumeToSlider(float volume) {
        return Mathf.Round((float) NumberUtils.Map(volume, -80, 20, 0, 150));
    }
}
