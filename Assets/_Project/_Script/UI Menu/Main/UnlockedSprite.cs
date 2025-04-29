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
        _image.color = _isUnlocked ? new Color(1f, 1f, 1f, 1f) : new Color(0.3f, 0.3f, 0.3f, 1f);
    }
    #endregion

    #region Unlocked
    public void SetIsUnlocked(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
    }
    #endregion
}
