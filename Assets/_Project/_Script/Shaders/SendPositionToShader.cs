using UnityEngine;

[ExecuteAlways]
public class SendPositionToShader : MonoBehaviour
{
    #region Fields
    [SerializeField] private string shaderPropertyName = "PlayerPosition";

    #endregion

    #region Main Functions
    private void Update()
    {
        Shader.SetGlobalVector(shaderPropertyName, transform.position);
    }
    #endregion
}
