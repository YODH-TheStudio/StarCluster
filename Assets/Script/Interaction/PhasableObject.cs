using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using static PlayerScript;

public class PhasableObject : Interactable
{
    // States
    public enum PhaseState { Solid, Phase }
    public PhaseState _currentPhase = PhaseState.Solid;

    // Animation
    public Transform _objectBasePosition;
    [SerializeField] private Vector3 _startOffset = Vector3.zero;
    [SerializeField] private Vector3 _endOffset = Vector3.zero;
    public float _phaseDuration = 1.0f;
    [SerializeField] private float _phaseRadius = 5.0f; // Restriction radius

    private Vector3 _currentStartPosition;
    private Vector3 _currentEndPosition;

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

        _currentStartPosition = _objectBasePosition.position + _startOffset;
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

        // Switch state
        _currentPhase = (_currentPhase == PhaseState.Solid) ? PhaseState.Phase : PhaseState.Solid;

        // Define current start and end positions
        _currentStartPosition = (_currentPhase == PhaseState.Solid) ? _objectBasePosition.position + _endOffset : _objectBasePosition.position + _startOffset;
        _currentEndPosition = (_currentPhase == PhaseState.Solid) ? _objectBasePosition.position + _startOffset : _objectBasePosition.position + _endOffset;

        float distance = Vector3.Distance(_userTransform.position, _currentStartPosition);

        // Ensure player is within the phase radius
        if (distance > _phaseRadius)
        {
            Debug.Log("Player is out of phase radius. Phase transition denied.");
            return;
        }

        // Prepare anim
        if (_objectCollider != null)
        {
            _objectCollider.enabled = false;
        }

        StartCoroutine(PhaseAnimation());
    }

    private IEnumerator PhaseAnimation()
    {
        PlayerScript playerScript = _userTransform.GetComponent<PlayerScript>();

        playerScript.MoveTo(_currentStartPosition, _phaseDuration);
        yield return new WaitForSeconds(_phaseDuration);


        playerScript.MoveTo(_currentEndPosition, _phaseDuration);
        yield return new WaitForSeconds(_phaseDuration);


        if (_objectCollider != null)
        {
            _objectCollider.enabled = true;
        }
    }

    private void Update()
    {
        if (_userTransform == null)
            return;

        _lineRenderer.SetPosition(0, _userTransform.position); 
        _lineRenderer.SetPosition(1, _currentStartPosition); 

    }

    private void OnDrawGizmos()
    {
        if (_objectBasePosition == null)
        {
            Debug.LogError("_objectBasePosition is null in OnDrawGizmos!");
            return;
        }

        Vector3 currentStartPosition = (_currentPhase == PhaseState.Solid) ? _objectBasePosition.position + _endOffset : _objectBasePosition.position + _startOffset;
        Vector3 currentEndPosition = (_currentPhase == PhaseState.Solid) ? _objectBasePosition.position + _startOffset : _objectBasePosition.position + _endOffset;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_objectBasePosition.position + _startOffset, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_objectBasePosition.position + _endOffset, 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_objectBasePosition.position + _startOffset, _objectBasePosition.position + _endOffset);
    }

}
