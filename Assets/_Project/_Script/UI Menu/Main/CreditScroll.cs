using System.Collections;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    #region
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float waitTime;
    [SerializeField] private CanvasGroup logo;
    public GameObject mainMenu;
    public GameObject creditMenu;
    private bool _shouldScroll;

    private RectTransform _rectTransform;

    #endregion

    #region Main Functions
    private void Start()
    {
        _shouldScroll = false;
        StartCoroutine(WaitForScroll(waitTime));
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!_shouldScroll) return;
        
        _rectTransform.localPosition += new Vector3(0, Time.deltaTime * scrollSpeed, 0);

        if (_rectTransform.localPosition.y > 1200)
        {
            ExitCredit();
        }
    }

    #endregion

    #region Credits 
    public void ExitCredit()
    {
            creditMenu.SetActive(false);
            mainMenu.gameObject.SetActive(true);
            ResetPosition();
    }
    #endregion

    #region Position 
    private void ResetPosition()
    {
        _rectTransform.localPosition = new Vector3(0, -975, 0);
    }
    #endregion

    #region Coroutine
    private IEnumerator WaitForScroll(float seconds)
    {
        logo.alpha = 0;
        logo.gameObject.SetActive(true);

        float elapsed = 0;
        float fadeDuration = seconds / 2f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            logo.alpha = Mathf.Clamp01(fadeDuration / elapsed);
            yield return null;
        }
        
        yield return new WaitForSeconds(seconds);
        
        _shouldScroll = true;
    }
    #endregion
}
