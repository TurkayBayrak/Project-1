using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    
    private void Awake()
    {
        if (!instance)
            instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Sounds sound)
    {
        audioSource.clip = clips[(int)sound];
        audioSource.Play();
    }
}

public enum Sounds
{
    MarkSound,
    UnmarkSound,
    MatchSound
}