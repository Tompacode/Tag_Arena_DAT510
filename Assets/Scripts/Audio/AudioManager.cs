using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField]
    private AudioMixer masterMixer;
    [SerializeField]
    private AudioMixerGroup musicMixerGroup;
    [SerializeField]
    private AudioMixerGroup sfxMixerGroup;

    [Header("Exposed Parameters")]
    [SerializeField]
    private string musicVolumeParam = "Volume (Of Music)";
    [SerializeField]
    private string sfxVolumeParam = "Volume (Of SFX)";

    [Header("Sources")]
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource sfxSource;

    private const string MusicVolumeKey = "audio_music_volume";
    private const string SfxVolumeKey = "audio_sfx_volume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        ApplySavedVolumes();
    }

    public void SetMusicVolume(float linearValue)
    {
        linearValue = Mathf.Clamp01(linearValue);
        if (masterMixer != null)
        {
            masterMixer.SetFloat(musicVolumeParam, LinearToDb(linearValue));
        }

        PlayerPrefs.SetFloat(MusicVolumeKey, linearValue);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float linearValue)
    {
        linearValue = Mathf.Clamp01(linearValue);
        if (masterMixer != null)
        {
            masterMixer.SetFloat(sfxVolumeParam, LinearToDb(linearValue));
        }

        PlayerPrefs.SetFloat(SfxVolumeKey, linearValue);
        PlayerPrefs.Save();
    }

    private void ApplySavedVolumes()
    {
        SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
        SetSfxVolume(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));
    }

    private static float LinearToDb(float value)
    {
        return Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f;
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.volume = Mathf.Clamp01(volume);
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public void PlaySfxAtPoint(AudioClip clip, Vector3 worldPosition, float volume = 1f)
    {
        if (clip == null)
        {
            return;
        }

        GameObject go = new GameObject("TempSfx");
        go.transform.position = worldPosition;

        AudioSource source = go.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.spatialBlend = 1f;
        source.clip = clip;
        source.volume = Mathf.Clamp01(volume);
        source.Play();

        Destroy(go, clip.length + 0.1f);
    }
}