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
        if (!_canVibrate) return;

        if (Application.platform != RuntimePlatform.Android) return;
        
        long milliseconds = (long)(duration * 1000);
        int amplitude = Mathf.Clamp((int)(strength * 255), 0, 255);

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
        {
            if (vibrator == null) return;
            
            using (AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect"))
            {
                AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                vibrator.Call("vibrate", effect);
            }
        }
    }

    private void LoadPlayerPrefs()
    {
        _canVibrate = PlayerPrefs.GetInt("CanVibrate", 1) == 1;
    }
    public void SwitchVibrationMode()
    {
        _canVibrate = !_canVibrate;
        int vibrationBool = _canVibrate ? 1 : 0;
        PlayerPrefs.SetInt("CanVibrate", vibrationBool);
        PlayerPrefs.Save();
    }

    public bool GetVibrationMode()
    {
        return _canVibrate;
    }
    #endregion
}
