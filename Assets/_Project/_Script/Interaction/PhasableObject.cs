using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhasableObject : Interactable
{
    #region Fields
    private SoundSystem _soundSystem;
    
    // Animation
    [SerializeField] private GameObject _startPosition;
    [SerializeField] private GameObject _endPosition;
    [SerializeField] private float PhaseDuration = 2.0f;
    [SerializeField] private float phaseRadius = 2.0f; // Restriction radius

    // Track a list of positions (Start, End) to phase
    private readonly List<(Vector3 start, Vector3 end)> _phasePairs = new List<(Vector3, Vector3)>();

    // LineRenderer to draw the radius in the game
    private LineRenderer _lineRenderer;

    private Collider _objectCollider;
    
    private PlayerScript _playerScript;
    
    private static readonly int Moving = Animator.StringToHash("IsMoving");
    private static readonly int Phasing = Animator.StringToHash("IsPhasing");

    #endregion

    #region Main Functions
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
        _phasePairs.Add((_startPosition.transform.position, _endPosition.transform.position));
        _phasePairs.Add((_endPosition.transform.position, _startPosition.transform.position));
    }

    private void Start()
    {
        _playerScript = GameManager.Instance.GetPlayer();
    }

    #endregion

    #region Interaction
    public override void Interact()
    {
        base.Interact();
        TogglePhase();
    }
    
    #endregion

    #region Phase

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
    }

    #endregion

    #region Draw Debug 
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
    #endregion

    #region Coroutine
    private IEnumerator PhaseAnimation((Vector3 start, Vector3 end) phasePair)
    {
        float dist = Vector3.Distance(_playerScript.transform.position, phasePair.start);
        
        GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Phasing);
        
        _playerScript.GetAnimator().SetBool(Moving, true);
        yield return StartCoroutine(_playerScript.MoveTo(phasePair.start, dist / 2f));

        
        _playerScript.GetAnimator().SetBool(Moving, false);
        _playerScript.GetAnimator().SetBool(Phasing, true);
        
        _soundSystem.PlaySoundFXClipByKey("Phase Electrophase", transform.position);
        
        // Move the player and wait until the end of the phase
        yield return StartCoroutine(_playerScript.MoveTo(phasePair.end, PhaseDuration));
        _playerScript.GetAnimator().SetBool(Phasing, false);

        GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Idle);
        
        // Activate the collider
        if (_objectCollider != null)
        {
            _objectCollider.enabled = true;
        }
    }
    #endregion
}
