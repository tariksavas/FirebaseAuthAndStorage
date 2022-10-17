namespace Incru.UI
{
    using Incru.User;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject userCreationPanel;

        public static UIController Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            UserManager.Loaded += OnLoaded;
            InitPanel(false);
        }

        private void OnDestroy()
        {
            UserManager.Loaded -= OnLoaded;
        }

        private void OnLoaded()
        {
            var loadScene = SceneManager.LoadSceneAsync("GameScene");
            LoadingProgress.Instance.Progress(loadScene);
        }

        public void InitPanel(bool loggedIn)
        {
            loginPanel.SetActive(!loggedIn);
            userCreationPanel.SetActive(loggedIn);
        }
    }
}
