using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private SoundSystem _soundSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
    }

    public void Play(string SFXKey)
    {
        _soundSystem.PlaySoundFXClipByKey(SFXKey);
    }
}
