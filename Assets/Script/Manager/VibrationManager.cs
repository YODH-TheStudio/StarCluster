using System.Collections;
using TMPro;
using UnityEngine;


public class VibrationManager : MonoBehaviour
{
    enum VibrationType
    {
        Light,
        Medium,
        Heavy
    }

    VibrationType vibrationType = VibrationType.Light;

    // Start is called before the first frame update
    void Start()
    {
        Handheld.Vibrate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVibrationType(int type)
    {
        vibrationType = (VibrationType)type;

        if (vibrationType == VibrationType.Light)
        {
            Debug.Log("Light");
            AndroidVibrate(1f, 0.1f);
        }
        else if (vibrationType == VibrationType.Medium)
        {
            Debug.Log("Medium");
            AndroidVibrate(100f, 0.25f);
        }
        else if (vibrationType == VibrationType.Heavy)
        {
            Debug.Log("Heavy");
            AndroidVibrate(255f, 0.5f);
        }
    }

    private void AndroidVibrate(float strength, float duration)
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            long milliseconds = (long)(duration * 1000);
            int amplitude = Mathf.Clamp((int)(strength * 255), 0, 255);

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
            {
                if (vibrator != null)
                {
                    using (AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect"))
                    {
                        AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                        vibrator.Call("vibrate", effect);
                    }
                }
            }
        }
    }
}
