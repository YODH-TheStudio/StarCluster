using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhasableObject : Interactable
{
    private SoundSystem _soundSystem;
    
    // Animation
    [SerializeField] private Vector3 startOffset = Vector3.zero;
    [SerializeField] private Vector3 endOffset = Vector3.zero;
    private const float PhaseDuration = 1.0f;
    [SerializeField] private float phaseRadius = 5.0f; // Restriction radius

    // Track a list of positions (Start, End) to phase
    private readonly List<(Vector3 start, Vector3 end)> _phasePairs = new List<(Vector3, Vector3)>();

    // LineRenderer to draw the radius in the game
    private LineRenderer _lineRenderer;

    private Collider _objectCollider;

    private void Awake()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
            
        _objectCollider = GetComponent<Collider>();
        _lineRenderer = gameObject.AddComponent<LineRenderer>();

        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.positionCount = 2;

        // Add both start and end positions
        _phasePairs.Add((this.transform.position + startOffset, this.transform.position + endOffset));
        _phasePairs.Add((this.transform.position + endOffset, this.transform.position + startOffset));
    }

    public override void Interact()
    {
        base.Interact();
        TogglePhase();
    }

    private void TogglePhase()
    {
        if (UserTransform == null)
        {
            Debug.LogError("_userTransform is null!");
            return;
        }

        // Loop through a pair of phase (start, end)
        foreach (var pair in _phasePairs)
        {
            // Check if the player is near the pair
            float distance = Vector3.Distance(UserTransform.position, pair.start);

            if (distance <= phaseRadius)
            {
                _soundSystem.PlaySoundFXClipByKey("Phase Elctrophase", transform.position);

                // Prepare the animation
                if (_objectCollider != null)
                {
                    _objectCollider.enabled = false;
                }

                StartCoroutine(PhaseAnimation(pair));  // Send the pair to the animator
                return;  // End the methode on transition
            }
        }

        // If no pair phase is active (not transition possible)
        Debug.Log("Player is out of phase radius. Phase transition denied.");
    }

    private IEnumerator PhaseAnimation((Vector3 start, Vector3 end) phasePair)
    {
        PlayerScript playerScript = UserTransform.GetComponent<PlayerScript>();

        yield return StartCoroutine(playerScript.MoveTo(phasePair.start, PhaseDuration));

        // Move the player and wait until the end of the phase
        yield return StartCoroutine(playerScript.MoveTo(phasePair.end, PhaseDuration));

        // Activate the collider
        if (_objectCollider != null)
        {
            _objectCollider.enabled = true;
        }
    }

    private void Update()
    {
        if (UserTransform == null) return;

        // Draw the position of the player and the pair
        _lineRenderer.SetPosition(0, UserTransform.position);

        // Optional : Can draw the line between the player and the phase start
        if (_phasePairs.Count > 0)
        {
            _lineRenderer.SetPosition(1, _phasePairs[0].start);  // Draw first pair for the example
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the phase pairs in game
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
