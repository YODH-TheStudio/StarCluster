using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PhasableObject : Interactable
{
    // States
    public enum PhaseState { Solid, Phase }
    public PhaseState currentPhase = PhaseState.Solid;

    // Animation
    public Transform objectBasePosition;
    [SerializeField] private Vector3 _startOffset = Vector3.zero;     // Offset pour la position de départ
    [SerializeField] private Vector3 _endOffset = Vector3.zero;       // Offset pour la position d'arrivée
    public float phaseDuration = 1.0f;

    // Base component
    private Collider _objectCollider;

    private void Awake()
    {
        _objectCollider = GetComponent<Collider>();
        Debug.Log("Awake called. Collider found: " + (_objectCollider != null));
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
        Vector3 start = (currentPhase == PhaseState.Solid) ? objectBasePosition.position + _endOffset : objectBasePosition.position + _startOffset;
        Vector3 end = (currentPhase == PhaseState.Solid) ? objectBasePosition.position + _startOffset : objectBasePosition.position + _endOffset;

        Debug.Log("Start position: " + start);
        Debug.Log("End position: " + end);

        // Disable collision during phase only
        if (_objectCollider != null)
        {
            _objectCollider.enabled = false;
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

 

        PlayerScript playerScript = _userTransform.GetComponent<PlayerScript>();
        playerScript.SetIsAnimating(true); 

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
        if (_objectCollider != null)
        {
            _objectCollider.enabled = true;
            Debug.Log("Collider re-enabled.");
        }

        playerScript.SetIsAnimating(false);

        Debug.Log("Phase transition complete.");

    }

    private void OnDrawGizmos()
    {
        if (objectBasePosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(objectBasePosition.position + _startOffset, 0.5f);
            Gizmos.DrawSphere(objectBasePosition.position + _endOffset, 0.5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(objectBasePosition.position + _startOffset, objectBasePosition.position + _endOffset);
            Debug.Log("Gizmos drawn: Start and End positions with offsets.");
        }
        else
        {
            Debug.LogError("_objectBasePosition is null in OnDrawGizmos!");
        }
    }
}
