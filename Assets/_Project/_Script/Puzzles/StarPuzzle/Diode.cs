using UnityEngine;
using UnityEngine.UI;

public class Diode : MonoBehaviour
{
    private Image _image;
    [SerializeField] private float offAlpha;
    [SerializeField] private float onAlpha;
    private bool IsOn{ get; set;}
    
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        IsOn = false;
    }

    public void SetDiode(bool state)
    {
        Color newColor = _image.color;
        newColor.a = state ? onAlpha : offAlpha;
        _image.color = newColor;
    }
}
