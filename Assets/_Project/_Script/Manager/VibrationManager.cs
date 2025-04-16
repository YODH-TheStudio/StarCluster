using System.Collections;
using TMPro;
using UnityEngine;


public class VibrationManager : MonoBehaviour
{
    private bool _canVibrate;
    
    private void Start()
    {
        LoadPlayerPrefs();
    }
    
    public void Vibrate(float strength, float duration)
    {
        if (!_canVibrate) return;
        
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

    private void LoadPlayerPrefs()
    {
        
    }
    public void SwitchVibrationMode()
    {
        _canVibrate = !_canVibrate;
    }

    public bool GetVibrationMode()
    {
        return _canVibrate;
    }
}
