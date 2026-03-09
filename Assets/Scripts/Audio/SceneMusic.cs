using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicClip;
    [SerializeField]
    [Range(0f, 1f)]
    private float volume = 1f;

    private void Start()
    {
        if (AudioManager.Instance != null && musicClip != null)
        {
            AudioManager.Instance.PlayMusic(musicClip, volume);
        }
    }
}