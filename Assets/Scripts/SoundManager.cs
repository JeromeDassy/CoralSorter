using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource flipSound;
    [SerializeField] private AudioSource matchSound;
    [SerializeField] private AudioSource mismatchSound;
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private AudioSource deathCardSound;

    void Awake()
    {
        Instance = this;
    }

    public void PlayFlipSound()
    {
        flipSound.Play();
    }

    public void PlayMatchSound()
    {
        matchSound.Play();
    }

    public void PlayMismatchSound()
    {
        mismatchSound.Play();
    }

    public void PlayWinSound()
    {
        winSound.Play();
    }

    public void PlayGameOverSound()
    {
        gameOverSound.Play();
    }

    public void DeathCardSound()
    {
        deathCardSound.Play();
    }
}
