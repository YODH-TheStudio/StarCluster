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
        Vector3 playerPos = new Vector4(transform.position.x, transform.position.y, transform.position.z);
        Shader.SetGlobalVector("_PlayerPosition", playerPos);
        //Shader.SetGlobalVector(shaderPropertyName, transform.position);
    }
}
