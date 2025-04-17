using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class SendPositionToShader : MonoBehaviour
{
    [SerializeField] private string shaderPropertyName = "_PlayerPosition";
    
    void Update()
    {
        Shader.SetGlobalVector(shaderPropertyName, transform.position);
    }
}
