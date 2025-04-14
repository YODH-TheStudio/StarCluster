using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmeSocle : MonoBehaviour
{
    public List<ObjetSoclePair> objetsSocles = new List<ObjetSoclePair>();
    private Dictionary<GameObject, bool> objetsPlacementStatus = new Dictionary<GameObject, bool>();

    public float validationRadius = 1.5f;

    public bool IsSolved { get; private set; }

    [System.Serializable]
    public class ObjetSoclePair
    {
        public GameObject objet; 
        public Vector3 offset;  
        public float radius;   
    }

    private void Start()
    {
        foreach (var pair in objetsSocles)
        {
            objetsPlacementStatus[pair.objet] = false; 
        }
    }

    private void Update()
    {

        foreach (var pair in objetsSocles)
        {
            Vector3 socleGlobalPosition = transform.position + pair.offset;

            if (pair.objet != null)
            {

                float distanceXZ = Vector3.Distance(new Vector3(pair.objet.transform.position.x, 0, pair.objet.transform.position.z), new Vector3(socleGlobalPosition.x, 0, socleGlobalPosition.z));

                if (distanceXZ <= pair.radius)
                {
                    if (!objetsPlacementStatus[pair.objet]) // Si l'objet n'était pas déjà validé
                    {
                        objetsPlacementStatus[pair.objet] = true;
                        Debug.Log($"Objet {pair.objet.name} placé correctement sur le socle.");
                    }
                    else
                    {
                        //Debug.Log($"Objet {pair.objet.name} déjà placé correctement.");
                    }
                }
                else
                {
                    if (objetsPlacementStatus[pair.objet]) 
                    {
                        objetsPlacementStatus[pair.objet] = false;
                        //Debug.Log($"Objet {pair.objet.name} n'est plus sur le socle.");
                    }
                    else
                    {
                        //Debug.Log($"Objet {pair.objet.name} est hors du rayon, mais n'était pas encore validé.");
                    }
                }
            }
            else
            {
                //Debug.LogWarning($"Objet non assigné pour le socle à {socleGlobalPosition}.");
            }
        }

        CheckEnigmeResolution();
    }

    private void CheckEnigmeResolution()
    {
        IsSolved = true;
        foreach (var item in objetsPlacementStatus)
        {
            if (!item.Value)
            {
                IsSolved = false;
                break;
            }
        }

        if (IsSolved)
        {
            OnEnigmeSolved();
        }
    }

    protected virtual void OnEnigmeSolved()
    {
        //Debug.Log("L'énigme est résolue!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 

        foreach (var pair in objetsSocles)
        {
            Vector3 socleGlobalPosition = transform.position + pair.offset;

            if (pair.objet != null)
            {
                DrawFilledCircle(socleGlobalPosition, pair.radius);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, socleGlobalPosition);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(socleGlobalPosition, 0.1f);
            }
        }
    }

    private void DrawFilledCircle(Vector3 position, float radius)
    {
        int segments = 50; 
        float angleStep = 360f / segments;

        Gizmos.color = Color.red;

        for (int i = 0; i < segments; i++)
        {
            float angle = angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            Vector3 pointOnCircle = position + new Vector3(radius * Mathf.Cos(radian), 0, radius * Mathf.Sin(radian));
            Gizmos.DrawLine(position, pointOnCircle); 
        }
    }
}
