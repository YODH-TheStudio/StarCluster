using UnityEngine;

[ExecuteAlways]
public class ShaderGlobalsDebugger : MonoBehaviour
{
    Vector3 playerPosition;
    Vector3 terrainPosition;
    void OnGUI()
    {
        GUILayout.Label("Shader Global Properties:");

        // Affiche la position du joueur
        playerPosition = Shader.GetGlobalVector("PlayerPosition");
        GUILayout.Label($"PlayerPosition: {playerPosition}");

        // Affiche la position du terrain
        terrainPosition = Shader.GetGlobalVector("TerrainPosition");
        GUILayout.Label($"TerrainPosition: {terrainPosition}");
        
        Vector3 dist = playerPosition - terrainPosition;
        GUILayout.Label($"Distance: {dist}");
    }

    void OnDrawGizmos()
    {
        float grassDisplacementFalloff = 0.7f;
        // float influence = saturate(1.0 - dist * grassDisplacementFalloff);
        
        //draw gizmo arround player
        Gizmos.DrawSphere(playerPosition, grassDisplacementFalloff);
    }
}