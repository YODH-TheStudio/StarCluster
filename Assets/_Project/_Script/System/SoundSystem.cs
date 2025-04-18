using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSystem : MonoBehaviour
{
    #region Classes
    public class SoundFX
    {
        public string Key;
        public List<AudioClip> Clip;
    }

    public class Track
    {
        public string Key;
        public AudioClip Clip;
    }

    #endregion

    #region Fields

    [SerializeField] private string sfxFolderPath;
    [SerializeField] private string musicFolderPath;
    [SerializeField] private string ambianceFolderPath;

    private int _numberOfChannels;

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup ambianceMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup; 
    private AudioListener _audioListener;

    [SerializeField] private AudioClip startingMusic;
    [SerializeField] private AudioClip startingAmbianceSound;

    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;

    private List<AudioSource> _audioSources;
    private AudioSource _currentMusicSource;
    private List<AudioSource> _currentAmbianceSources;

    [SerializeField] private AudioSource soundFXObject;

    [SerializeField] private SFXPoolManager sfxPoolManager;

    private PlayerScript Player => GameManager.Instance.GetPlayer();


    private readonly List<SoundFX> _sfxList = new List<SoundFX>();
    private readonly List<Track> _musicList = new List<Track>();
    private readonly List<Track> _ambianceList = new List<Track>();

    #endregion

    #region Main Functions

    //private void OnEnable()
    //{
    //    GameManager.Instance.OnChangeSceneEvent += SetAudioScene;
    //}
    private void SetAudioScene()
    {
        if (Camera.main != null) SetAudioListener(Camera.main.GetComponent<AudioListener>());
    }

    protected void Awake()
    {
        //base.Awake();

        GenerateKeys();
        _audioSources = new List<AudioSource>();
        _currentMusicSource = null;
        _currentAmbianceSources = new List<AudioSource>();
        _numberOfChannels = GetComponents<AudioSource>().Length;

        sfxPoolManager.SetMixerGroup(sfxMixerGroup);

        AudioSource[] attachedAudioSources = GetComponents<AudioSource>();

        for (int i = 0; i < _numberOfChannels; i++)
        {
            _audioSources.Add(attachedAudioSources[i]);
        }

    }

    private void Start()
    {
        LoadPlayerPrefs();
    }
    
    private void GenerateKeys()
    {
        GenerateSfxKeys();
        GenerateMusicKeys();
        GenerateAmbianceKeys();
    }

    private void GenerateSfxKeys()
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(sfxFolderPath);

        foreach (AudioClip audioClip in audioClips)
        {
            string[] words = audioClip.name.Split('_');

            if (words[0] != "SFX" || words.Length < 3)
            {
                Debug.LogWarning($"The audio clip {audioClip.name} has not the SFX_xxx_xxx format.");
            }

            string key = "";

            for (int i = 1; i < words.Length; i++)
            {
                key += char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower() + " ";
            }

            key = key.Trim();

            int index = key.Length - 1;

            while (char.IsDigit(key[index]))
                index--;

            key = key.Substring(0, index + 1);

            Debug.Log(key);

            SoundFX existingSound = _sfxList.Find(sound => sound.Key == key);



            if (existingSound != null)
            {
                existingSound.Clip.Add(audioClip);
            }
            else
            {
                SoundFX newSound = new SoundFX
                {
                    Key = key,
                    Clip = new List<AudioClip> { audioClip }
                };
                _sfxList.Add(newSound);
            }
        }
    }

    private void GenerateMusicKeys()
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(musicFolderPath);

        foreach (AudioClip audioClip in audioClips)
        {
            string[] words = audioClip.name.Split('_');

            if (words[0] != "MUSIC" || words.Length < 2)
            {
                Debug.LogWarning($"The audio clip {audioClip.name} has not the MUSIC_xxx format.");
            }

            string key = "";

            for (int i = 1; i < words.Length; i++)
            {
                key += char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower() + " ";
            }

            key = key.Trim();

            int index = key.Length - 1;

            while (char.IsDigit(key[index]))
                index--;

            key = key.Substring(0, index + 1);


            Track newTrack = new Track
            {
                Key = key,
                Clip = audioClip
            };

            _musicList.Add(newTrack);
        }
    }

    private void GenerateAmbianceKeys()
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(ambianceFolderPath);

        foreach (AudioClip audioClip in audioClips)
        {
            string[] words = audioClip.name.Split('_');

            if (words[0] != "AMBIANCE" || words.Length < 2)
            {
                Debug.LogWarning($"The audio clip {audioClip.name} has not the AMBIANCE_xxx format.");
            }

            string key = "";

            for (int i = 1; i < words.Length; i++)
            {
                key += char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower() + " ";
            }


            key = key.Trim();


            int index = key.Length - 1;

            while (char.IsDigit(key[index]))
                index--;

            key = key.Substring(0, index + 1);


            Track newTrack = new Track
            {
                Key = key,
                Clip = audioClip
            };

            _ambianceList.Add(newTrack);
        }
    }

    private void SetAudioListener(AudioListener audioListener)
    {
        _audioListener = audioListener;
    }


    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }

        _audioSources[0].Stop();
        return _audioSources[0];
    }

    private AudioClip GetSfxByKey(string key)
    {
        foreach (var sound in _sfxList)
        {
            if (sound.Key != key) continue;

            if (sound.Clip.Count <= 1) return sound.Clip[0];
            
            int randomIndex = Random.Range(0, sound.Clip.Count);
            return sound.Clip[randomIndex];

        }
        Debug.LogWarning($"SFX not found for key: {key}");
        return null;
    }

    private AudioClip GetMusicByKey(string key)
    {

        foreach (var sound in _musicList)
        {
            if (sound.Key == key)
            {
                return sound.Clip;
            }
        }
        Debug.LogWarning($"Music not found for key: {key}");
        return null;
    }

    private AudioClip GetAmbianceByKey(string key)
    {
        foreach (var sound in _ambianceList)
        {
            if (sound.Key == key)
            {
                return sound.Clip;
            }
        }
        Debug.LogWarning($"Ambiance not found for key: {key}");
        return null;
    }

    private void LoadPlayerPrefs()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        
        Debug.Log($"[SoundSystem] Loaded: Master={master}, Music={music}, SFX={sfx}");
        
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetAmbianceVolume(music);
        SetSfxVolume(sfx);
    }
    #endregion

    #region Master
    public void SetMasterVolume (float volume)
    {
        masterMixerGroup.audioMixer.SetFloat("MasterVolume", LinearToDecibel(volume));
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    #endregion
    
    #region Music

    private void ChangeMusic(AudioClip audioClip)
    {
        StartCoroutine(FadeOutInMusic(audioClip));
    }

    public void ChangeMusicByKey(string key)
    {
        ChangeMusic(GetMusicByKey(key));
    }

    public void SetMusicVolume (float volume)
    {
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", LinearToDecibel(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    #endregion

    #region Ambiances

    public void StopAmbianceSources()
    {
        foreach (AudioSource audioSource in _currentAmbianceSources)
        {
            StartCoroutine(FadeOutAudio(audioSource, fadeOutDuration));
        }

        _currentAmbianceSources.Clear();
    }

    private void AddAmbianceSound(AudioClip audioClip, float volume = 1f)
    {
        AudioSource audioSource = GetAvailableAudioSource();
        audioSource.outputAudioMixerGroup = ambianceMixerGroup;
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
        StartCoroutine(FadeInAudio(audioSource, fadeInDuration, volume));
        _currentAmbianceSources.Add(audioSource);
    }

    public void AddAmbianceSoundByKey(string key, float volume = 1f)
    {
        var audioClip = GetAmbianceByKey(key);
        if (audioClip != null)
        {
            AddAmbianceSound(audioClip, volume);
        }
    }

    public void SetAmbianceVolume(float volume)
    {
        ambianceMixerGroup.audioMixer.SetFloat("AmbianceVolume", LinearToDecibel(volume));
    }


    #endregion

    #region SoundFX

    private void PlaySoundFXClip(AudioClip audioClip, Vector3 spawnPosition, float volume = 1.0f)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("PlaySoundFXClip: AudioClip est null !");
            return;
        }

        sfxPoolManager.PlayClip(audioClip, spawnPosition, volume);
    }

    public void PlaySoundFXClipByKey(string key, Vector3 spawnPosition, float volume = 1.0f)
    {
        var audioClip = GetSfxByKey(key);
        if (audioClip != null)
        {
            Debug.Log($"Playing sound: {key}");
            PlaySoundFXClip(audioClip, spawnPosition, volume);
        }
    }

    //public void PlaySoundFXClipByKey(string key, float volume = 1.0f)
    //{
    //    AudioSource audioSource = GetAvailableAudioSource();
    //    AudioClip audioClip = GetSFXByKey(key);

    //    audioSource.clip = audioClip;
    //    audioSource.volume = volume;
    //    audioSource.Play();
    //}

    public void PlayRandomSoundFXClipByKeys(string[] keys, Vector3 spawnPosition, float volume = 1.0f)
    {
        List<AudioClip> clips = new List<AudioClip>();
        foreach (var key in keys)
        {
            var clip = GetSfxByKey(key);
            if (clip != null)
            {
                clips.Add(clip);
            }
        }

        if (clips.Count > 0)
        {
            int rand = Random.Range(0, clips.Count);
            PlaySoundFXClip(clips[rand], spawnPosition, volume);
        }
    }

    //private AudioSource CreateSoundFXSource(Vector3 spawnPosition)
    //{
    //    AudioSource audioSource = Instantiate(soundFXObject, spawnPosition, Quaternion.identity);
    //    return audioSource;
    //}

    //private void PlayAndDestroy(AudioSource audioSource, float clipLength)
    //{
    //    audioSource.Play();
    //    Destroy(audioSource.gameObject, clipLength);
    //}

    public void SetSfxVolume(float volume)
    {
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", LinearToDecibel(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }


    #endregion

    #region ConvertDecibel
    // Convert a value of 0.0001 to 1.0 in dB (useful for AudioMixer)
    private float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f)
            return -80f; // Silence
        return Mathf.Log10(linear) * 20f;
    }

    #endregion

    #region Coroutines
    private IEnumerator FadeOutInMusic(AudioClip newClip)
    {
        if (_currentMusicSource != null)
        {
            yield return StartCoroutine(FadeOutAudio(_currentMusicSource, fadeOutDuration));
        }

        AudioSource newMusicSource = GetAvailableAudioSource();
        newMusicSource.outputAudioMixerGroup = musicMixerGroup;
        newMusicSource.clip = newClip;
        newMusicSource.loop = true;
        newMusicSource.volume = 0.0f;
        newMusicSource.Play();

        _currentMusicSource = newMusicSource;

        yield return StartCoroutine(FadeInAudio(newMusicSource, fadeInDuration));
    }

    private static IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float currentTime = 0f;
        float startVolume = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0.0f, currentTime / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private static IEnumerator FadeInAudio(AudioSource audioSource, float duration, float volume = 1f)
    {
        float currentTime = 0f;
        audioSource.volume = 0.0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0.0f, volume, currentTime / duration);
            yield return null;
        }

        audioSource.volume = volume;
    }
    #endregion

}
