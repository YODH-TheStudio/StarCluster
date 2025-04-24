using UnityEngine;
using UnityEngine.UI;

public enum DiodeColor
{
    Blue,
    Green,
    Brown,
    Red,
    Purple,
}

public class Diode : MonoBehaviour
{
    private Image _image;
    private bool _isOn;
    private DiodeColor DiodeColor { get; set; }
    [SerializeField] private Sprite off;
    [SerializeField] private Sprite on;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _image.sprite = off;
        _isOn = false;
    }

    private void SetDiode(bool state)
    {
        _image.sprite = state ? on : off;
    }

    public bool GetIsOn()
    {
        return _isOn;
    }
}
