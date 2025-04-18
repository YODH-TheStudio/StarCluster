using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    #region Fields
    private PlayerScript _playerScript;

    #endregion

    #region Main Functions
    // Start is called before the first frame update
    private void Start()
    {
        _playerScript = GetComponentInParent<PlayerScript>();
    }

    // Update is called once per frame
    public void FootStep()
    {
        if (_playerScript != null)
        {
            _playerScript.PlayFootstepSound();
        }
    }

    #endregion
}
