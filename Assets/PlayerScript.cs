using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, Controler.IPlayerActions
{
    private CharacterController _controller;
    private Vector3 _direction;
    private float _speed = 5.0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Controler playerControls = new Controler();
        playerControls.Player.SetCallbacks(this);
    }

    private Vector3 IsoVectorConvert(Vector3 vector)
    {
        Quaternion rotation = Quaternion.Euler(0, 45.0f, 0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        Vector3 result = isoMatrix.MultiplyPoint3x4(vector);
        return result;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 readVector = context.ReadValue<Vector2>();
        Vector3 toConvert = new Vector3(readVector.x, 0, readVector.y);
        _direction = IsoVectorConvert(toConvert);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.Log("Can Interact");
            hit.transform.position = transform.position;
        }
        else
        {
            Debug.Log("Can't Interact");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _controller.Move(_direction * _speed * Time.deltaTime);
    }
}
