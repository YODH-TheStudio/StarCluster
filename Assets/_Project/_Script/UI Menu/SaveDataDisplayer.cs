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
    
    private LocalizeStringEvent localizeStringEvent;

    private void Awake()
    {
        // Setup planet name localisation
        //localizeStringEvent = GetComponent<LocalizeStringEvent>();
        localizeStringEvent = _planetNameTxt.GetComponent<LocalizeStringEvent>();
        if (localizeStringEvent == null)
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
        //Debug.LogAssertion("Setting planet name to " + planetNameKey);
        localizeStringEvent.StringReference.TableReference = "Planet Names";
        localizeStringEvent.StringReference.TableEntryReference = planetNameKey;
        localizeStringEvent.RefreshString(); // Triggers the UI update
        
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
