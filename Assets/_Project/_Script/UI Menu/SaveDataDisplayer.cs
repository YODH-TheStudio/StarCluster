using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _planetName;
    [SerializeField] private TextMeshProUGUI _saveDate;
    [SerializeField] private RawImage _planetIcon;
    
    public void Set(string planetName, string saveDate, Texture2D planetIcon)
    {
        _planetName.text = planetName;
        _saveDate.text = saveDate;
        _planetIcon.texture = planetIcon;
    }
}
