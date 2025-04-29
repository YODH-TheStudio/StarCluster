using System.Collections.Generic;
using UnityEngine;

public class CanvasInput : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject interactionButton;
    [SerializeField] private GameObject joystick;
    [SerializeField]private List<Vector3> gameobjectPosition = new List<Vector3>()
    {
        new Vector2(0, 0),
        new Vector2(250, -59),
        new Vector2(188, 0),
        new Vector2(-250, -59),
    };

    private Canvas _canvas;

    private HandDominanceManager _handDominanceManager;
    #endregion

    #region Main Functions
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        GameManager.Instance.GetPlayer().gameObject.GetComponent<PlayerInteractionZone>().SetInteractionButton(interactionButton);

        _handDominanceManager = GameManager.Instance.GetHandDominanceManager();

        _handDominanceManager.onUpdate += HandSwitch;
    }

    private void OnDestroy()
    {
        _handDominanceManager.onUpdate -= HandSwitch;
    }
    #endregion

    #region Hand Switch
    public void HandSwitch()
    {
        if (_handDominanceManager.GetHandDominance())
        {
            joystick.GetComponent<RectTransform>().anchoredPosition = gameobjectPosition[2];
            interactionButton.GetComponent<RectTransform>().anchoredPosition = gameobjectPosition[3];
        }
        else
        {
            joystick.GetComponent<RectTransform>().anchoredPosition = gameobjectPosition[0];
            interactionButton.GetComponent<RectTransform>().anchoredPosition = gameobjectPosition[1];  
        }
    }
    #endregion
}
