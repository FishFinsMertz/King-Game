using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitchRange = 1f)
    {
        audioSource.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
        audioSource.PlayOneShot(clip, volume); 
    }

    public void PlaySFXLoop(AudioClip clip, float volume = 1f, float pitchRange = 1f)
    {
        audioSource.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
        audioSource.clip = clip;  
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopSFX()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }
}
