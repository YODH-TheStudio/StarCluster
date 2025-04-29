using UnityEngine;
using UnityEngine.UI;

#region Enums
public enum DiodeColor
{
    Blue,
    Green,
    Brown,
    Red,
    Purple,
}
#endregion

public class Diode : MonoBehaviour
{
    #region Fields

    private SoundSystem _soundSystem;
    private Image _image;
    private bool _isOn;
    [SerializeField] private Sprite off;
    [SerializeField] private Sprite on;
    #endregion

    #region Main Functions
    private void Awake()
    {
        _image = GetComponent<Image>();
        _soundSystem = GameManager.Instance.GetSoundSystem();
    }

    private void Start()
    {
        _image.sprite = off;
        _isOn = false;
    }
    #endregion

    #region Diode status
    public void SetDiode(bool state)
    {
        if (!_isOn && state)
        {
            _soundSystem.PlaySoundFXClipByKey("Chimes Chime A", transform.position);

        }

        _isOn = state;
        _image.sprite = state ? on : off;
    }

    public bool GetIsOn()
    {
        return _isOn;
    }
    #endregion
}
