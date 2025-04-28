using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class SaveDataDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _planetNameTxt;
    [SerializeField] private TextMeshProUGUI _saveDateTxt;
    [SerializeField] private RawImage _planetIcon;
    
    private LocalizeStringEvent _localizeStringEvent;

    private void Awake()
    {
        // Setup planet name localisation
        _localizeStringEvent = _planetNameTxt.GetComponent<LocalizeStringEvent>();
        if (_localizeStringEvent == null)
        {
            Debug.LogError("LocalizeStringEvent component is missing on " + gameObject.name);
        }
        if(_planetIcon.texture == null)
        {
            _planetIcon.gameObject.SetActive(false);
        }
    }

    public void Set(string planetNameKey, string saveDate, Texture2D planetIcon)
    {
        //_planetNameTxt.text = planetNameKey;
        _localizeStringEvent.StringReference.TableReference = "Planet Names";
        _localizeStringEvent.StringReference.TableEntryReference = planetNameKey;
        _localizeStringEvent.RefreshString(); // Triggers the UI update
        
        _saveDateTxt.text = saveDate;
        if(planetIcon == null)
        {
            _planetIcon.gameObject.SetActive(false);
        }
        else
        {
            _planetIcon.gameObject.SetActive(true);
            _planetIcon.texture = planetIcon;
        }
    }
}