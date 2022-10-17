namespace Incru.UI
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LoadingProgress : MonoBehaviour
    {
        [SerializeField] private Image ownImage;
        [SerializeField] private TextMeshProUGUI ownText;

        public static LoadingProgress Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
                gameObject.SetActive(false);
            }
        }

        public IEnumerator Progress(AsyncOperation asyncOperation)
        {
            gameObject.SetActive(true);
            while (!asyncOperation.isDone)
            {
                var progress = asyncOperation.progress;
                ownText.text = $"{(int)progress}%";
                ownImage.fillAmount = progress;

                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}