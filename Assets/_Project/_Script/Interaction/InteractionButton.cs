using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    #region Field

    private PlayerScript _playerScript;

    #endregion

    #region Main Functions

        private void Start()
        {
            _playerScript = GameManager.Instance.GetPlayer();
        }
        
        public void Interact()
        {
            _playerScript.OnInteract();
        }

    #endregion
}
