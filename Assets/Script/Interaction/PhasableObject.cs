using UnityEngine;
using System.Collections;

public class PhasableObject : Interactable
{
    // States
    public enum PhaseState { Solid, Phase }
    public PhaseState currentPhase = PhaseState.Solid;

    // Animation
    public Transform _objectBasePosition;
    [SerializeField] private Vector3 startOffset = Vector3.zero;     // Offset pour la position de départ
    [SerializeField] private Vector3 endOffset = Vector3.zero;       // Offset pour la position d'arrivée
    public float phaseDuration = 1.0f;

    // Base component
    private Collider objectCollider;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
        Debug.Log("Awake called. Collider found: " + (objectCollider != null));
    }

    public override void Interact()
    {
        Debug.Log("Interact called.");
        base.Interact();
        TogglePhase();
    }

    // Toggle phase in enter phase/exit phase
    private void TogglePhase()
    {
        Debug.Log("TogglePhase called.");

        if (_userTransform == null)
        {
            Debug.LogError("_userTransform is null!");
            return;
        }

        Debug.Log("Current phase: " + currentPhase);

        // Switch state 
        currentPhase = (currentPhase == PhaseState.Solid) ? PhaseState.Phase : PhaseState.Solid;
        Debug.Log("New phase: " + currentPhase);

        // Switch enter/exit start/end position
        Vector3 start = (currentPhase == PhaseState.Solid) ? _objectBasePosition.position + endOffset : _objectBasePosition.position + startOffset;
        Vector3 end = (currentPhase == PhaseState.Solid) ? _objectBasePosition.position + startOffset : _objectBasePosition.position + endOffset;

        Debug.Log("Start position: " + start);
        Debug.Log("End position: " + end);

        // Disable collision during phase only
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
            Debug.Log("Collider disabled.");
        }
        else
        {
            Debug.LogError("Collider is null!");
        }

        StartCoroutine(PhaseAnimation(start, end));
    }

    private IEnumerator PhaseAnimation(Vector3 start, Vector3 end)
    {
        Debug.Log("PhaseAnimation started.");
        Debug.Log("Start position: " + start + ", End position: " + end);

        float elapsed = 0f;

        while (elapsed < phaseDuration)
        {
            _userTransform.position = Vector3.Lerp(start, end, elapsed / phaseDuration);
            //Debug.Log("Lerp progress: " + (elapsed / phaseDuration) + " | Current position: " + _userTransform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure that the user is correctly positioned at the end
        //_userTransform.position = end;
        Debug.Log("Final position: " + _userTransform.position);

        // Reactivate the collider
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
            Debug.Log("Collider re-enabled.");
        }

        Debug.Log("Phase transition complete.");

    }

    private void OnDrawGizmos()
    {
        if (_objectBasePosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_objectBasePosition.position + startOffset, 0.5f);
            Gizmos.DrawSphere(_objectBasePosition.position + endOffset, 0.5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_objectBasePosition.position + startOffset, _objectBasePosition.position + endOffset);
            Debug.Log("Gizmos drawn: Start and End positions with offsets.");
        }
        else
        {
            Debug.LogError("_objectBasePosition is null in OnDrawGizmos!");
        }
    }
}
