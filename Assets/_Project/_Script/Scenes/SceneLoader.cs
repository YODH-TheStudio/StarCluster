using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Image loadingBar;
        [SerializeField] private float fillSpeed = 0.5f;
        [SerializeField] private Camera loadingCamera;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private SceneGroup[] sceneGroups;

        private float _targetProgress;
        private bool _isLoading;

        public readonly SceneGroupManager Manager = new SceneGroupManager();

        private void Awake()
        {
            Manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            Manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            Manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        private async void Start()
        {
            await LoadSceneGroup(0);
        }

        private void Update()
        {
            if (!_isLoading) return;

            float currentFillAmount = loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Task LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            _targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.Log("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new();
            progress.Progressed += target => _targetProgress = Mathf.Max(target, _targetProgress);

            EnableLoadingElements();
            await Manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingElements(false);
        }

        private void EnableLoadingElements(bool enable = true)
        {
            _isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }

        public class LoadingProgress : IProgress<float>
        {
            public event Action<float> Progressed;

            const float ratio = 1f;

            public void Report(float value)
            {
                Progressed?.Invoke(value / ratio);
            }
        }
    }
}
