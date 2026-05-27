using UnityEngine;

public class UISFXManager : MonoBehaviour
{
    public static UISFXManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("UI SFX")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip dialogOpenClip;
    [SerializeField] private AudioClip dialogCloseClip;
    [SerializeField] private AudioClip confirmClip;
    [SerializeField] private AudioClip cancelClip;
    [SerializeField] private AudioClip selectClip;

    private void Awake()
    {
        Instance = this;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayHover() => Play(hoverClip);
    public void PlayClick() => Play(clickClip);
    public void PlayDialogOpen() => Play(dialogOpenClip);
    public void PlayDialogClose() => Play(dialogCloseClip);
    public void PlayConfirm() => Play(confirmClip);
    public void PlayCancel() => Play(cancelClip);
    public void PlaySelect() => Play(selectClip); 
    private void Play(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.PlayOneShot(clip);
    }
}