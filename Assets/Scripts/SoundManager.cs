using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SFX")]
    public AudioClip moveClip;
    public AudioClip dropClip;
    public AudioClip hardDropClip;
    public AudioClip rotateClip;
    public AudioClip lineClearClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMove()
    {
        Play(moveClip);
    }

    public void PlaySoftDrop()
    {
        Play(dropClip);
    }

    public void PlayHardDrop()
    {
        Play(hardDropClip);
    }

    public void PlayRotate()
    {
        Play(rotateClip);
    }

    private void Play(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void PlayLineClear()
    {
        Play(lineClearClip);
    }
}
