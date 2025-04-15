using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class GameObjectMovements : MonoBehaviour
{
    [SerializeField]private float rotationSpeed;
    [SerializeField] private bool lockXRotation;
    [SerializeField] private bool lockYRotation;
    [SerializeField] private bool lockZRotation;
    private Vector3 _rotation;
    private GameObject _object;

    private void Awake()
    {
        _object = this.gameObject;
        _rotation = _object.transform.eulerAngles;
    }

    private void RotateCamera(Vector3 rotation)
    {
        Quaternion targetRotation = Quaternion.Euler(rotation);
        _object.transform.rotation = Quaternion.Lerp(_object.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Update()
    {
        if (!lockXRotation)
        {
            _rotation.x += rotationSpeed * Time.deltaTime;
        }
        if (!lockYRotation)
        {
            _rotation.y += rotationSpeed * Time.deltaTime;
        }
        if (!lockZRotation)
        {
            _rotation.z += rotationSpeed * Time.deltaTime;
        }

        RotateCamera(_rotation);
    }
}
