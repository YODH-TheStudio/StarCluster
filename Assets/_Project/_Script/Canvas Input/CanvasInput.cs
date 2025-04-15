using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasInput : MonoBehaviour
{
    [SerializeField]
    private GameObject _interactionButton;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.GetPlayer().gameObject.GetComponent<PlayerInteractionZone>().SetInteractionButton(_interactionButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
