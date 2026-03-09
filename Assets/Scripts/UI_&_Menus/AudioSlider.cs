using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioSlider : MonoBehaviour
{
    [SerializeField]
    private AudioMixer Mixer;
    [SerializeField]
    private AudioSource AudioSource;
    [SerializeField]
    private TextMeshProUGUI ValueText;
    [SerializeField]
    private AudioMixMode MixMode;

    [SerializeField]
    private AudioManagerVolumeTarget managerTarget = AudioManagerVolumeTarget.None;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnChangeSlider);
            OnChangeSlider(slider.value);
        }
    }

    private void OnDisable()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnChangeSlider);
        }
    }

    public void OnChangeSlider(float rawValue)
    {
        if (ValueText != null)
        {
            ValueText.SetText($"{Mathf.RoundToInt(rawValue)}");
        }

        float value = Normalize(rawValue); // 0..1 for actual volume

        if (managerTarget != AudioManagerVolumeTarget.None)
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("AudioSlider: AudioManager.Instance is null.");
                return;
            }

            if (managerTarget == AudioManagerVolumeTarget.Music)
            {
                AudioManager.Instance.SetMusicVolume(value);
            }
            else if (managerTarget == AudioManagerVolumeTarget.Sfx)
            {
                AudioManager.Instance.SetSfxVolume(value);
            }

            return;
        }

        switch (MixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                if (AudioSource != null)
                {
                    AudioSource.volume = value;
                }
                break;

            case AudioMixMode.LinearMixerVolume:
                if (Mixer != null)
                {
                    Mixer.SetFloat("Volume", -80f + value * 100f);
                }
                break;

            case AudioMixMode.LogarithmicMixerVolume:
                if (Mixer != null)
                {
                    float safeValue = Mathf.Max(0.0001f, value);
                    Mixer.SetFloat("Volume", Mathf.Log10(safeValue) * 20f);
                }
                break;
        }
    }

    private float Normalize(float rawValue)
    {
        if (slider == null || Mathf.Approximately(slider.maxValue, slider.minValue))
        {
            return Mathf.Clamp01(rawValue);
        }

        return Mathf.InverseLerp(slider.minValue, slider.maxValue, rawValue);
    }

    public enum AudioMixMode
    {
        LinearAudioSourceVolume,
        LinearMixerVolume,
        LogarithmicMixerVolume
    }

    public enum AudioManagerVolumeTarget
    {
        None,
        Music,
        Sfx
    }
}
