using UnityEngine;

[ExecuteAlways]
public class SendPositionToShader : MonoBehaviour
{
    [SerializeField] private string shaderPropertyName = "_PlayerPosition";
    
    private void Update()
    {
        Shader.SetGlobalVector(shaderPropertyName, transform.position);
    }
}
