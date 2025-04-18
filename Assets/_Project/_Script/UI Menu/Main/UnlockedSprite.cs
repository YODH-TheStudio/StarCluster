using UnityEngine;
using UnityEngine.UI;

public class UnlockedSprite : MonoBehaviour
{
    private bool _isUnlocked;
    private Image _image;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _image.color = _isUnlocked ? new Color(70, 70, 70, 255) : new Color(255, 255, 255, 255);
    }

    public void SetIsUnlocked(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
    }
}
