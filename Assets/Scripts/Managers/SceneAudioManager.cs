using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;          // Assign a new clip to play, or leave null to stay silent
    [SerializeField] private float volume;
    [SerializeField] private bool stopPrevMusic;   // If true, stops current music regardless

    void Start()
    {
        if (stopPrevMusic)
        {
            AudioManager.Instance.StopMusic();
        }
        
        AudioManager.Instance.PlayMusic(musicClip, volume);
    }
}
