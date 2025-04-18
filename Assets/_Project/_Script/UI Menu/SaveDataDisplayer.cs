using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI planetNameText;
    [SerializeField] private TextMeshProUGUI saveDateText;
    [SerializeField] private RawImage planetIconImage;
    
    public void Set(string planetName, string saveDate, Texture2D planetIcon)
    {
        planetNameText.text = planetName;
        saveDateText.text = saveDate;
        if(planetIcon == null)
        {
            planetIconImage.gameObject.SetActive(false);
        }
        else
        {
            planetIconImage.gameObject.SetActive(true);
            planetIconImage.texture = planetIcon;
        }
    }
}
