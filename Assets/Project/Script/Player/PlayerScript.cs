using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static UnityEngine.ParticleSystem;

public class PlayerScript : MonoBehaviour, Controler.IPlayerActions
{
    // Player Controler Variable 
    [SerializeField]
    private float _speed = 250.0f;

    [SerializeField]
    private float _raycastDistance = 1.25f;

    [SerializeField]
    private float _turnSpeed = 360.0f;

    [SerializeField]
    private Vector3 _particleStartPos;

    private GameObject _particle;

    // debug pour le temps que on a pas de detecteur de sol
    [SerializeField]
    private bool _isOnGrass;

    private CharacterController _controller;
    private Vector3 _direction;

    private bool _isGrabbing = false;
    private GameObject _objectGrabbed;

    // Set the variable
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Controler playerControls = new Controler();
        playerControls.Player.SetCallbacks(this);
    }

    // Allow the player to move, Is called by the character controler component in player
    public void OnMove(Vector2 readVector)
    {
        // Calculated the movement of the player with the 45� change due to isometric view
        Vector3 position = new Vector3(readVector.x, 0, readVector.y);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45.0f, 0));

        _direction = isoMatrix.MultiplyPoint3x4(position);
    }

    // Allow the player to interact with objetc, Is called by the character controler component in player
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            if (IsGrabbing())
            {
                SetGrabbing(false);
                _objectGrabbed.GetComponent<BoxCollider>().enabled = true;
                _objectGrabbed.GetComponent<Rigidbody>().useGravity = true;

                return;
            }

            else
            {
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))
                {
                    GameManager.Instance._soundSystem.PlaySoundFXClipByKey("Violon");
                    GameManager.Instance._soundSystem.ChangeMusicByKey("Doce"); 
                    //SoundManager.PlaySound(SoundType.None,1);
                }

                return;
            }
        }
    }

    // Rotate the player in the direction he is walking
    private void Look()
    {
        // Don't go back to the starting rotation when the player don't move
        if (_direction != Vector3.zero)
        {
            // Calculated the rotation and set it
            var relative = (transform.position + _direction) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
        }  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _controller.SimpleMove(_direction * _speed * Time.deltaTime);
        Look();

        if(_particle != null)
        {
            if (_direction != Vector3.zero)
            {
                _particle.GetComponent<ParticleSystem>().enableEmission = true;
            }
            else if (_direction == Vector3.zero)
            {
                _particle.GetComponent<ParticleSystem>().enableEmission = false;
            }
        }

        if (IsGrabbing())
        {
            _objectGrabbed.transform.position = transform.position + (Vector3.forward * 2);
        }

    }

    // Code to interact with object
    public bool IsGrabbing()
    {
        return _isGrabbing;
    }

    public void SetGrabbing(bool isGrabbing)
    {
        _isGrabbing = isGrabbing;
    }

    public GameObject GetObjectGrabbed()
    {
        return _objectGrabbed;
    }

    public void SetObjectGrabbed(GameObject objectGrabbed)
    {
        _objectGrabbed = objectGrabbed;
    }

    public bool IsMoving()
    {
        return _direction != Vector3.zero;
    }

    public void SpawnParticle(GameObject particle)
    {
        if (_particle != null)
        {
            _particle.GetComponent<ParticleSystem>().enableEmission = false;
        }
        Destroy(_particle, 2);

        _particle = Instantiate(particle, Vector3.zero, Quaternion.AngleAxis(180, new Vector3(0, 1, 0)));
        _particle.GetComponent<ParticleSystem>().enableEmission = true;
        _particle.transform.SetParent(gameObject.transform);
        _particle.transform.localPosition = _particleStartPos;
    }
}
