using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    #region Fields
    private bool _canVibrate;

    #endregion

    #region Main Functions
    private void Start()
    {
        LoadPlayerPrefs();
    }
    #endregion

    #region Vibration
    public void Vibrate(float strength, float duration)
    {
        if (!_canVibrate || Application.platform != RuntimePlatform.Android)
            return;

        long milliseconds = (long)(duration * 1000);
        int amplitude = Mathf.Clamp((int)(strength * 255), 1, 255); 

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
        using (AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
            if (vibrator == null) return;

            if (sdkInt >= 26)
            {
                using (AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect"))
                {
                    bool hasAmplitudeControl = vibrator.Call<bool>("hasAmplitudeControl");
                    if (hasAmplitudeControl)
                    {
                        AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                        vibrator.Call("vibrate", effect);
                    }
                    else
                    {
                        vibrator.Call("vibrate", milliseconds);
                    }
                }
            }
            else
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
    }

    private void LoadPlayerPrefs()
    {
        _canVibrate = PlayerPrefs.GetInt("CanVibrate", 1) == 1;
    }
    public void SwitchVibrationMode(bool vibrate)
    {
        PlayerPrefs.SetInt("CanVibrate", vibrate ? 1 : 0);
        PlayerPrefs.Save();
        _canVibrate = PlayerPrefs.GetInt("CanVibrate", 1) == 1;
    }

    public bool GetVibrationMode()
    {
        return _canVibrate;
    }
    #endregion
}
