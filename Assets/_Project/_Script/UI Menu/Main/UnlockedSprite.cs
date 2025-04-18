using UnityEngine;
using UnityEngine.UI;

public class UnlockedSprite : MonoBehaviour
{
    #region Fields
    private bool _isUnlocked;
    private Image _image;
    #endregion

    #region Main Functions
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _image.color = _isUnlocked ? new Color(70, 70, 70, 255) : new Color(255, 255, 255, 255);
    }
    #endregion

    #region Unlocked
    public void SetIsUnlocked(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
    }
    #endregion
}
