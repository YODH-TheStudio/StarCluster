//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(LevelData))]
//public class Puzzle2DEditor : Editor
//{
//    private LevelData levelData;

//    private void OnEnable()
//    {
//        levelData = (LevelData)target; // Récupère le ScriptableObject attaché
//    }

//    public override void OnInspectorGUI()
//    {
//        // Affiche l'inspecteur de base
//        base.OnInspectorGUI();

//        // Ajout d'un bouton pour ajouter un nouveau point
//        if (GUILayout.Button("Ajouter un Point"))
//        {
//            Vector2 newPoint = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
//            levelData.points.Add(newPoint);
//            EditorUtility.SetDirty(levelData); // Marque l'objet comme modifié
//        }

//        // Affiche les points existants dans l'inspecteur
//        EditorGUILayout.LabelField("Points dans le Niveau:");
//        foreach (var point in levelData.points)
//        {
//            EditorGUILayout.Vector2Field("Point", point);
//        }

//        // Ajoute un bouton pour ajouter une connexion entre deux points
//        if (GUILayout.Button("Ajouter une Connexion"))
//        {
//            if (levelData.points.Count >= 2)
//            {
//                levelData.connections.Add((levelData.points[0], levelData.points[1])); // Connexion entre le premier et le deuxième point
//                EditorUtility.SetDirty(levelData);
//            }
//            else
//            {
//                Debug.LogWarning("Pas assez de points pour créer une connexion.");
//            }
//        }
//    }

//    private void OnSceneGUI()
//    {
//        // Affiche les points dans la scène
//        if (levelData != null)
//        {
//            Handles.color = Color.green;
//            foreach (var point in levelData.points)
//            {
//                Handles.DrawSolidDisc(point, Vector3.forward, 0.1f);
//            }

//            // Affiche les connexions sous forme de lignes
//            Handles.color = Color.red;
//            foreach (var connection in levelData.connections)
//            {
//                Handles.DrawLine(connection.Item1, connection.Item2);
//            }
//        }
//    }
//}
