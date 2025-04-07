using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPoolManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXPrefab;
    [SerializeField] private int poolSize = 10;

    private List<AudioSource> _pool = new List<AudioSource>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = Instantiate(soundFXPrefab, transform);
            source.gameObject.SetActive(false);
            _pool.Add(source);
        }
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var source in _pool)
        {
            if (!source.gameObject.activeInHierarchy)
                return source;
        }

        Debug.LogWarning("[SoundFXPoolManager] Aucun AudioSource dispo dans le pool ! Son SFX ignoré.");
        //return null;
        AudioSource extra = Instantiate(soundFXPrefab, transform);
        extra.gameObject.SetActive(false);
        _pool.Add(extra);
        return extra;
    }

    public void PlayClip(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();

        //AudioSource source = GetAvailableSource();
        //if (source == null) return; // On arrête ici si aucun n’est dispo


        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        source.gameObject.SetActive(true);
        source.Play();
        StartCoroutine(DisableAfterPlay(source, clip.length));
    }

    private IEnumerator DisableAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
    }
}
