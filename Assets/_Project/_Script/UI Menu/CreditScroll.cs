using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float waitTime;
    [SerializeField] private CanvasGroup logo;
    public GameObject mainMenu;
    public GameObject creditMenu;
    private bool _shouldScroll;

    private RectTransform _rectTransform;

    void Start()
    {
        _shouldScroll = false;
        StartCoroutine(WaitForScroll(waitTime));
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_shouldScroll) return;
        
        _rectTransform.localPosition += new Vector3(0, Time.deltaTime * scrollSpeed, 0);

        ExitCredit();
    }

    private void ExitCredit()
    {
        if (_rectTransform.localPosition.y > 1500)
        {
            creditMenu.SetActive(false);
            mainMenu.gameObject.SetActive(true);
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        _rectTransform.localPosition = new Vector3(0, -975, 0);
    }

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
}
