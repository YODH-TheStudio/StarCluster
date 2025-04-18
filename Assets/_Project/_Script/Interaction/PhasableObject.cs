 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhasableObject : Interactable
{
    // Animation
    public Transform _objectBasePosition;
    [SerializeField] private Vector3 _startOffset = Vector3.zero;
    [SerializeField] private Vector3 _endOffset = Vector3.zero;
    public float _phaseDuration = 1.0f;
    [SerializeField] private float _phaseRadius = 5.0f; // Restriction radius

    // Liste des paires de positions (Start, End) pour les phases
    private List<(Vector3 start, Vector3 end)> _phasePairs = new List<(Vector3, Vector3)>();

    // LineRenderer to draw the radius in the game
    private LineRenderer _lineRenderer;

    // Base component
    private Collider _objectCollider;

    private void Awake()
    {
        _objectCollider = GetComponent<Collider>();
        _lineRenderer = gameObject.AddComponent<LineRenderer>();

        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.positionCount = 2;

        // Ajouter les paires de positions de d�part et d'arriv�e
        _phasePairs.Add((_objectBasePosition.position + _startOffset, _objectBasePosition.position + _endOffset));
        _phasePairs.Add((_objectBasePosition.position + _endOffset, _objectBasePosition.position + _startOffset));
    }

    public override void Interact()
    {
        base.Interact();
        TogglePhase();
    }

    private void TogglePhase()
    {
        if (_userTransform == null)
        {
            Debug.LogError("_userTransform is null!");
            return;
        }

        // Parcours chaque paire de phase (start, end)
        foreach (var pair in _phasePairs)
        {
            // V�rifie si le joueur est proche du point de d�part de cette paire
            float distance = Vector3.Distance(_userTransform.position, pair.start);

            if (distance <= _phaseRadius)
            {
                GameManager.Instance._soundSystem.PlaySoundFXClipByKey("Phase Elctrophase", transform.position);

                // Pr�pare l'animation
                if (_objectCollider != null)
                {
                    _objectCollider.enabled = false;
                }

                StartCoroutine(PhaseAnimation(pair));  // Envoie la paire directement � l'animation
                return;  // Fin de la m�thode d�s qu'une transition est effectu�e
            }
        }

        // Si aucune paire de phase n'est activ�e (pas de transition possible)
        Debug.Log("Player is out of phase radius. Phase transition denied.");
    }

    private IEnumerator PhaseAnimation((Vector3 start, Vector3 end) phasePair)
    {
        PlayerScript playerScript = _userTransform.GetComponent<PlayerScript>();

        yield return StartCoroutine(playerScript.MoveTo(phasePair.start, _phaseDuration));

        // D�place le joueur � la fin de la phase et attend que �a soit fini
        yield return StartCoroutine(playerScript.MoveTo(phasePair.end, _phaseDuration));

        // R�active le collider
        if (_objectCollider != null)
        {
            _objectCollider.enabled = true;
        }
    }

    private void Update()
    {
        if (_userTransform == null)
            return;

        // Affiche la position du joueur et de la phase active
        _lineRenderer.SetPosition(0, _userTransform.position);

        // Optionnel : On peut aussi dessiner la ligne de phase active entre le joueur et le point de d�part de la phase
        if (_phasePairs.Count > 0)
        {
            _lineRenderer.SetPosition(1, _phasePairs[0].start);  // Affiche la premi�re paire pour l'exemple
        }
    }

    private void OnDrawGizmos()
    {
        if (_objectBasePosition == null)
        {
            Debug.LogError("_objectBasePosition is null in OnDrawGizmos!");
            return;
        }

        // Dessine les paires de phases dans l'�diteur
        foreach (var pair in _phasePairs)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pair.start, 0.5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pair.end, 0.5f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pair.start, pair.end);
        }
    }
}
