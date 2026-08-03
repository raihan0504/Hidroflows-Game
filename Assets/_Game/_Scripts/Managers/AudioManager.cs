using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] AudioSource sfxSource;

    [Header("UI")]
    [SerializeField] AudioClip buttonClip;
    [SerializeField] AudioClip buttonHover;


    //[Header("Game")]
    //[SerializeField] AudioClip valveOpen;
    //[SerializeField] AudioClip waterFlow;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    #region UI
    public void PlayButtonClick()
    {
        Play(buttonClip);
    }

    public void PlayButtonHover()
    {
        Play(buttonHover);
    }
    #endregion
}
