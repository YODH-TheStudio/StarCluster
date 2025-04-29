using System.Collections.Generic;
using UnityEngine;

public class CanvasInput : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject interactionButton;
    [SerializeField] private GameObject joystick;
    [SerializeField]private List<Vector3> gameobjectPosition = new List<Vector3>()
    {
        new Vector3(0, 0, 0),
        new Vector3(333, -59, 0),
        new Vector3(188, 0, 0),
        new Vector3(-333, -59, 0),
    };
    #endregion

    #region Main Functions
    private void Start()
    {
        GameManager.Instance.GetPlayer().gameObject.GetComponent<PlayerInteractionZone>().SetInteractionButton(interactionButton);
    }
    #endregion

    #region Hand Switch
    public void HandSwitch(bool IsLeftHanded)
    {
        if (IsLeftHanded)
        {
            joystick.transform.localPosition = gameobjectPosition[0];
            interactionButton.transform.localPosition = gameobjectPosition[1];
        }
        else
        {
            joystick.transform.localPosition = gameobjectPosition[2];
            interactionButton.transform.localPosition = gameobjectPosition[3];  
        }
    }
    #endregion
}
