using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System.Reflection;
using UnityEngine.Rendering.Universal;

public class CustomURPEditorResourcesLoader : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ReplaceURPEditorResources()
    {
        UniversalRenderPipelineAsset urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
        if (urpAsset == null)
        {
            Debug.LogError("Not using URP.");
            return;
        }

        // Load your custom URP Editor Resources
        string customAssetPath = "Assets/Rendering/Custom-URP-EditorResources.asset";
        var customEditorResources = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineEditorResources>(customAssetPath);

        if (customEditorResources == null)
        {
            Debug.LogError("Custom URP Editor Resources asset not found.");
            return;
        }

        // Use reflection to set the internal field
        FieldInfo editorResourcesField = typeof(UniversalRenderPipelineAsset).GetField("m_EditorResourcesAsset", BindingFlags.NonPublic | BindingFlags.Instance);
        if (editorResourcesField != null)
        {
            editorResourcesField.SetValue(urpAsset, customEditorResources);
            Debug.Log("URP Editor Resources replaced successfully.");
        }
        else
        {
            Debug.LogError("Unable to find the internal field in URP Asset.");
        }
    }
}