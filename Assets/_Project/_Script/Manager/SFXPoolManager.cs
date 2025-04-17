using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXPoolManager : MonoBehaviour
{
    #region Fields
    public GameObject sfxPrefab;
    public int poolSize = 10;

    private Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private AudioMixerGroup sfxMixerGroup;

    #endregion

    #region Main Function 
    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(sfxPrefab, transform);
            obj.SetActive(false);
            AudioSource source = obj.GetComponent<AudioSource>();
            if (sfxMixerGroup != null)
            {
                source.outputAudioMixerGroup = sfxMixerGroup;
            }
            availableSources.Enqueue(source);
        }
    }
    #endregion

    #region SFX
    public void SetMixerGroup(AudioMixerGroup mixerGroup)
    {
        sfxMixerGroup = mixerGroup;

        // On applique le groupe aux sources déjà créées
        foreach (var source in availableSources)
        {
            source.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    public AudioSource GetAvailableSource()
    {
        if (availableSources.Count > 0)
        {
            return availableSources.Dequeue();
        }
        else
        {
            Debug.LogWarning("SFXPoolManager: Pas de sources disponibles !");
            return null;
        }
    }


    public void PlayClip(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }
        Debug.Log("Clip à jouer : " + clip.name);

        AudioSource source = GetAvailableSource();
        if (source == null) return; // Sécurité au cas où la pool est vide

        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        source.gameObject.SetActive(true);
        source.Play();
        StartCoroutine(DisableAfterPlay(source, clip.length));
    }
    #endregion

    #region Coroutines 
    private IEnumerator DisableAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        availableSources.Enqueue(source); // IMPORTANT : On remet la source dans la pool
    }

    #endregion
}
