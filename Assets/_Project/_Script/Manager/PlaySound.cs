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
        _soundSystem.GetSFXMixerGroup();
       
    }
    #endregion

    #region Play Sond button 
    public void Play(string sfxKey)
    {
        _soundSystem.PlaySoundFXClipByKey(sfxKey, transform.position);
        
    }
    
     public void ChangeMusic(string musicKey)
     {
         _soundSystem.ChangeMusicByKey(musicKey);
     }
    
    #endregion
    
}
