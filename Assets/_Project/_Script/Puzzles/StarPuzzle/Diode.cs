using UnityEngine;
using UnityEngine.UI;

public class Diode : MonoBehaviour
{
    private Image _image;
    [Range(0,1)][SerializeField] private float offAlpha;
    [Range(0,1)][SerializeField] private float onAlpha;
    private bool IsOn{ get; set;}
    private SoundSystem _soundSystem;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        _soundSystem = GameManager.Instance.GetSoundSystem();
    }

    private void Start()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
        IsOn = false;
    }

    public void SetDiode(bool state)
    {
        Color newColor = _image.color;
        
        if (!IsOn && state)
        {
            _soundSystem.PlaySoundFXClipByKey("Chimes Chime A", transform.position);
            IsOn = true;
        }
        else if (!state)
        {
            IsOn = false;
        }
        
        newColor.a = state ? onAlpha : offAlpha;
        _image.color = newColor;
    }
}
