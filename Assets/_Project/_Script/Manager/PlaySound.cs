using UnityEngine;

public class PlaySound : MonoBehaviour
{

    #region  Fields

    private SoundSystem _soundSystem;
    
    #endregion
    
    #region Main Functions
    void Start()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
    }
    #endregion

    #region Play Sond button 
    public void Play(string sfxKey)
    {
        _soundSystem.PlaySoundFXClipByKey(sfxKey);
    }
    
     public void ChangeMusic(string musicKey)
     {
         _soundSystem.ChangeMusicByKey(musicKey);
     }
    
    #endregion
    
}
