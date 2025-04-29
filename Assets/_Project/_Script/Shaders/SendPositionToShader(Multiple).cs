using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SendMultiplePositionsToShader : MonoBehaviour
{
    #region Fields
    [SerializeField] private string shaderPropertyName = "CubesPositions";
    [SerializeField] private List<Transform> objectsToTrack;
    #endregion

    #region Main Functions
    private void Update()
    {
        // if (objectsToTrack.Count != Shader.GetGlobalVectorArray(shaderPropertyName).Length)
        // {
        //     Debug.LogError("The number of objects to track is different than the shader array size:" + objectsToTrack.Count + " object sent, expected " + Shader.GetGlobalVectorArray(shaderPropertyName).Length);
        //     return;
        // }
        
        Vector4[] positions = new Vector4[objectsToTrack.Count];
        // get only transform position
        for (int i = 0; i < objectsToTrack.Count; i++)
        {
            positions[i] = objectsToTrack[i].position;
        }
        Shader.SetGlobalVectorArray(shaderPropertyName, positions);
    }
    #endregion
}
