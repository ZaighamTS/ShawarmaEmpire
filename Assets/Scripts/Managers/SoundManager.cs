using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip cashCollectClip;
    [SerializeField] private AudioClip upgradeClip;
    [SerializeField] private AudioClip shawarmaClip;

    private void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

   

    public void PlayButtonClick()
    {
        PlaySound(buttonClickClip);
    }

    public void PlayCashCollect()
    {
        PlaySound(cashCollectClip);
    }

    public void PlayUpgrade()
    {
        PlaySound(upgradeClip);
    }
    public void PlayShwaramaClick()
    {
        PlaySound(shawarmaClip);
    }
   
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
