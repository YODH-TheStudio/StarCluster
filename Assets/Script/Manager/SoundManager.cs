using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public enum SoundType
{
    None,
    Woaw
}


[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] _soundList;
    private static SoundManager _instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }


    public static void PlaySound(SoundType sound , float volume = 1 ) 
    {
        AudioClip[] _clips = _instance._soundList[(int)sound]._Sounds;
        AudioClip _randomclip =  _clips[UnityEngine.Random.Range(0,_clips.Length)];

        _instance._audioSource.PlayOneShot(_randomclip, volume);
        Debug.LogError(_randomclip);
       // _instance._audioSource.PlayOneShot(_instance._soundList[(int)sound], volume); 
    }

#if UNITY_EDITOR

    private void OnEnable()
    {
        string[] _names = Enum.GetNames( typeof( SoundType ) );
        Array.Resize(ref _soundList, _names.Length);
        for (int i = 0; i < _soundList.Length; i++)
        {
            _soundList[i]._name = _names[i];
        }
    }

#endif

}

[Serializable]
public struct SoundList 
{
    public AudioClip[] _Sounds { get => _sounds; }
    [HideInInspector] public string _name;
    [SerializeField] private AudioClip[] _sounds;
}
