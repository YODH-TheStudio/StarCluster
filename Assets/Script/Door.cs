using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float OpenHeight = 3.0f; // Hauteur d'ouverture
    public float Speed = 2.0f; // Vitesse d'ouverture
    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isOpen = false;

    private void Start()
    {
        _closedPosition = transform.position;
        _openPosition = _closedPosition + new Vector3(0, OpenHeight, 0);
    }

    private void Update()
    {
        if (_isOpen)
            transform.position = Vector3.Lerp(transform.position, _openPosition, Time.deltaTime * Speed);
        else
            transform.position = Vector3.Lerp(transform.position, _closedPosition, Time.deltaTime * Speed);
    }

    public void OpenDoor()
    {
        _isOpen = true;
        Debug.Log("[Door] Porte ouverte !");
    }

    public void CloseDoor()
    {
        _isOpen = false;
        Debug.Log("[Door] Porte fermée !");
    }
}
