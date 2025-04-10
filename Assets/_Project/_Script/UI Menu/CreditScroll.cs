using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float waitTime;
    public GameObject MainMenu;
    public GameObject CreditMenu;

    private RectTransform _rectTransform;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        ResetPosition();
        StartCoroutine(Wait(waitTime));
    }

    // Update is called once per frame
    void Update()
    {

        _rectTransform.localPosition += new Vector3(0, Time.deltaTime * scrollSpeed, 0);
        Debug.Log(_rectTransform.localPosition);

        if (_rectTransform.localPosition.y > 1500)
        {
            ResetPosition();
            Debug.Log("reset");
            CreditMenu.SetActive(false);
            MainMenu.gameObject.SetActive(true);
        }
    }

    private void ResetPosition()
    {
        _rectTransform.localPosition.Set(0, -975, 0);
    }

    private IEnumerator Wait(float seconds)
    {
        Debug.Log(seconds);
        yield return new WaitForSeconds(seconds);
    }
}
